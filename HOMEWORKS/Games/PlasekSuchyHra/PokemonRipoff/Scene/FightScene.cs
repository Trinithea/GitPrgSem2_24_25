using PokemonRipoff.Pokeballs;
using PokemonRipoff.UI;
using PokemonRipoff.Pokemons;

namespace PokemonRipoff.Scenes;

enum State {
	PlayerAttacking,
	PlayerPostAttack,
	OpponentAttacking,
	OpponentPostAttack,
	CatchSelectBall,
	CatchTrying,
}

enum Menu {
	Root,
	Moves,
	Items
}

public class FightScene : Scene {
	private Pokemon _opponent;
	private List<Pokemons.Moves.Move>? _playerMoves;
	private State _state = State.PlayerAttacking;
	private Menu _currMenu = Menu.Root;
	private Pokeball? _selectedPokeball = null;
	private string _message = "";

	private ConsoleKey BackKey = ConsoleKey.Escape;

	private void BufferMainDisplay(Renderer renderer, Pokemon player, bool printPlayer = true) {
		SpriteElement opponentSprite = new SpriteElement("opponentSprite", _opponent.Sprite);
		TextElement opponentName = new TextElement("opponentName", _opponent.DisplayName,
			_state == State.OpponentAttacking || _state == State.OpponentPostAttack
				? TextDecoration.Underline
				: TextDecoration.None);
		VerticalUIGroup opponentGroup = new VerticalUIGroup("opponent", 1);
		opponentGroup.AddChild(opponentSprite);
		opponentGroup.AddChild(opponentName);

		if (printPlayer) {
			renderer.Buffer(
				new TextElement("playerHealth", "Health: " + player.Health, TextDecoration.None, Anchor.TopLeft));

			SpriteElement playerSprite = new SpriteElement("playerSprite", player.Sprite);
			TextElement playerName = new TextElement("playerName", player.DisplayName + " (You)",
				_state == State.PlayerAttacking || _state == State.PlayerPostAttack
					? TextDecoration.Underline
					: TextDecoration.None);
			VerticalUIGroup playerGroup = new VerticalUIGroup("player", 1);
			playerGroup.AddChild(playerSprite);
			playerGroup.AddChild(playerName);

			HorizontalUIGroup mainFight = new HorizontalUIGroup("pokemons", 9);
			mainFight.AddChild(playerGroup);
			mainFight.AddChild(opponentGroup);

			renderer.Buffer(mainFight);
		} else {
			renderer.Buffer(opponentGroup);
		}
	}

	public FightScene(string id, Pokemon opponent) : base(id) {
		_opponent = opponent;

		RegisterPreRenderHook((renderer, game) => {
			game.State.Pokemons.TryGetValue(game.State.ActivePokemonId, out Pokemon? player);
			if (player == null) {
				throw new Exception("No pokemon selected for player");
			}

			if (_playerMoves == null) {
				_playerMoves = player.Moves; // can be an expensive operation, we want to cache this
			}

			int messageOffset = 0;
			if (_message != string.Empty) {
				renderer.Buffer(new TextElement("messageBorder", "---", TextDecoration.None, Anchor.BottomLeft,
					-1));
				renderer.Buffer(new TextElement("message", _message, TextDecoration.None,
					Anchor.BottomLeft));
				messageOffset = -2;
			}
			
			switch (_state) {
				case State.PlayerAttacking:
				{
					BufferMainDisplay(renderer, player);
					switch (_currMenu) {
						case Menu.Root:
							renderer.Buffer(new TextElement("root-title", "What would you like to do? [Press key]",
								TextDecoration.None, Anchor.BottomLeft, -3 + messageOffset));
							renderer.Buffer(new TextElement("root-option-1", "[1] Move", TextDecoration.None,
								Anchor.BottomLeft, -2 + messageOffset));
							renderer.Buffer(new TextElement("root-option-2", "[2] Item", TextDecoration.None,
								Anchor.BottomLeft, -1 + messageOffset));
							renderer.Buffer(new TextElement("root-option-2", "[3] Try to run", TextDecoration.None,
								Anchor.BottomLeft, messageOffset));
							break;

						case Menu.Moves:
							renderer.Buffer(new TextElement("moves-title", "Choose a move: [Press key]",
								TextDecoration.None, Anchor.BottomLeft,
								_playerMoves.Count * -1 - 1 + messageOffset));

							for (int i = 0; i < _playerMoves.Count; i++) {
								renderer.Buffer(new TextElement("moves-" + i,
									$"[{i + 1}] " + _playerMoves[i].DisplayName,
									TextDecoration.None,
									Anchor.BottomLeft, _playerMoves.Count * -1 + i + messageOffset));
							}

							renderer.Buffer(new TextElement("moves-back", $"[{BackKey.ToString()}] Go back",
								TextDecoration.None,
								Anchor.BottomLeft, messageOffset));
							break;

						case Menu.Items:
							renderer.Buffer(new TextElement("items-title", "Choose an item: [Press key]",
								TextDecoration.None, Anchor.BottomLeft,
								messageOffset - 1));
							renderer.Buffer(new TextElement("items-back", $"[{BackKey.ToString()}] Go back",
								TextDecoration.None, Anchor.BottomLeft, messageOffset));
							break;
					}
				}
					break;

				case State.PlayerPostAttack:
				{
					BufferMainDisplay(renderer, player);
					renderer.Buffer(new TextElement("pressAnyKey", "[Press any key]", TextDecoration.None,
						Anchor.BottomLeft, messageOffset));
				}
					break;

				case State.OpponentAttacking:
				{
					BufferMainDisplay(renderer, player);
					renderer.Buffer(new TextElement("opponent-attacking",
						$"{_opponent.DisplayName} is attacking... [Press any key]", TextDecoration.None,
						Anchor.BottomLeft, messageOffset));
				}
					break;

				case State.OpponentPostAttack:
				{
					BufferMainDisplay(renderer, player);
					renderer.Buffer(new TextElement("pressAnyKey", "[Press any key]", TextDecoration.None,
						Anchor.BottomLeft, messageOffset));
				}
					break;

				case State.CatchSelectBall:
				{
					BufferMainDisplay(renderer, player, false);

					var pokeballsSorted = game.State.Pokeballs.OrderBy(kvp => kvp.Key).ToList();

					renderer.Buffer(new TextElement("pokeball-select-title",
						"Select which pokéball to use... [Press key]", TextDecoration.None,
						Anchor.BottomLeft, pokeballsSorted.Count * -1 - 1 + messageOffset));

					for (int i = 0; i < pokeballsSorted.Count; i++) {
						Pokeball currPokeball = Pokeball.FromId(pokeballsSorted[i].Key);
						renderer.Buffer(new TextElement("pokeball-select-" + i,
							$"[{i + 1}] " + currPokeball.DisplayName + $" ({pokeballsSorted[i].Value})",
							TextDecoration.None, Anchor.BottomLeft, pokeballsSorted.Count * -1 + i + messageOffset));
					}

					renderer.Buffer(new TextElement("moves-run", $"[{BackKey.ToString()}] Try to run",
						TextDecoration.None,
						Anchor.BottomLeft, messageOffset));
				}
					break;

				case State.CatchTrying:
				{
					BufferMainDisplay(renderer, player, false);
					if (_selectedPokeball != null) {
						renderer.Buffer(new TextElement("catchTrying",
							$"Selected {_selectedPokeball.DisplayName}, trying to catch...", TextDecoration.None,
							Anchor.BottomLeft, messageOffset));
					}
				}
					break;
			}
		});

		RegisterKeyEventHook((keyInfo, game) => {
			switch (_state) {
				case State.PlayerAttacking:
				{
					switch (_currMenu) {
						case Menu.Root:
							switch (keyInfo.Key) {
								case ConsoleKey.D1:
								case ConsoleKey.NumPad1:
								case ConsoleKey.Oem1:
									_currMenu = Menu.Moves;
									break;

								case ConsoleKey.D2:
								case ConsoleKey.NumPad2:
								case ConsoleKey.Oem2:
									_currMenu = Menu.Items;
									break;

								case ConsoleKey.D3:
								case ConsoleKey.NumPad3:
								case ConsoleKey.Oem3:
									Random rnd = new Random();
									// 30% chance of running
									if (rnd.Next(0, 10) < Config.RUN_CHANCE * 10) {
										game.RemoveScene("encounter");
										game.ActivateScene("main", "You succesfully ran from the encounter!");
									} else {
										_message = "That didn't work...";
										_state = State.OpponentAttacking;
									}

									break;
							}

							break;

						case Menu.Moves:
							if (keyInfo.Key == BackKey) {
								_currMenu = Menu.Root;
							}

							if (Int32.TryParse(keyInfo.KeyChar.ToString(), out int number)) {
								if (number > 0 && number < _playerMoves?.Count + 1) {
									game.State.Pokemons.TryGetValue(game.State.ActivePokemonId,
										out Pokemon? player);
									if (player == null) {
										throw new Exception("No pokemon selected for player");
									}

									_message = _playerMoves[number - 1].Use(game.State, player, _opponent);
									_currMenu = Menu.Root;
									_state = State.PlayerPostAttack;
								}
							}

							break;

						case Menu.Items:
							if (keyInfo.Key == BackKey) {
								_currMenu = Menu.Root;
							}

							break;
					}
				}
					break;

				case State.PlayerPostAttack:
				{
					_message = "";
					if (_opponent.Health <= 0) {
						_state = State.CatchSelectBall;
					} else {
						_state = State.OpponentAttacking;
					}
				}
					break;

				case State.OpponentAttacking:
				{
					game.State.Pokemons.TryGetValue(game.State.ActivePokemonId,
						out Pokemon? player);
					if (player == null) {
						throw new Exception("No pokemon selected for player");
					}

					Pokemons.Moves.Move selectedMove = _opponent.CalcMove(player);
					_message = selectedMove.Use(game.State, _opponent, player);
					_state = State.OpponentPostAttack;
				}
					break;

				case State.OpponentPostAttack:
				{
					if (!game.State.Pokemons.TryGetValue(game.State.ActivePokemonId,
						    out Pokemon? player)) {
						throw new Exception("No pokemon selected for player");
					}

					_message = "";
					if (player.Health <= 0) {
						game.RemoveScene("encounter");
						game.ActivateScene("main",
							"Your pokemon passed out from the long fight, you have to heal it at the hospital!");
					} else {
						_state = State.PlayerAttacking;
					}
				}
					break;

				case State.CatchSelectBall:
				{
					if (game.State.Pokeballs.Values.Sum() < 0) {
						game.State.PlayerMoney += 20;
						game.RemoveScene("encounter");
						game.ActivateScene("main", $"You ran out of pokeballs and the pokemon ran away! [+20 money]");
					}

					var pokeballsSorted = game.State.Pokeballs.OrderBy(kvp => kvp.Key).ToList();
					if (Int32.TryParse(keyInfo.KeyChar.ToString(), out int number)) {
						if (number > 0 && number < pokeballsSorted?.Count + 1) {
							game.State.Pokemons.TryGetValue(game.State.ActivePokemonId,
								out Pokemon? player);
							if (player == null) {
								throw new Exception("No pokemon selected for player");
							}

							int selectedPokeballId = pokeballsSorted[number - 1].Key;

							// We have to have some pokeball of that type!
							if (game.State.Pokeballs[selectedPokeballId] > 0) {
								_selectedPokeball = Pokeball.FromId(selectedPokeballId);
								game.State.Pokeballs[selectedPokeballId] -= 1;
								_message = $"Selected {_selectedPokeball.DisplayName}, trying to catch...";
								_state = State.CatchTrying;
							} else {
								_message = "No pokeball of that type left, select another one!";
							}
						}
					}
				}
					break;

				case State.CatchTrying:
				{
					if (_selectedPokeball != null && _selectedPokeball.Catch(_opponent)) {
						game.State.PlayerMoney += 100;
						game.RemoveScene("encounter");
						if (!game.State.Pokemons.ContainsKey(_opponent.Id)) {
							game.State.Pokemons.Add(_opponent.Id, _opponent);
							game.ActivateScene("main",
								$"You caught {_opponent.DisplayName}! (But it's severly damaged, you should definitely get it healed!) [+100 money]");
						} else {
							game.ActivateScene("main", $"You defeated {_opponent.DisplayName} [+100 money]!");
						}
					} else {
						_message = "That didn't work, try again...";
						_state = State.CatchSelectBall;
					}
				}
					break;
			}
		});
	}
}