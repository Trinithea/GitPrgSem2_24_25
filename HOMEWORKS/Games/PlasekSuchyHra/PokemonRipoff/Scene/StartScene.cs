using System.Text;
using PokemonRipoff.UI;

namespace PokemonRipoff.Scenes;

public class StartScene : Scene {
	public StartScene() : base("start") {
		TextElement questionEl = new TextElement("question", "What's your name?");
		InputElement playerNameInputEl = new InputElement("playerName", 16, Anchor.Center, 2);
		AddUIElement(questionEl);
		AddUIElement(playerNameInputEl);
		
		RegisterKeyEventHook(delegate(ConsoleKeyInfo keyInfo, Game game) {
			Types.Option<InputElement> playerNameInputOpt = GetUIElement<InputElement>("playerName");
			if (playerNameInputOpt.IsSome()) {
				InputElement playerNameInput = playerNameInputOpt.Unwrap();
				if (keyInfo.Key == ConsoleKey.Enter && playerNameInput.Input.Count > 0) {
					StringBuilder collectedName = new StringBuilder();
					for (int i = 0; i < playerNameInput.Input.Count; i++) {
						collectedName.Append(playerNameInput.Input.ElementAt(i));
					}

					game.State.PlayerName = collectedName.ToString();
					game.ActivateScene("intro");
				} else if (keyInfo.Key == ConsoleKey.Backspace && playerNameInput.Input.Count > 0) {
					playerNameInput.Input.RemoveLast();
				} else if (Char.IsLetter(keyInfo.KeyChar) && playerNameInput.Input.Count < 16) {
					playerNameInput.Input.AddLast(keyInfo.KeyChar);
				}
			}
		});
	}
}