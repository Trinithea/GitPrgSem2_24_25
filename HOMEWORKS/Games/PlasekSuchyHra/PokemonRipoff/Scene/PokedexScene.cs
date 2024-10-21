using PokemonRipoff.UI;
using PokemonRipoff.Pokemons;

namespace PokemonRipoff.Scenes;

public class PokedexScene : Scene {
	private int _pokedexIndex = 0;
	private int[] _pokemonIds = Pokemon.GetAllIds();

	public PokedexScene() : base("pokedex") {
		AddUIElement(new TextElement("title", "Pokédex", TextDecoration.Underline, Anchor.Top));

		RegisterPreRenderHook((renderer, game) => {
			Pokemon currPokemon = Pokemon.FromId(_pokemonIds[_pokedexIndex]);
			Sprite[,] currPokemonSprite = currPokemon.Sprite;
			
			renderer.Buffer(new SpriteElement("pokemonSprite", currPokemonSprite, Anchor.TopRight, 1));

			int lineIndex = 1;

			renderer.Buffer(new TextElement("pokemonTitle", currPokemon.DisplayName, TextDecoration.Underline,
				Anchor.TopLeft, lineIndex));
			lineIndex++;
			renderer.Buffer(new TextElement("pokemonStatHP", "Health - " + currPokemon.Health, TextDecoration.None,
				Anchor.TopLeft, lineIndex));
			lineIndex++;
			renderer.Buffer(new TextElement("pokemonStatAttack", "Attack - " + currPokemon.Attack,
				TextDecoration.None, Anchor.TopLeft, lineIndex));
			lineIndex++;
			renderer.Buffer(new TextElement("pokemonStatDefense", "Defense - " + currPokemon.Defense,
				TextDecoration.None, Anchor.TopLeft, lineIndex));
			lineIndex += 2;
			renderer.Buffer(new TextElement("yourTitle", "Your pokemon:", TextDecoration.Underline, Anchor.TopLeft,
				lineIndex));
			lineIndex++;

			if (game.State.Pokemons.TryGetValue(_pokemonIds[_pokedexIndex],
				    out Pokemon? player)) {
				if (game.State.ActivePokemonId == _pokemonIds[_pokedexIndex]) {
					renderer.Buffer(new TextElement("active", "Is active!", TextDecoration.None, Anchor.TopLeft,
						lineIndex));
					lineIndex++;
				} else {
					renderer.Buffer(new TextElement("notActive", $"Isn't active, [{ConsoleKey.Enter}] to select", TextDecoration.None, Anchor.TopLeft, lineIndex));
					lineIndex++;
				}

				renderer.Buffer(new TextElement("health", "Health - " + player.Health, TextDecoration.None,
					Anchor.TopLeft, lineIndex));
				lineIndex++;
			} else {
				// We didn't catch that pokemon
				renderer.Buffer(new TextElement("didntCatch", "\u274c - You didn't catch this pokémon",
					TextDecoration.None, Anchor.TopLeft, lineIndex));
				lineIndex++;
			}
			
			renderer.Buffer(new TextElement("pokedex-back",
				$"Press [{ConsoleKey.Escape.ToString()}] to go back to map",
				TextDecoration.None,
				Anchor.BottomLeft));
		});

		RegisterKeyEventHook((keyInfo, game) => {
			switch (keyInfo.Key) {
				case ConsoleKey.D:
					if (_pokedexIndex < _pokemonIds.Length - 1) {
						_pokedexIndex++;
					}

					break;

				case ConsoleKey.A:
					if (_pokedexIndex > 0) {
						_pokedexIndex--;
					}

					break;
				
				case ConsoleKey.Enter:
					if (game.State.Pokemons.TryGetValue(_pokemonIds[_pokedexIndex], out Pokemon? player)) {
						game.State.ActivePokemonId = _pokemonIds[_pokedexIndex];
					}
					break;

				case ConsoleKey.Escape:
					game.ActivateScene("main");
					break;
			}
		});
	}
}