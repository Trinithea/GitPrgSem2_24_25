
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Program.StoryPresenter
{
    class Presenter
    {
        int partner;
        public void Start(Player player)
        {
            Random rPartner = new Random();
            partner = rPartner.Next(0, 4);
            string[] wordFillIn = [];
            switch (partner) //výběr zda partnerem bude manžel Samuel nebo manželka Samanta (for fun)
            {
                default:
                    wordFillIn = ["se svým manželem Samuelem", "Samuela", "stáli okolo vystrašeného Samuela."];
                    break;
                case 1:
                    wordFillIn = ["se svou manželkou Samantou", "Samantu", "stáli okolo vystrašené Samanty."];
                    break;
                case 2:
                    wordFillIn = ["se svým kocourem Samuelem", "Samuela", "drželi v ruce vystrašeného Samuela."];
                    break;
                case 3:
                    wordFillIn = ["se svou kočkou Samantou", "Samantu", "drželi v ruce vystrašenou Samanty."];
                    break;
            }
            StorySlide("BUCH, BUCH, BUCH!");
            StorySlide("Vrátil se.");
            StorySlide("Toto agresivní bouchání na dveře slyšíte dnes již po druhé.");
            StorySlide("Zdá se, že nepřišel sám. Z venku se ozývá cinkání řetězů a šeptání hlasů.");
            StorySlide("Se synem vévody Smaragdového vrchu se nemáte rádi již od té doby" +
                "\nco jste se sem společně " + wordFillIn[0] + " přistěhovali před pěti lety");
            StorySlide("Podle tvého názoru to je namyšlený spratek a blbeček vysávající veškeré jmění svého otce,  který se mu jinak tolik nevěnuje");
            StorySlide("Baltazar Gosse, syn vévody, si dovoluje na všechny a nikdo si netroufá se bránit.");
            StorySlide("Nikdo až na tebe.");
            StorySlide("Proto dnes přišel po druhé.");
            StorySlide("S nervózním pohledem na " + wordFillIn[1] + " otevíráš dveře");
            StorySlide("BALTAZAR GOSSE: \"Nazdárek!\"");
            StorySlide("PRÁSK!");
            StorySlide("Padáš na zem. Cítíš silnou tupou bolest v obličeji. Někdo křičí.");
            StorySlide("Do tvého domu za hlasitého dupotu a smíchu vlétli Baltazar a jeho přátelé.");
            StorySlide("Pár z nich tě zvedli ze země a násilím tě vlekli do kočáru zaparkovaným před tvým domem.");
            StorySlide("BALTAZAR GOSSE: \"Ha! Tohle je od teď náš dům!\"");
            StorySlide("Když ses naposledy ohlédl" + player.PrefferedSuffix + " na svůj dům, zřel" + player.PrefferedSuffix + " jsi jak Baltazarovo muži  " + wordFillIn[2]);
            StorySlide("Pak vidíš pouze temno.");
            StorySlide("Když ses pak probudil" + player.PrefferedSuffix + ", zjistil" + player.PrefferedSuffix + " jsi, že se nacházíš uprostřed nějakého neznámého lesa.");
            StorySlide("Nedaleko vidíš stopy kol kočáru");
            StorySlide("Baltazarovo muži ti vzali veškeré tvé zbraně, věci a oblečení");
            StorySlide("V lesích žije spousta nestvůr a záhadných tvorů");
            StorySlide("Tvé šance na přežití jsou velmi nízké, avšak Baltazar musí zaplatit");
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            StorySlide("Tímto začíná tvá cesta zpět na Smaragdový vrch");
            Console.ForegroundColor = ConsoleColor.White;
        }
        public void QuickStart(Player player) 
        {
            Random rPartner = new Random();
            partner = rPartner.Next(0, 4);
            string wordFillIn;
            switch (partner) //výběr zda partnerem bude manžel Samuel nebo manželka Samanta (for fun)
            {
                default:
                    wordFillIn = "Mého manžela Samuela mi nikdo nevezme!";
                    break;
                case 1:
                    wordFillIn = "Mou manželku Samantu mi nikdo nevezme";
                    break;
                case 2:
                    wordFillIn = "Mého kocoura Samuela mi nikdo nevezme!";
                    break;
                case 3:
                    wordFillIn = "Mou kočku Samuantu mi nikdo nevezme!";
                    break;
            }
            StorySlide(wordFillIn);
        }
        public void Tutorial()
        {
            StorySlide("Zmáčkněte enter...");
            StorySlide("Pro postoupení ve hře zmáčknete vždy enter...");
            Console.WriteLine("Pro výběr možností píšete čísla (Př. 1, 2, 3 atd...)");
            Console.WriteLine("Zkus to! :");
        }

        public void Progress(Player player)
        {
            StorySlide("Vidíš světlo. Vycházíš z lesa.");
            StorySlide("Dorazíš na rozcestí, u kterého stojí stánek.");
            StorySlide("Přiblížíš se ke stánku, je v něm stařena.");
            StorySlide("Ptáš se stařeny, zda neví kudy vede nejkratší cesta na Smaragdový vrch");
            StorySlide("STAŘENA: Nejkratší cesta vede támhletudy, ale varuji Vás! Nikdo z ní ještě nevyšel živ!");
            StorySlide("Stařena ukázala na cestičku vedoucí skrz enormní rokli. Zahalovala ji temná mlha.");
            StorySlide("Stařena ti darovala záhadný červený elixír.");
            StorySlide("Elixír jsi vypil" + player.PrefferedSuffix + ". Chutnal po jahodách.");
            StorySlide("Maximální počet životů se ti zvýšil o 10!");
            player.maxHealth += 10;
            player.health = player.maxHealth + player.armor.MaxHealthIncrease;
            StorySlide("Máš " + player.health + "životů");
            StorySlide("S nově získaným odhodláním se necháš pohltit mrazivou atmosférou rokliny a vydáváváš se hlouběji do ní.");
            player.levelNumber = 2;
            player.regeneration = 7;
        }
        public void EndGame1(Player player) 
        {
            StorySlide("Opouštíš chladnou atmosféru rokliny.");
            StorySlide("Před tebou se rozléhá město.");
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            StorySlide("Smaragdový vrch.");
            Console.ForegroundColor = ConsoleColor.White;
            StorySlide("Vkráčíš do města.");
            StorySlide("Na náměstí v hospodě si objednáš korbel medoviny na posilněnou. Tvůj maximální počet životů se zvedá o 5! (" + (player.maxHealth + 5 + player.armor.MaxHealthIncrease) + ")");
            player.maxHealth += 5;
            player.health = player.maxHealth + player.armor.MaxHealthIncrease;
            StorySlide("V hospodě narazíš na pár poskoků Baltazara.");
            StorySlide("Jeden z nich tě zahlédl a ihned upozornil své přátele.");
            StorySlide("Všichni se zvedáte od svých nápojů.");
            StorySlide("Tasíte zbraně.");
            StorySlide("Hospodský se připravuje na pořádnou hospodskou rvačku.");
            player.levelNumber = 3;

        }
        public void EndGame2(Player player) 
        {
            string[] wordFillIn = [];
            switch (partner) //výběr zda partnerem bude manžel Samuel nebo manželka Samanta (for fun)
            {
                default:
                    wordFillIn = ["Samuela", "objímá Samuela kolem ramen"];
                    break;
                case 1:
                    wordFillIn = ["Samantu", "objímá Samantu kolem ramen"];
                    break;
                case 2:
                    wordFillIn = ["Samuela", "drží tvého kocoura Samuela v náruči"];
                    break;
                case 3:
                    wordFillIn = ["Samantu", "drží tvou kočku Samantu v náruči"];
                    break;
            }

            StorySlide("Baltazarovo poskoci leží na zemi.");
            StorySlide("Hospodský na tebe křičí, že máš vypadnout.");
            StorySlide("Dopiješ svou medovinu a uposlechneš ho. (Máš plný počet životů)");
            player.health = player.maxHealth + player.armor.MaxHealthIncrease;
            StorySlide("S nově nabitými silami po těžké bitvě osedláš jednoho z ořů zesnulých Baltazarovo poskoků a s větrem v uších uháníš po hlavní stezce směrem ke svému domu.");
            StorySlide("Jedeš zachránit " + wordFillIn[0] + " z krutého domácího vězení Baltazara.");
            StorySlide("Prr!");
            StorySlide("Přinutíš svého oře aby se zastavil.");
            StorySlide("Stojíš před svým domem.");
            StorySlide("Slézáš z koně a tasíš zbraně. Připravuješ se na nejhorší.");
            StorySlide("ROZKOPNEŠ DVEŘE SVÉHO DOMU!");
            StorySlide("Vidíš Baltazara jak " + wordFillIn[1] + ".");
            StorySlide("Baltazar zvedne oči.");
            StorySlide("BALTAZAR GOSSE: Ty?... Jak?");
            StorySlide("Baltazar odhodí " + wordFillIn[0] + " stranou.");
            StorySlide("BALTAZAR GOSSE: Ty si nedáš pokoj co?! Tak pojď");
            StorySlide("Pravíš Baltazarovi, že si jdeš vzít zpátky to co je zákonem tvé...");
            Console.ForegroundColor = ConsoleColor.Red;
            StorySlide("Jeho hlavu!");
            Console.ForegroundColor = ConsoleColor.White;
            player.levelNumber = 4;


        }
        public void Victory(Player player) 
        {
            string[] wordFillIn = [];
            switch (partner) //výběr zda partnerem bude manžel Samuel nebo manželka Samanta (for fun)
            {
                default:
                    wordFillIn = ["Samuela", "Obejmeš ho"];
                    break;
                case 1:
                    wordFillIn = ["Samantu", "Obejmeš jí"];
                    break;
                case 2:
                    wordFillIn = ["Samuela", "Vezmeš si ho do náruče"];
                    break;
                case 3:
                    wordFillIn = ["Samantu", "Vezmeš si ji do náruče"];
                    break;
            }
            StorySlide("Baltazar se ti naposled podívá do očí.");
            StorySlide("Otevře pusu, chce z posledních sil něco říct.");
            StorySlide("BALTAZAR: Jen počkej... až se o tomhle doví otec...");
            StorySlide("Baltazar klesl na zem a naposled vydechl.");
            StorySlide("Pustíš své zbraně na zem. Podíváš se na " + wordFillIn[0]);
            StorySlide(wordFillIn[1]);
            StorySlide("Baltazar měl bohužel v něčem pravdu");
            StorySlide("Jeho otec bude zaručeně chtít pomstít svého syna.");
            StorySlide("Smaragdový vrch byl hezký domov, ale nyní je čas se přestěhovat.");
            StorySlide("A to hned.");
            StorySlide("Svět dokáže být krutý a nespravedlivý.");
            Console.ForegroundColor = ConsoleColor.Yellow;
            StorySlide("Rychle si zabalíte vaše nejdůležitější věci do pytlů, nasednete na oře a společně vyjedete za západem slunce vstříc novému a, doufejme, klidnějšímu životu");
            Console.ForegroundColor = ConsoleColor.White;
            StorySlide("Konec.");
            player.levelNumber = 5;



        }
        private void StorySlide(string story)
        {
            Console.WriteLine(story);
            Console.ReadKey();

        }
    }
    
}
