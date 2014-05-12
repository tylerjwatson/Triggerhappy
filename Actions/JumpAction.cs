using System;
using System.Xml.Linq;

namespace TriggerHappy {

    /// <summary>
    /// Action that jumps to another chain and transfers control to that chain.
    /// 
    /// This action stops further processing on this chain.
    /// </summary>
    [Action("Jump")]
    public class JumpAction : Action {
        protected string chainName = null;

        public JumpAction(Chain parentChain, XElement actionElement) : base(parentChain, actionElement) {
            if (actionElement.HasAttributes == true && actionElement.Attribute("ToChain") != null) {				
                chainName = actionElement.Attribute("ToChain").Value;
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

            destinationChain.ProcessChain(ref dataArgs, true);
            stopProcessing = true;
        }

        #endregion

        public override string ToString() {
            return string.Format("[JumpAction]");
        }
    }
}

