using System;
using System.Xml.Linq;
using System.Linq;

namespace TriggerHappy {

    /// <summary>
    /// Disconnect player action.
    /// 
    /// Immediately terminates a player from the game.
    /// </summary>
    [Action("DisconnectPlayer")]
    public class DisconnectPlayerAction : Action {
        public DisconnectPlayerAction(Chain parentChain, XElement actionElement) : base(parentChain, actionElement) {
        }

        #region implemented abstract members of Action

        public override void EvalAction(ref TerrariaApi.Server.GetDataEventArgs dataArgs, ref bool stopProcessing) {
            Terraria.Player player = Terraria.Main.player.ElementAtOrDefault(dataArgs.Msg.whoAmI);
            if (player == null) {
                //TODO: Log error
                return;
            }

            Terraria.NetMessage.SendData((int)PacketTypes.Disconnect, player.whoAmi, text: "");
        }

        #endregion

        public override string ToString() {
            return string.Format("[DisconnectPlayerAction]");
        }
    }
}

