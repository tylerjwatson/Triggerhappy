using System;
using System.Xml.Linq;

namespace TriggerHappy {

    /// <summary>
    /// Calls another chain and keeps on processing other actions.
    /// </summary>
    [Action("Call")]
    public class CallAction : Action {
        protected string chainName = null;

        public CallAction(Chain parentChain, XElement actionElement) : base(parentChain, actionElement) {
            if (actionElement.HasAttributes == true && actionElement.Attribute("Chain") != null) {				
                chainName = actionElement.Attribute("Chain").Value;
            }
        }

        #region implemented abstract members of Action

        public override void EvalAction(ref TerrariaApi.Server.GetDataEventArgs dataArgs, ref bool stopProcessing) {
            Chain destinationChain = null;
            if (parentChain == null || string.IsNullOrEmpty(chainName) == true) {
                //TODO: Log error
                return;
            }

            if ((destinationChain = parentChain.Parent.GetChainByName(chainName)) == null) {
                return;
            }

            destinationChain.ProcessChain(ref dataArgs);
        }

        #endregion

        public override string ToString() {
            return string.Format("[CallAction] Chain={0}", chainName);
        }
    }
}

