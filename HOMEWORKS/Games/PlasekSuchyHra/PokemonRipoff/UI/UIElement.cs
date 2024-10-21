namespace PokemonRipoff.UI;

// ReSharper disable once InconsistentNaming
public abstract class UIElement(string id, Anchor anchor, int offset) : IRenderable {
	public string Id { get; } = id;
	public Anchor Anchor { get; set; } = anchor;
	public int Offset { get; set; } = offset;

	public abstract Sprite[,] Render();
}