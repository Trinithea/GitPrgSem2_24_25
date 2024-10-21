using PokemonRipoff.Types;

namespace PokemonRipoff.Map;

public class Map(int size) : IRenderable {
	public Anchor Anchor { get; set; } = Anchor.Center;
	public int Offset { get; set; }
	public int Layer { get; set; } = 999;
	public int Size { get; set; } = size;

	/// <summary>
	/// Cell[Y][X]
	/// </summary>
	public Cells.Cell[,] Cells { get; set; } = new Cells.Cell[size, size];

	private Types.Position _cameraPos = new(0, 0);

	public Types.Position CameraPos => _cameraPos;
	private const int CameraFov = 15;

	public static Map Load(byte[] data) {
		return MapParser.Parse(data);
	}

	public Cells.Cell GetCurrCell() {
		return Cells[CameraPos.Y, CameraPos.X];
	}

	/// <summary>
	/// Move the camera (player)
	/// </summary>
	/// <returns>The new camera (player) position</returns>
	public Types.Position Move(int xDiff, int yDiff) {
		if (_cameraPos.X + xDiff < 0 || _cameraPos.X + xDiff > Size - 1 || _cameraPos.Y + yDiff < 0 ||
		    _cameraPos.Y + yDiff > Size - 1) {
			return _cameraPos;
		}

		_cameraPos.X += xDiff;
		_cameraPos.Y += yDiff;
		return _cameraPos;
	}

	private Types.Option<Cells.Cell> GetCellFromRenderPos(int renderX, int renderY) {
		Types.Position renderWorldPos =
			new Types.Position(_cameraPos.X - CameraFov / 2 + renderX, _cameraPos.Y - CameraFov / 2 + renderY);
		if (renderWorldPos.X < 0 || renderWorldPos.X > Size - 1 || renderWorldPos.Y < 0 ||
		    renderWorldPos.Y > Size - 1) {
			return new Types.None<Cells.Cell>();
		}

		return new Types.Some<Cells.Cell>(Cells[renderWorldPos.Y, renderWorldPos.X]);
	}

	private static Color CellToColor(Types.Option<Cells.Cell> cell) {
		if (!cell.IsSome()) {
			return new Color(20, 20, 20);
		}

		return cell.Unwrap().Color;
	}

	public Sprite[,] Render() {
		Sprite[,] result = new Sprite[(CameraFov + 3) / 2, CameraFov + 2];
		// Global line index - "pointer" into the result buffer, each line consists of two cells
		int lineIndex = 0;

		// Print the top border and top cells
		result[0, 0] = new Sprite('\u2580').WithFgColor(new Color(0, 0, 0)).WithBgColor(new Color(0, 0, 0));
		for (int j = 0; j < CameraFov; j++) {
			// j + 1 since the 0th column is the black border
			result[0, j + 1] = new Sprite('\u2580').WithFgColor(new Color(0, 0, 0))
				.WithBgColor(CellToColor(GetCellFromRenderPos(j, 0)));
		}

		lineIndex += 1;
		result[0, CameraFov + 1] = new Sprite('\u2580').WithFgColor(new Color(0, 0, 0)).WithBgColor(new Color(0, 0, 0));

		// Print the rest of the map
		// i is a "pointer" into the map (e.g.: when in a loop with i = 1, we are rendering the 1st and 2nd line, starting from 0)
		for (int i = 1; i < CameraFov - 1; i += 2) {
			result[lineIndex, 0] = new Sprite('\u2580').WithFgColor(new Color(0, 0, 0)).WithBgColor(new Color(0, 0, 0));
			// j is a "pointer" into the map, we are accesing the 0th column of the map but the 1st column of the render result buffer
			for (int j = 0; j < CameraFov; j++) {
				result[lineIndex, j + 1] =
					new Sprite('\u2580').WithFgColor(CellToColor(GetCellFromRenderPos(j, i)))
						.WithBgColor(CellToColor(GetCellFromRenderPos(j, i + 1)));
			}

			result[lineIndex, CameraFov + 1] =
				new Sprite('\u2580').WithFgColor(new Color(0, 0, 0)).WithBgColor(new Color(0, 0, 0));
			lineIndex++;
		}

		if (CameraFov % 2 == 0) {
			// If the camera fov is even, we have to render the last row of the map and the border
			result[lineIndex, 0] = new Sprite('\u2580').WithFgColor(new Color(0, 0, 0)).WithBgColor(new Color(0, 0, 0));
			for (int j = 0; j < CameraFov; j++) {
				result[lineIndex, j + 1] = new Sprite('\u2580')
					.WithFgColor(CellToColor(GetCellFromRenderPos(j, CameraFov - 1))).WithBgColor(new Color(0, 0, 0));
			}

			result[lineIndex, CameraFov + 1] =
				new Sprite('\u2580').WithFgColor(new Color(0, 0, 0)).WithBgColor(new Color(0, 0, 0));
		} else {
			// If the camera fov is odd, we have rendered everything, and we just have to render the border
			for (int j = 0; j < CameraFov + 2; j++) {
				result[result.GetLength(0) - 1, j] = new Sprite('\u2580').WithFgColor(new Color(0, 0, 0));
			}
		}

		// Print player (camera pos is in sync with the player, at least for now) with appropriate ground type
		if (CameraFov % 3 == 0) {
			result[CameraFov / 4 + 1, CameraFov / 2 + 1] =
				new Sprite('\u2580').WithFgColor(new Color(255, 0, 255))
					.WithBgColor(CellToColor(GetCellFromRenderPos(CameraFov / 2, CameraFov / 2 + 1))); 
		} else {
			result[CameraFov / 4, CameraFov / 2 + 1] = new Sprite('\u2580')
				.WithFgColor(CellToColor(GetCellFromRenderPos(CameraFov / 2, CameraFov / 2 - 1)))
				.WithBgColor(new Color(255, 0, 0));
		}

		return result;
	}
}