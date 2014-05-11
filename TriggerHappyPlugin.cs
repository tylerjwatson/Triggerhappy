using System;
using Terraria;
using TerrariaApi.Server;
using TerrariaApi;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

namespace TriggerHappy {
	[ApiVersion(1, 15)]
	public class TriggerHappyPlugin : TerrariaPlugin {
		internal List<Chain> chainList = new List<Chain>();
		internal Dictionary<string, Type> actionTypes = null;
		internal Dictionary<string, Type> filterTypes = null;
		internal Dictionary<string, Type> triggerTypes = null;
		public ChainLoader chainLoader = null;

		protected bool enabled = true;

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
		/// Occurs when a packet gets sent from a client to the server.
		/// </summary>
		/// <param name="args">Arguments.</param>
		void Net_GetData(GetDataEventArgs args) {
			if (enabled == false) {
				return;
			}
		}

		public override void Initialize() {
			ServerApi.Hooks.NetGetData.Register(this, Net_GetData);
			LoadTypeCache();
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
			try {
				this.actionTypes = this.chainLoader.LoadAttributeTypes<ActionAttribute>();
				this.filterTypes = this.chainLoader.LoadAttributeTypes<FilterAttribute>();
				this.triggerTypes = this.chainLoader.LoadAttributeTypes<TriggerAttribute>();
			} catch (Exception) {
				//TODO: Log error
			}
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
	}
}

