// See https://aka.ms/new-console-template for more information

using System.Reflection;
using System.Text;
using PokemonRipoff.Scenes;
using PokemonRipoff.UI;
using PokemonRipoff.Pokemons;

namespace PokemonRipoff {
	class Program {
		static Scenes.Scene GenerateInventoryScene() {
			Scenes.Scene inventoryScene = new Scenes.Scene("inventory");
			inventoryScene.RegisterPreRenderHook((renderer, game) => {
				int lineIndex = 0;
				renderer.Buffer(new TextElement("inventoryTitle", "Inventory", TextDecoration.Underline, Anchor.Top));
				lineIndex++;
				renderer.Buffer(new TextElement("pokeballTitle", "Pokéballs", TextDecoration.Underline, Anchor.TopLeft,
					lineIndex));
				lineIndex++;

				for (int i = 0; i < game.State.Pokeballs.Count; i++) {
					game.State.Pokeballs.TryGetValue(i, out int pokeballCount);
					renderer.Buffer(new TextElement("pokeballCount",
						Pokeballs.Pokeball.FromId(i).DisplayName + " - " + pokeballCount,
						TextDecoration.None, Anchor.TopLeft, lineIndex));
					lineIndex++;
				}

				renderer.Buffer(new TextElement("help", $"Press [{ConsoleKey.Escape.ToString()}] to go back to map",
					TextDecoration.None,
					Anchor.BottomLeft));
			});

			inventoryScene.RegisterKeyEventHook((keyInfo, game) => {
				switch (keyInfo.Key) {
					case ConsoleKey.Escape:
						game.ActivateScene("main");
						break;
				}
			});

			return inventoryScene;
		}

		static Scene GenerateHospitalScene() {
			Scene hospitalScene = new Scene("hospital");

			hospitalScene.AddUIElement(new TextElement("title", "Hospital", TextDecoration.Underline, Anchor.Top));
			TextElement question = new TextElement("question", "Do you want to heal your currently active pokemon?",
				TextDecoration.None, Anchor.TopLeft, 1);
			hospitalScene.AddUIElement(question);
			hospitalScene.AddUIElement(new TextElement("hospital-help",
				$"Press [{ConsoleKey.Enter}] to heal, [{ConsoleKey.Escape}] to go back to map", TextDecoration.None,
				Anchor.BottomLeft));

			hospitalScene.RegisterKeyEventHook((info, game) => {
				switch (info.Key) {
					case ConsoleKey.Enter:
						game.State.Pokemons.TryGetValue(game.State.ActivePokemonId,
							out Pokemon? player);
						if (player == null) {
							throw new Exception("No pokemon selected for player");
						}

						player.Health = player.MaxHealth;
						question.Text = $"You healed {player.DisplayName}'s health to {player.MaxHealth}!"
							.ToCharArray();
						break;

					case ConsoleKey.Escape:
						game.ActivateScene("main");
						break;
				}
			});

			return hospitalScene;
		}

		static void Main(string[] args) {
			SpriteManager.LoadSprites(Path.Combine(
				Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty,
				@"..\..\..\Data\Sprites"));

			Game game = new Game(new Renderer(new RGBFormatter()));

			// Setup start scene
			StartScene startScene = new StartScene();
			game.AddScene(startScene);
			
			// Setup intro scene
			IntroScene introScene = new IntroScene();
			game.AddScene(introScene);

			// Setup main map scene
			MapScene mainScene = new MapScene("main");
			game.AddScene(mainScene);

			// Setup inventory scene
			Scene inventoryScene = GenerateInventoryScene();
			game.AddScene(inventoryScene);

			// Setup pokedex scene
			PokedexScene pokedexScene = new PokedexScene();
			game.AddScene(pokedexScene);

			// Setup shop scene
			ShopScene shopScene = new ShopScene("shop");
			game.AddScene(shopScene);

			Scene hospitalScene = GenerateHospitalScene();
			game.AddScene(hospitalScene);

			game.ActivateScene("start");
			game.Run();
		}
	}
}