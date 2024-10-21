using System.Reflection;
using PokemonRipoff.Cells;
using PRMap = PokemonRipoff.Map;
using PokemonRipoff.UI;
using PokemonRipoff.Pokemons;

namespace PokemonRipoff.Scenes;

public class MapScene : Scene {
	public PRMap.Map Map { get; set; }
	private ConsoleKey _inventoryKey = ConsoleKey.E;
	private ConsoleKey _pokedexKey = ConsoleKey.P;
	private string _message = "";

	public MapScene(string id) : base(id) {
		string mapFilePath =
			Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty,
				@"..\..\..\mapfile");
		Map = PRMap.Map.Load(File.ReadAllBytes(mapFilePath));

		TextElement helperEl = new TextElement("help",
			$"[{_inventoryKey.ToString()}] - Inventory, [{_pokedexKey.ToString()}] - Pokédex", TextDecoration.None,
			Anchor.BottomLeft, -1);
		AddUIElement(helperEl);

		RegisterPreRenderHook((renderer, game) => {
			renderer.Buffer(new TextElement("money", $"Money - {game.State.PlayerMoney}", TextDecoration.None,
				Anchor.BottomLeft, -2));

			renderer.Buffer(Map);

			if (_message != string.Empty) {
				renderer.Buffer(new TextElement("message", _message, TextDecoration.None, Anchor.BottomLeft));
			}

			if (game.State.Message != string.Empty) {
				if (game.State.MessageTimeout > 0) {
					renderer.Buffer(new TextElement("gameMessage", game.State.Message, TextDecoration.None,
						Anchor.BottomRight));
					game.State.MessageTimeout -= 1;
				} else {
					game.State.Message = "";
				}
			}
		});

		RegisterKeyEventHook((info, game) => {
			switch (info.Key) {
				case ConsoleKey.W:
					Map.Move(0, -1);
					break;

				case ConsoleKey.A:
					Map.Move(-1, 0);
					break;

				case ConsoleKey.D:
					Map.Move(1, 0);
					break;

				case ConsoleKey.S:
					Map.Move(0, 1);
					break;

				case ConsoleKey.E:
					game.ActivateScene("inventory");
					break;

				case ConsoleKey.P:
					game.ActivateScene("pokedex");
					break;

				case ConsoleKey.Enter:
					switch (Map.GetCurrCell().Type) {
						case CellType.Shop:
							game.ActivateScene("shop");
							break;
						
						case CellType.Hospital:
							game.ActivateScene("hospital");
							break;
					}

					break;
			}

			Random rnd = new Random();
			switch (Map.GetCurrCell().Type) {
				case CellType.Grass:
					game.State.Pokemons.TryGetValue(game.State.ActivePokemonId,
						out Pokemon? player);
					if (player == null) {
						throw new Exception("No pokemon selected for player");
					}

					if (player.Health <= 0) {
						break;
					}
						
					if (rnd.Next(0, 100) < Config.ENCOUNTER_CHANCE * 100) {
						int[] pokemonsIds = Pokemon.GetAllIds();
						FightScene encounter = new FightScene("encounter",
							Pokemon.FromId(pokemonsIds[rnd.Next(0, pokemonsIds.Length)]));
						game.AddScene(encounter);
						game.ActivateScene(encounter);
					}

					break;

				case CellType.Water:
					if (rnd.Next(0, 100) < Config.WATER_MONEY_CHANCE * 100) {
						int foundMoney = rnd.Next(0, Config.WATER_MONEY_MAX) + 1;
						_message = $"You found {foundMoney} money in the water!";
						game.State.PlayerMoney += foundMoney;
					}
					break;
				
				case CellType.Shop:
					_message = "Shop! Press [Enter] to go inside!";
					break;
				
				case CellType.Hospital:
					_message = "Hospital! Press [Enter] to go inside!";
					break;
			}
		});
	}
}