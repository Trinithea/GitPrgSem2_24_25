using PokemonRipoff.UI;

namespace PokemonRipoff.Scenes;

public class IntroScene : Scene {
	private int _state = 0;

	public IntroScene() : base("intro") {
		string[] lines = [
			"You wake up, barely remembering the events of last night...",
			"Your friends warned you about drinking too much, but you never listened to them...",
			"And as you curse yourself for again making a fool out of yourself...",
			"...you suddenly feel a light breeze making its way through the... grass... around you?",
			"You stand up and try to figure out where your legs took you last night, when suddenly...",
			"\"Oh no, not this again...\", you think to yourself and realise...",
			"You're stuck in a very bad pokemon game that the authors didn't have that much time to spend on...",
			".",
			".",
			"This has happened to you before, somehow"
		];

		AddUIElement(new TextElement("helper", $"Press [{ConsoleKey.Enter}] to continue", TextDecoration.None, Anchor.BottomLeft));

		RegisterPreRenderHook((renderer, _) => {
			for (int i = 0; i < _state + 1; i++) {
				renderer.Buffer(new TextElement(i.ToString(), lines[i], TextDecoration.None, Anchor.TopLeft, i));
			}
		});

		RegisterKeyEventHook((_, game) => {
			if (_state < lines.Length - 1) {
				_state++;
			} else {
				game.ActivateScene("main");
			}
		});
	}
}