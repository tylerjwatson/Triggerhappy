using System;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace TriggerHappy {
	public class Chain {
		protected XElement chainElement;
		public string Name { get; protected set; }
		public TriggerHappyPlugin Parent { get; protected set; }
		public List<Filter> Filters { get; protected set; }
		public List<Trigger> Triggers { get; protected set; }
		public List<Action> Actions { get; protected set; }


		public Chain(TriggerHappyPlugin parent, XElement chainElement) {
			this.Parent = parent;
			this.chainElement = chainElement;
		
			if (this.chainElement.HasAttributes == true && this.chainElement.Attribute("Name") != null) {
				this.Name = this.chainElement.Attribute("Name").Value;
			}

			LoadFilters(ref chainElement);
			LoadTriggers(ref chainElement);
			LoadActions(ref chainElement);
		}

		protected void LoadActions(ref XElement chainElement) {
			if (this.chainElement.Element("Actions") != null) {
				Actions = new List<Action>();

				foreach (XElement element in this.chainElement.Element("Actions").Elements()) {
					Type actionType = null;
					Action actionInstance = null;
					if ((actionType = Parent.GetActionTypeByName(element.Name.ToString())) == null) {
						THLog.Log(LogLevel.Error, "Can't find an Action by the name " + element.Name.ToString());
						continue;
					}

					try {
						if ((actionInstance = Activator.CreateInstance(actionType, new object[] { this, element }) as Action) == null) {
							THLog.Log(LogLevel.Error, "Can't create an instance of the Action " + element.Name.ToString());
							continue;
						}
					} catch (Exception) {
						THLog.Log(LogLevel.Error, "Can't create an instance of the Action " + element.Name.ToString());
						continue;
					}

					Actions.Add(actionInstance);
				}
			}
		}

		/// <summary>
		/// Loads the triggers for this chain by everything specified under the Triggers collection
		/// </summary>
		/// <param name="chainElement">Chain element.</param>
		protected void LoadTriggers(ref XElement chainElement) {
			if (this.chainElement.Element("Triggers") != null) {
				Triggers = new List<Trigger>();

				foreach (XElement element in this.chainElement.Element("Triggers").Elements()) {
					Type triggerType = null;
					Trigger triggerInstance = null;
					if ((triggerType = Parent.GetTriggerTypeByName(element.Name.ToString())) == null) {
						//TODO: Log error, trigger not found
						continue;
					}

					try {
						if ((triggerInstance = Activator.CreateInstance(triggerType, this, element) as Trigger) == null) {
							//TODO: Log error, cannot create instance of trigger
							continue;
						}
					} catch (Exception) {
						//TODO: Log error, cannot create instance of trigger
						continue;
					}

					Triggers.Add(triggerInstance);
				}
			}
		}

		/// <summary>
		/// Loads the filters for this chain by everything specified under the Filters collection
		/// </summary>
		/// <param name="chainElement">Chain element.</param>
		protected void LoadFilters(ref XElement chainElement) {
			if (this.chainElement.Element("Filters") != null) {
				Filters = new List<Filter>();

				foreach (XElement filterElement in this.chainElement.Element("Filters").Elements()) {
					Type filterType = null;
					Filter filterInstance = null;
					if ((filterType = Parent.GetFilterTypeByName(filterElement.Name.ToString())) == null) {
						//TODO: Log error, filter not found
						continue;
					}

					try {
						if ((filterInstance = Activator.CreateInstance(filterType, new object[] { this, filterElement }) as Filter) == null) {
							//TODO: Log error, cannot create instance of filter
							continue;
						}
					} catch (Exception) {
						//TODO: Log error, cannot create instance of filter
						continue;
					}

					Filters.Add(filterInstance);
				}
			}
		}

		/// <summary>
		/// Occurs when a trigger pulls in this chain.  From here the chains actions get invoked.
		/// </summary>
		internal void TriggerPulled(Trigger trigger, ref TerrariaApi.Server.GetDataEventArgs dataArgs) {
			//TODO: Log trigger pull
			ProcessActions(ref dataArgs);
		}

		/// <summary>
		/// Processes the actions on this chain.
		/// </summary>
		/// <param name="dataArgs">Data arguments.</param>
		protected void ProcessActions(ref TerrariaApi.Server.GetDataEventArgs dataArgs) {
			foreach (Action a in this.Actions) {
				bool stopProcessing = false;

				a.EvalAction(ref dataArgs, ref stopProcessing);
				if (stopProcessing == true) {
					break;
				}
			}
		}

		/// <summary>
		/// Processes the chain with the supplied data arguments, and optionally ignores all the filters on it.
		/// </summary>
		/// <param name="dataArgs">Data arguments.</param>
		/// <param name="ignoreFilters">If set to <c>true</c> ignore filters.</param>
		public void ProcessChain(ref TerrariaApi.Server.GetDataEventArgs dataArgs, bool ignoreFilters = false) {
			if (ChainCanProcess(ref dataArgs, ignoreFilters) == false) {
				return;
			}

			ProcessTriggers(ref dataArgs);
		}

		/// <summary>
		/// Processes the triggers in this chain.  If a trigger gets pulled then the TriggerPulled event will be
		/// raised and no further triggers will be processed for this loop.
		/// </summary>
		/// <param name="dataArgs">Data arguments.</param>
		protected void ProcessTriggers(ref TerrariaApi.Server.GetDataEventArgs dataArgs) {
			foreach (Trigger t in this.Triggers) {
				bool stopProcessing = false;

				t.EvalTrigger(ref dataArgs, ref stopProcessing);
				if (stopProcessing == true) {
					break;
				}
			}
		}

		/// <summary>
		/// Gets a flag indicating whether the chain is elegible to execute from it's specified
		/// filters.
		/// </summary>
		/// <returns><c>true</c>, if the chain can be processed, <c>false</c> otherwise.</returns>
		/// <param name="dataArgs">Data arguments.</param>
		protected bool ChainCanProcess(ref TerrariaApi.Server.GetDataEventArgs dataArgs, bool ignoreFilters) {
			if (ignoreFilters == true) {
				return true;
			}

			if (this.Filters == null) {
				return false;
			}

			foreach (Filter f in this.Filters) {
				bool stopProcessing = false;

				if (f.EvalFilter(ref dataArgs, ref stopProcessing) == true) {
					return true;
				}
				if (stopProcessing == true) {
					return false;
				}
			}
			return false;
		}
	}
}

