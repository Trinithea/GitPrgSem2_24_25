using System.Text.RegularExpressions;
using PokemonRipoff.Types;

namespace PokemonRipoff;

public static class SpriteManager {
	public static Dictionary<int, Sprite[,]> PokemonSprites = new Dictionary<int, Sprite[,]>();

	private static Color? ParseColor(int[] bytes) {
		if (bytes[3] == 0) {
			return null;
		}

		return new Color(bytes[0], bytes[1], bytes[2]);
	}

	public static void LoadPokemonSprites(string spriteDirectory) {
		string[] files = Directory.GetFiles(spriteDirectory);
		int lineOffset = 20 * 4;
		
		for (int f = 0; f < files.Length; f++) {
			Sprite[,] result = new Sprite[10, 20];
			
			byte[] bytes = File.ReadAllBytes(files[f]); // 20 * 20 * 4
			for (int i = 0; i < 20; i += 2) {
				for (int j = 0; j < 20; j++) {
					Color? currColorTop = ParseColor(new int[] {
						bytes[i * lineOffset + j * 4],
						bytes[i * lineOffset + j * 4 + 1],
						bytes[i * lineOffset + j * 4 + 2],
						bytes[i * lineOffset + j * 4 + 3]
					});
					
					Color? currColorBottom = ParseColor(new int[] {
						bytes[(i + 1) * lineOffset + j * 4],
						bytes[(i + 1) * lineOffset + j * 4 + 1],
						bytes[(i + 1) * lineOffset + j * 4 + 2],
						bytes[(i + 1) * lineOffset + j * 4 + 3]
					});

					if (currColorTop == null && currColorBottom == null) {
						result[i / 2, j] = new Sprite(' ');
						continue;
					}

					if (currColorTop == null && currColorBottom != null) {
						result[i / 2, j] = new Sprite('\u2584').WithFgColor(currColorBottom.Value);
						continue;
					}

					if (currColorTop != null && currColorBottom == null) {
						result[i / 2, j] = new Sprite('\u2580').WithFgColor(currColorTop.Value);
						continue;
					}
					
					result[i / 2, j] = new Sprite('\u2580').WithFgColor(currColorTop!.Value).WithBgColor(currColorBottom!.Value);
				}
			}

			Regex r = new Regex(@"Pokemon(\d+).sprite");
			Match m = r.Match(files[f]);
			PokemonSprites.Add(int.Parse(m.Groups[1].Captures[0].ToString()), result);
		}
	}

	public static void LoadSprites(string spriteDirectory) {
		LoadPokemonSprites(Path.Combine(spriteDirectory, "Pokemon"));
	}
}