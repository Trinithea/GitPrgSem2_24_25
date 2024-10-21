using System.Text;
using PokemonRipoff.Pokeballs;
using PokemonRipoff.UI;

namespace PokemonRipoff.Scenes;

public class ShopScene : Scene {
	private Pokeballs.Pokeball[]? _pokeballs;
	private Items.Item[]? _items;
	private int _selectedIndex = 0;

	public ShopScene(string id) : base(id) {
		int[] pokeballIds = Pokeball.GetAllIds();
		List<Pokeball> pokeballs = new List<Pokeball>();
		for (int i = 0; i < pokeballIds.Length; i++) {
			pokeballs.Add(Pokeball.FromId(pokeballIds[i]));
		}

		_pokeballs = pokeballs.ToArray();
		_items = new Items.Item[0];

		RegisterPreRenderHook((renderer, game) => {
			int lineIndex = 0;
			renderer.Buffer(new TextElement("shopTitle", "Shop", TextDecoration.Underline, Anchor.Top, lineIndex));
			lineIndex++;
			renderer.Buffer(new TextElement("pokeballsTitle", "Poké balls", TextDecoration.Underline, Anchor.TopLeft,
				lineIndex));
			lineIndex++;

			for (int i = 0; i < _pokeballs.Length; i++) {
				StringBuilder sb = new StringBuilder();
				if (_selectedIndex == i) {
					sb.Append("> ");
				}

				sb.Append(_pokeballs[i].DisplayName);
				sb.Append(" - cost: ");
				sb.Append(_pokeballs[i].Cost);
				sb.Append(" (you own: ");

				if (game.State.Pokeballs.TryGetValue(pokeballIds[i], out int pokeballCount)) {
					sb.Append(pokeballCount);
				} else {
					sb.Append("0");
				}

				sb.Append(")");
				renderer.Buffer(new TextElement("pokeball-" + pokeballIds[i], sb.ToString(), TextDecoration.None,
					Anchor.TopLeft, lineIndex));
				lineIndex++;
			}

			lineIndex++;

			renderer.Buffer(new TextElement("itemsTitle", "Items", TextDecoration.Underline, Anchor.TopLeft, lineIndex));

			renderer.Buffer(new TextElement("money", "Money - " + game.State.PlayerMoney, TextDecoration.None, Anchor.BottomLeft, -1));
			renderer.Buffer(new TextElement("shopHelp",
				$"Press [{ConsoleKey.Enter}] to buy, [{ConsoleKey.Escape.ToString()}] to go back to map",
				TextDecoration.None,
				Anchor.BottomLeft
			));
		});

		RegisterKeyEventHook((info, game) => {
			switch (info.Key) {
				case ConsoleKey.S:
					if (_selectedIndex < _pokeballs.Length + _items.Length - 1) {
						_selectedIndex++;
					}

					break;

				case ConsoleKey.W:
					if (_selectedIndex > 0) {
						_selectedIndex--;
					}

					break;

				case ConsoleKey.Enter:
					if (_selectedIndex < _pokeballs.Length) {
						if (game.State.PlayerMoney >= _pokeballs[_selectedIndex].Cost) {
							if (game.State.Pokeballs.ContainsKey(_pokeballs[_selectedIndex].Id)) {
								game.State.Pokeballs[_pokeballs[_selectedIndex].Id] += 1;
							} else {
								game.State.Pokeballs.Add(_pokeballs[_selectedIndex].Id, 1);
							}

							game.State.PlayerMoney -= _pokeballs[_selectedIndex].Cost;
						}
					}

					break;

				case ConsoleKey.Escape:
					game.ActivateScene("main");
					break;
			}
		});
	}
}