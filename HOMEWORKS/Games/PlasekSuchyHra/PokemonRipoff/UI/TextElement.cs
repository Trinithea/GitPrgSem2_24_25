namespace PokemonRipoff.UI;

[Flags]
public enum TextDecoration {
	None = 0,
	Bold = 1,
	Underline = 2,
}

public class TextElement(string id, string text, TextDecoration decoration = TextDecoration.None, Anchor anchor = Anchor.Center, int offset = 0)
	: UIElement(id, anchor, offset) {
	public char[] Text { get; set; } = text.ToCharArray();

	public override Sprite[,] Render() {
		Sprite[,] result = new Sprite[1, Text.Length];

		for (int i = 0; i < Text.Length; i++) {
			result[0, i] = new Sprite(Text[i]);

			if ((decoration & TextDecoration.Bold) == TextDecoration.Bold) {
				result[0, i] = result[0, i].WithBold();
			}

			if ((decoration & TextDecoration.Underline) == TextDecoration.Underline) {
				result[0, i] = result[0, i].WithUnderline();
			}
		}

		return result;
	}
}