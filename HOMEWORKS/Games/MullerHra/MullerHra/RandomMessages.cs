using Program;
using Program.MonsterManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RM
{
    class RandomMessages
    {
        public string MonsterSpawn(Monster monster)
        {
        Random r = new Random();
        string[] messages = ["Cestu ti zablokoval" + monster.Suffix,
            "Ze křoví na tebe náhle vyskočil" + monster.Suffix,
            "Připlížil" + monster.Suffix + " se k tobě",
            "Z temného stínu se vynořil" + monster.Suffix,
            "Překvapil" + monster.Suffix + " tě",
            "Z jeskyně na tebe vyletěl" + monster.Suffix,
            "Na zem tě silně srazil" + monster.Suffix,
            "Vycenil" + monster.Suffix + " na tebe zuby" ,
            "Objevl" + monster.Suffix + " se"];
        return messages[r.Next(0, messages.Length - 1)];
        }
        public string MonsterDeathMessage(Monster am)
        {
            Random r = new Random();
            string[] messages = ["byl" + am.Suffix + " rozdrcen" + am.Suffix + " na prášek",
                    "byl" + am.Suffix + " přetržen" + am.Suffix + " vejpůl",
                    "byl" + am.Suffix + " rozpuštěno" + am.Suffix + " na ",
                    ];
            return messages[r.Next(0, messages.Length - 1)];
        }
        public string PlayerDeathMessage(Monster am)
        {
            Random r = new Random();
            string[] messages = ["tě rozprskl" + am.Suffix + " na kostičky",
                    "tě rozdrásal" + am.Suffix + " na kousíčky",
                    "tě roztrhl" + am.Suffix + " na tři půlky",
                    ];
            return messages[r.Next(0, messages.Length - 1)];
        }
        public string LootSpawn(int levelNumber)
        {
            Random r = new Random();
            string[] messages;
            switch (levelNumber) 
            {
                default:
                    messages = ["V houští zahlédneš jak se něco blýská",
                    "V opuštěném kempu leží pár předmětů",
                    "Kostlivec ležící na zemi měl u sebe něco zajímavého",
                    "Nalezneš truhlu s pokladem",
                    "Zdá se, že z odjíždějícího kočáru něco vypadlo",
                    "Milý stařec ti nabízí pomoc",
                    "Cítíš požehnání lesních přízraků. Před tebou se objevil předmět",
                    "Z mrtvé nestvůry ležící na zemi něco vypadlo",
                    "Pomocí materiálů v lese si dokážeš něco vyrobit",
                    "V půdě je něco zahrabaného"];
                    return messages[r.Next(0, messages.Length - 1)];
                case 2:
                    messages = ["V jeskyni se něco blýská",
                    "Pod kamenem se schovával nějaký předmět",
                    "Kostlivec ležící na zemi měl u sebe něco zajímavého",
                    "Nalezneš truhlu s pokladem",
                    "Z úkrytu pašeráku můžeš něco vzít",
                    "Milý stařec ti nabízí pomoc",
                    "Něco ti zašeptalo do ucha. Před tebou se objevil předmět",
                    "Z mrtvé nestvůry ležící na zemi něco vypadlo",
                    "Pomocí materiálů v roklině si dokážeš něco vyrobit",
                    "V půdě je něco zahrabaného"];
                    return messages[r.Next(0, messages.Length - 1)];
            }
        }
    }
}
