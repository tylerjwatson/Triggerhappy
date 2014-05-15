using System;
using System.Xml.Linq;

namespace TriggerHappy {

    [Filter("Always")]
    public class AlwaysFilter : Filter {
        public AlwaysFilter(Chain parentChain, XElement filterElement) : base(parentChain, filterElement) {
        }

        #region implemented abstract members of Filter

        public override bool EvalFilter(ref TerrariaApi.Server.GetDataEventArgs dataEventArgs, ref bool stopProcessing) {
            stopProcessing = true;
            return true;
        }

        #endregion

        public override string ToString() {
            return string.Format("[AlwaysFilter]");
        }
    }
}

