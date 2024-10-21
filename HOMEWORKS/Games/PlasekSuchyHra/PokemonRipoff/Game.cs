using PokemonRipoff.Scenes;
using PokemonRipoff.Pokemons;
using PokemonRipoff.Types;

namespace PokemonRipoff;

public class Game(Renderer renderer) {
	public GameState State { get; set; } = new GameState();
	private Dictionary<string, Scene> _scenes = new Dictionary<string, Scene>();
	private Scene? _currScene;
	private Renderer _renderer = renderer;

	public Game AddScene(Scene scene) {
		_scenes.Add(scene.Id, scene);
		return this;
	}

	public Game RemoveScene(string sceneId) {
		if (_scenes.ContainsKey(sceneId)) {
			_scenes.Remove(sceneId);
		}

		return this;
	}

	public bool ActivateScene(Scene scene) {
		if (_scenes.ContainsKey(scene.Id)) {
			_currScene = scene;
			return true;
		}

		return false;
	}

	public bool ActivateScene(string sceneId, string message = "") {
		if (_scenes.ContainsKey(sceneId)) {
			_currScene = _scenes[sceneId];
			State.Message = message;
			return true;
		}

		return false;
	}

	public Option<Pokemon> GetActivePokemon(string id) {
		if (State.Pokemons.TryGetValue(State.ActivePokemonId, out Pokemon? pokemon)) {
			if (pokemon == null) {
				return new None<Pokemon>();
			} else {
				return new Some<Pokemon>(pokemon);
			}
		}
		
		return new None<Pokemon>();
	}

	public void Run() {
		if (_currScene == null) {
			throw new Exception("Specify an active scene first please!");
		}

		while (true) {
			Scene? currScene = _currScene;
			currScene?.Render(_renderer, this);

			ConsoleKeyInfo keyInfo = Console.ReadKey(true);
			if (keyInfo.Key == ConsoleKey.OemComma) {
				break;
			}

			currScene?.KeyEventHook(keyInfo, this);
		}
	}
}

public class GameState {
	public string PlayerName { get; set; } = "";
	public int PlayerMoney { get; set; } = 100;
	public Dictionary<int, Pokemon> Pokemons { get; }
	public int ActivePokemonId { get; set; } = 0; // Pointer into Pokemons ^
	public Dictionary<int, int> Pokeballs { get; }
	public Dictionary<int, int> Items { get; }
	public string Message { get; set; } = "";
	public int MessageTimeout { get; set; } = 5;

	public GameState() {
		Pokemons = new Dictionary<int, Pokemon>();
		Pokemons.Add(0, new Bulbasaur());
		Pokemons.Add(1, new Charmander());
		
		Pokeballs = new Dictionary<int, int>();
		Pokeballs.Add(0, 5); // Five starting poké balls
	}
}