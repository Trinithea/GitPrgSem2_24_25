namespace PokemonRipoff;

public abstract class Formatter {
	public string Bold() {
		return "\x1B[1m";
	}

	public string Bold(string input) {
		return Bold() + input;
	}

	public string Underline() {
		return "\x1B[4m";
	}

	public string Underline(string input) {
		return Underline() + input;
	}
	
	public string FgColor(int code) {
		return "\x1B[38;5;" + code + "m";
	}

	public string FgColor(string input, int code) {
		return FgColor(code) + input;
	}

	public virtual string FgColor(int r, int g, int b) {
		if (r < 0 || r > 255 || g < 0 || g > 255 || b < 0 || b > 255) {
			throw new Exception("Incorrect RGB value");
		}
		return "\x1B[38;2;" + r + ";" + g + ";" + b + "m";
	}

	public virtual string FgColor(string input, int r, int g, int b) {
		return FgColor(r, g, b) + input;
	}

	public string BgColor(int code) {
		return "\x1B[48;5;" + code + "m";
	}

	public string BgColor(string input, int code) {
		return BgColor(code) + input;
	}

	public virtual string BgColor(int r, int g, int b) {
		if (r < 0 || r > 255 || g < 0 || g > 255 || b < 0 || b > 255) {
			throw new Exception("Incorrect RGB value");
		}
		return "\x1B[48;2;" + r + ";" + g + ";" + b + "m";
	}

	public virtual string BgColor(string input, int r, int g, int b) {
		return BgColor(r, g, b) + input;
	}
	
	public string Reset() {
		return "\x1B[0m";
	}

	public string Reset(string input) {
		return input + Reset();
	}
}

public class LegacyFormatter : Formatter {
	private int ConvertColor(int colorChannel) {
		return (int)Math.Round((double)(colorChannel * 5) / 255, 0);
	}
	
	/// <summary>
	/// If the current 
	/// </summary>
	/// <param name="r"></param>
	/// <param name="g"></param>
	/// <param name="b"></param>
	/// <returns></returns>
	public override string FgColor(int r, int g, int b) {
		if (r < 0 || r > 255 || g < 0 || g > 255 || b < 0 || b > 255) {
			throw new Exception("Incorrect RGB value");
		}
		return "\x1B[38;2;" + (16 + 36 * ConvertColor(r) + 6 * ConvertColor(g) + ConvertColor(b)) + "m";
	}
}

public class RGBFormatter : Formatter;
