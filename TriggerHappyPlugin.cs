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

    [ApiVersion(1, 15)]
    public class TriggerHappyPlugin : TerrariaPlugin {
        internal ChainLoader chainLoader = null;
        internal bool enabled = true;
        internal List<Chain> chainList = new List<Chain>();
        internal Dictionary<string, Type> actionTypes = null;
        internal Dictionary<string, Type> filterTypes = null;
        internal Dictionary<string, Type> triggerTypes = null;
        internal volatile List<long> processTimerStats = new List<long>(100);
        internal readonly object processTimerMutex = new object();
        internal long packetCounter = 0;
        internal long handledCounter = 0;
        internal long triggerPullCounter = 0;

        private static readonly Regex commandParamRegex = new Regex("\"[^\"]+\"|[\\w]+");
        private static readonly Regex commandNameRegex = new Regex("!([^ ]+)");

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
            ServerApi.Hooks.ServerCommand.Register(this, Server_Command);
            InitChains();
        }

        /// <summary>
        /// Loads all the chains in the triggerhappy/chains directory and then registers the data handler.
        /// </summary>
        private void InitChains() {
            THLog.Debug("DeregisterGetDataHook");
            ServerApi.Hooks.NetGetData.Deregister(this, Net_GetData);
            THLog.Debug("DeregisterGetDataHook succeeded");

            try {
                LoadTypeCache();
                chainLoader.LoadChainsInDirectory("triggerhappy" + System.IO.Path.DirectorySeparatorChar + "chains");

                if (chainLoader.VerifyChains() == false) {
                    THLog.Log(LogLevel.Error, "Error: Initializing TriggerHappy failed and will be disabled.  Use !chain reload to retry");
                    return;
                }
            } catch (Exception) {
                THLog.Log(LogLevel.Error, "Error: Initializing TriggerHappy failed and will be disabled.  Use !chain reload to retry");
                return;
            }

            THLog.Debug("RegisterGetDataHook");
            ServerApi.Hooks.NetGetData.Register(this, Net_GetData);
            THLog.Debug("RegisterGetDataHook succeeded");
        }

        private void Server_Command(CommandEventArgs args) {
            string command = null;
            string[] arguments = null;
            MatchCollection paramMatch = null;
            int paramCount = 0;

            if (commandNameRegex.IsMatch(args.Command) == false) {
                return;
            }

            command = args.Command;
            if (commandParamRegex.IsMatch(command) == true) {
                paramMatch = commandParamRegex.Matches(command);
                paramCount = paramMatch.Count;
            }

            if (paramCount > 0) {
                arguments = new string[paramCount - 1];
                for (int i = 1; i < paramCount; i++) {
                    arguments[i - 1] = paramMatch[i].Value;
                }
            }

            ProcessCommand(commandParamRegex.Match(command).Groups[0].Value, arguments);
            args.Handled = true;
        }

        private void ProcessCommand(string command, string[] args) {
            THLog.Debug("Command: {0} params: {1}", command, args.Length);
            if (command == "chain") {
                if (args.Length == 0) {
                    //todo: write !chain help
                    return;
                }

                if (args[0] == "list") {
                    foreach (Chain chain in chainList) {
                        THLog.Log(LogLevel.Info, "{0}", chain);
                    }
                } else if (args[0] == "reload") {
                    this.chainList.Clear();
                    InitChains();
                }
            } else if (command == "fw") {
                if (args.Length == 0) {
                    //todo: write !fw help
                    return;
                }
                if (args[0] == "on") {
                    enabled = true;
                } else if (args[0] == "off") {
                    enabled = false;
                } else if (args[0] == "debug") {
                    THLog.debugMode = !THLog.debugMode;
                } else if (args[0] == "perf") {
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

        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);

            if (disposing) {
                ServerApi.Hooks.NetGetData.Deregister(this, Net_GetData);
            }
        }

    }
}

