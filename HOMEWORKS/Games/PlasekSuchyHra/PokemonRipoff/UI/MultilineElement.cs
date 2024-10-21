namespace PokemonRipoff.UI;

public class MultilineElement(string id, string[] text, Anchor anchor = Anchor.Center, int offset = 0)
	: UIElement(id, anchor, offset) {
	public string[] Text { get; set; } = text;

	public override Sprite[,] Render() {
		Sprite[,] result = new Sprite[Text.Length, Text.Aggregate(string.Empty, (seed, f) => f.Length > seed.Length ? f : seed).Length];

		for (int i = 0; i < result.GetLength(0); i++) {
			char[] currLine = Text[i].ToCharArray();
			for (int j = 0; j < result.GetLength(1); j++) {
				result[i,j] = new Sprite(currLine[j]);
			}
		}
		
		return result;
	}
}