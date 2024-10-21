using System.Text;
using PokemonRipoff.Types;

namespace PokemonRipoff;

public interface IRenderable {
	/// <summary>
	///  Where to affix the rendered string to
	/// </summary>
	public Anchor Anchor { get; set; }

	/// <summary>
	///  Line offset of the rendered string - e.g.: 1 will render the string one line below the specified anchor 
	/// </summary>
	public int Offset { get; set; }

	Sprite[,] Render();
}

public class Renderer {
	private int width = Console.BufferWidth;
	private int height = Console.BufferHeight;
	private Sprite[,] _buffer = new Sprite[Console.WindowHeight, Console.WindowWidth];
	private Formatter _formatter;

	public Renderer(Formatter formatter) {
		_formatter = formatter;
		Reset();
	}

	private void Reset() {
		width = Console.WindowWidth;
		height = Console.WindowHeight;
		_buffer = new Sprite[height, width];
	}

	private int CalcCenterLeftPad(Sprite[,] renderedSprites) {
		return (width - renderedSprites.GetLength(1)) / 2;
	}

	private int CalcRightLeftPad(Sprite[,] renderedSprites) {
		return width - renderedSprites.GetLength(1);
	}

	private int CalcCenterOffset(Sprite[,] renderedSprites) {
		return (height + 1) / 2 - (renderedSprites.GetLength(0) - 1) / 2;
	}

	private int CalcBottomOffset(Sprite[,] renderedSprites) {
		return height - renderedSprites.GetLength(0);
	}

	public void Buffer(IRenderable renderable) {
		Sprite[,] renderedSprites = renderable.Render();
		int currOffset = renderable.Offset;
		int padLeft = 0;
		switch (renderable.Anchor) {
			case Anchor.Top:
				padLeft = CalcCenterLeftPad(renderedSprites);
				break;

			case Anchor.TopRight:
				padLeft = CalcRightLeftPad(renderedSprites);
				break;

			case Anchor.CenterLeft:
				currOffset += CalcCenterOffset(renderedSprites);
				break;

			case Anchor.Center:
				currOffset += CalcCenterOffset(renderedSprites);
				padLeft = CalcCenterLeftPad(renderedSprites);
				break;

			case Anchor.CenterRight:
				currOffset += CalcCenterOffset(renderedSprites);
				padLeft = CalcRightLeftPad(renderedSprites);
				break;

			case Anchor.BottomLeft:
				currOffset += CalcBottomOffset(renderedSprites);
				break;

			case Anchor.Bottom:
				currOffset += CalcBottomOffset(renderedSprites);
				padLeft += CalcBottomOffset(renderedSprites);
				break;

			case Anchor.BottomRight:
				currOffset += CalcBottomOffset(renderedSprites);
				padLeft = CalcRightLeftPad(renderedSprites);
				break;
		}

		for (int i = 0; i < renderedSprites.GetLength(0); i++) {
			for (int j = 0; j < padLeft; j++) {
				_buffer[currOffset + i, j] = new Sprite(' ');
			}

			for (int j = 0; j < renderedSprites.GetLength(1); j++) {
				_buffer[currOffset + i, padLeft + j] = renderedSprites[i, j];
			}
		}
	}

	public void Render() {
		Console.Clear();
		StringBuilder sb = new StringBuilder(String.Empty);

		for (int i = 0; i < _buffer.GetLength(0); i++) {
			for (int j = 0; j < _buffer.GetLength(1); j++) {
				string currChar = _buffer[i, j].Char.ToString();
				if (_buffer[i, j].Bold) {
					currChar = _formatter.Bold(currChar);
				}

				if (_buffer[i, j].Underline) {
					currChar = _formatter.Underline(currChar);
				}
				
				if (_buffer[i, j].FgColor != null) {
					currChar = _formatter.FgColor(currChar, ((Color)_buffer[i, j].FgColor!).R,
						((Color)_buffer[i, j].FgColor!).G, ((Color)_buffer[i, j].FgColor!).B);
				}

				if (_buffer[i, j].BgColor != null) {
					currChar = _formatter.BgColor(currChar, ((Color)_buffer[i, j].BgColor!).R,
						((Color)_buffer[i, j].BgColor!).G, ((Color)_buffer[i, j].BgColor!).B);
				}

				sb.Append(_formatter.Reset(currChar));
			}

			if (i != _buffer.GetLength(0) - 1) {
				sb.AppendLine();
			}
		}

		Console.Write(sb.ToString());
		Reset();
	}
}