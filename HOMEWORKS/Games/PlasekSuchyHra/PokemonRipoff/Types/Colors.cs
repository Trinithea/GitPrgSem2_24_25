namespace PokemonRipoff.Types;

public struct Color(int r, int g, int b) {
	public int R { get; set; } = r;
	public int G { get; set; } = g;
	public int B { get; set; } = b;
}