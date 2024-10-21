using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Program;
using Program.BattlePhaseManager;
using Program.LootPhaseManager;
using Program.LootLibrary;
using Program.StoryPresenter;

namespace Program.PhaseManager
{
    
    class Game 
    {
        public void BeginGame(Player player)
        {
            LootPhase lp = new LootPhase();
            BattlePhase bp = new BattlePhase();
            Program program = new Program();
            int roundCount = 0;
            while (player.levelNumber < 5)
            {
                roundCount++;
                switch (player.levelNumber) 
                {
                    case 1 or 2:
                        lp.Start(player);
                        break;
                    case 3:
                        Console.WriteLine("Cestou do města najdeš spoustu zajímavých předmětů!");
                        Console.ReadKey();
                        for (int i = 0; i < 3; i++) 
                        {
                            lp.Start(player);
                        }
                        break;
                }

                bp.StartBattle(player, roundCount);
                if (player.health > 0)
                {
                    HealPhase(player);
                }
                else
                {
                    Console.WriteLine("Konec Hry");
                    Console.WriteLine("1. Začít znovu?");
                    switch (program.PlayerInput()) 
                    {
                        case 1:
                            Game game = new Game();
                            Presenter storyPresenter = new Presenter();
                            storyPresenter.QuickStart(player);
                            Player newPlayer = new Player();
                            newPlayer.PrefferedSuffix = player.PrefferedSuffix;
                            game.BeginGame(newPlayer);
                            break;
                    };
                    break;
                }
            }
        }
        private void HealPhase(Player player) 
        {
            int healing = player.regeneration;
            Console.WriteLine("Po chvíli odpočinku se cítíš lépe");
            if (player.health + healing + player.armor.HealPhaseBonus >= player.maxHealth + player.armor.MaxHealthIncrease) 
            {
                player.health = player.maxHealth + player.armor.MaxHealthIncrease;
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("Máš plný počet životů! " + player.health + "♥");
                Console.ForegroundColor = ConsoleColor.White;
            }
            else 
            {
                player.health += healing + player.armor.HealPhaseBonus;
                int temp = healing + player.armor.HealPhaseBonus;
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("Regeneroval" + player.PrefferedSuffix + " jsi se o " + temp + "♥\n Nyní máš " + player.health +"♥");
                Console.ForegroundColor = ConsoleColor.White;
            }
            Console.ReadKey();
        }
    }
}
