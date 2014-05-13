using System;
using System.IO;
using TerrariaApi;
using Terraria;
using TerrariaApi.Server;

namespace TriggerHappy.TestWrapper {

    public class Program {
        //
        // Static Fields
        //
        private static Main Game;
        //
        // Static Methods
        //
        private static void Main(string[] args) {
            try {
                Game = new Main();
            
                Terraria.Main.SavePath = Path.Combine(Environment.GetFolderPath(), ".");
                Terraria.Main.WorldPath = Path.Combine(Terraria.Main.SavePath, "Worlds");
                Terraria.Main.PlayerPath = Path.Combine(Terraria.Main.SavePath, ".");
                try {
                    Console.WriteLine(ServerApi.ApiVersion + " " + Terraria.Main.versionNumber2);
                    ServerApi.Initialize(args, ProgramServer.Game);
                } catch (Exception ex) {
                    ServerApi.LogWriter.ServerWriteLine(+ex, TraceLevel.Error);
                    Console.ReadLine();
                    return;
                }
                ProgramServer.Game.DedServ();
                ServerApi.DeInitialize();
            } catch (Exception ex2) {
                ServerApi.LogWriter.ServerWriteLine(+ex2, TraceLevel.Error);
                Console.ReadLine();
            }
        }
    }
}

