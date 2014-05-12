using System;
using System.Xml.Linq;

namespace TriggerHappy {

    /// <summary>
    /// Never filter.
    /// 
    /// Unconditionally filters to false and stops the processing of any subsequent filters.
    /// </summary>
    [Filter("Never")]
    public class NeverFilter : Filter {
        public NeverFilter(Chain parentChain, XElement filterElement) : base(parentChain, filterElement) {
        }

        #region implemented abstract members of Filter

        public override bool EvalFilter(ref TerrariaApi.Server.GetDataEventArgs dataEventArgs, ref bool stopProcessing) {
            stopProcessing = true;
            return false;
        }

        #endregion

        public override string ToString() {
            return string.Format("[NeverFilter]");
        }
    }
}

