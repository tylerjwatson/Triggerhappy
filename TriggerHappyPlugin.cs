using System;
using Terraria;
using TerrariaApi.Server;
using TerrariaApi;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Text.RegularExpressions;

namespace TriggerHappy {

    [ApiVersion(1, 16)]
    public class TriggerHappyPlugin : TerrariaPlugin {
        public ChainLoader chainLoader = null;
        public bool enabled = true;
        internal List<Chain> chainList = new List<Chain>();
        internal Dictionary<string, Type> actionTypes = null;
        internal Dictionary<string, Type> filterTypes = null;
        internal Dictionary<string, Type> triggerTypes = null;
        internal volatile List<long> processTimerStats = new List<long>(100);
        internal readonly object processTimerMutex = new object();
        internal long packetCounter = 0;
        internal long handledCounter = 0;
        internal long triggerPullCounter = 0;

        #region "TerrariaPlugin overrides"

        public override string Author {
            get {
                return "Wolfje, Ijwu and others.";
            }
        }

        public override string Description {
            get {
                return "Packet-level protection for Terraria servers";
            }
        }

        public override string Name {
            get {
                return "Trigger Happy";
            }
        }

        public override Version Version {
            get {
                return Assembly.GetExecutingAssembly().GetName().Version;
            }
        }

        #endregion

        /// <summary>
        /// Updates the internal processor statistics with the number of packets
        /// processed and the time it took.
        /// </summary>
        /// <param name="time"></param>
        void UpdateProcessorStats(long time) {
            lock (processTimerMutex) {
                processTimerStats.Insert(0, time);
                if (processTimerStats.Count > 100) {
                    processTimerStats.RemoveAt(processTimerStats.Count - 1);
                }
            }
        }

        /// <summary>
        /// Occurs when a packet gets sent from a client to the server.
        /// </summary>
        /// <param name="args">Arguments.</param>
        void Net_GetData(GetDataEventArgs args) {
            Chain incomingChain = null;
            string packetInfo = null;
            Stopwatch processTimer = null;

            if (enabled == false || (incomingChain = GetIncomingChain()) == null) {
                return;
            }

            processTimer = new Stopwatch();
            processTimer.Start();
            packetInfo = string.Format("{0} len {1} from client slot {2}", args.MsgID.ToString("X"), args.Length, args.Msg.whoAmI);

            THLog.Debug("Packet in: " + packetInfo);
            incomingChain.ProcessChain(ref args, false);
            if (args.Handled == false) {
                THLog.Debug("Packet out: " + packetInfo);
                Interlocked.Increment(ref packetCounter);
            } else {
                Interlocked.Increment(ref handledCounter);
            }

            processTimer.Stop();
            UpdateProcessorStats(processTimer.ElapsedMilliseconds);

            THLog.Debug("Chain process time: {0}ms", processTimer.ElapsedMilliseconds);
        }

        public override void Initialize() {
            try {
                LoadTypeCache();
                chainLoader.LoadChainsInDirectory("triggerhappy" + System.IO.Path.DirectorySeparatorChar + "chains");

                if (chainLoader.VerifyChains() == false) {
                    THLog.Log(LogLevel.Error, "Error: Initializing TriggerHappy failed, unable to start.");
                    return;
                }
            } catch (Exception) {
                THLog.Log(LogLevel.Error, "Error: Initializing TriggerHappy failed, unable to start.");
                return;
            }

            ServerApi.Hooks.ServerCommand.Register(this, Server_Command);
            ServerApi.Hooks.NetGetData.Register(this, Net_GetData);
        }

        private void Server_Command(CommandEventArgs args) {
            string[] spaceSplit = null;
            if (args.Command.StartsWith("!") == false) {
                return;
            }

            spaceSplit = args.Command.Split(' ');
            if (spaceSplit.Length == 0) {
                return;
            }

            ProcessCommand(spaceSplit[0], spaceSplit.Length > 1 ? spaceSplit.Skip(1).ToArray() : null);

            Console.WriteLine("blah");
            args.Handled = true;
        }

        private void ProcessCommand(string command, string[] args) {
            if (command == "!stats") {
                THLog.Log(LogLevel.Info, "{0} packets in, {1} filtered, {2} pulled.", packetCounter, handledCounter, triggerPullCounter);
                lock (processTimerMutex) {
                    if (processTimerStats.Count == 0) {
                        THLog.Log(LogLevel.Info, "Perf: 0ms avg, 0ms min, 0ms max.");
                        return;
                    }
                    THLog.Log(LogLevel.Info, "Perf: {0:0.000}ms avg, {1:0.000}ms min, {2:0.000}ms max.", processTimerStats.Average(), processTimerStats.Min(), processTimerStats.Max());
                }
            }
        }

        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);

            if (disposing) {
                ServerApi.Hooks.NetGetData.Deregister(this, Net_GetData);
            }
        }

        public TriggerHappyPlugin(Main game) : base(game) {
            this.chainLoader = new ChainLoader(this);
        }

        public void LoadTypeCache() {
            this.actionTypes = this.chainLoader.LoadAttributeTypes<ActionAttribute>();
            this.filterTypes = this.chainLoader.LoadAttributeTypes<FilterAttribute>();
            this.triggerTypes = this.chainLoader.LoadAttributeTypes<TriggerAttribute>();
        }

        #region "Trigger Accessors"
        public Chain GetIncomingChain() {
            return GetChainByName("__INCOMING__");
        }

        public Chain GetChainByName(string chainName) {
            return chainList.FirstOrDefault(i => i.Name.Equals(chainName));
        }

        public Type GetActionTypeByName(string name) {
            if (actionTypes == null || actionTypes.ContainsKey(name) == false) {
                return null;
            }

            return actionTypes[name];
        }

        public Type GetFilterTypeByName(string name) {
            if (filterTypes == null || filterTypes.ContainsKey(name) == false) {
                return null;
            }

            return filterTypes[name];
        }

        public Type GetTriggerTypeByName(string name) {
            if (triggerTypes == null || triggerTypes.ContainsKey(name) == false) {
                return null;
            }

            return triggerTypes[name];
        }
        #endregion

    }
}

