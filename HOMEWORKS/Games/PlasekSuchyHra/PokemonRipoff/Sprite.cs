using PokemonRipoff.Types;

namespace PokemonRipoff;

public enum Anchor {
	TopLeft,
	Top,
	TopRight,
	CenterLeft,
	Center,
	CenterRight,
	BottomLeft,
	Bottom,
	BottomRight
}

/// <summary>
/// In the case of the map or any other pixel art, a sprite consists of an "Upper Half Block" character and renders two "pixels" - top defined by FgColor and bottom by BgColor
/// </summary>
/// <param name="spriteChar">The inner character</param>
public struct Sprite(char spriteChar) {
	public char Char { get; set; } = spriteChar;
	public bool Bold = false;
	public bool Underline = false;
	public Color? FgColor { get; private set; }
	public Color? BgColor { get; private set; }

	public Sprite WithBold() {
		Bold = true;
		return this;
	}

	public Sprite WithUnderline() {
		Underline = true;
		return this;
	}
	
	public Sprite WithFgColor(Color fgColor) {
		FgColor = fgColor;
		return this;
	}

	public Sprite WithBgColor(Color bgColor) {
		BgColor = bgColor;
		return this;
	}

	public override string ToString() {
		return Char.ToString();
	}
}