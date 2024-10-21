namespace PokemonRipoff.UI;

public class InputElement(string id, int length, Anchor anchor = Anchor.Center, int offset = 0)
	: UIElement(id, anchor, offset) {
	public int Length { get; set; } = length;
	public LinkedList<char> Input { get; set; } = new LinkedList<char>();

	public override Sprite[,] Render() {
		Sprite[,] result = new Sprite[1, Length];
		for (int i = 0; i < Length; i++) {
			if (i < Input.Count) {
				result[0, i] = new Sprite(Input.ElementAt(i));
			} else {
				result[0, i] = new Sprite('_');
			}
		}

		return result;
	}
}