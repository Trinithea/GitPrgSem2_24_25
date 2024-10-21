namespace PokemonRipoff.UI;

public class SpriteElement(string id, Sprite[,] sprites, Anchor anchor = Anchor.Center, int offset = 0)
	: UIElement(id, anchor, offset) {
	public Sprite[,] Sprites { get; set; } = sprites;

	public override Sprite[,] Render() {
		return Sprites;
	}
}