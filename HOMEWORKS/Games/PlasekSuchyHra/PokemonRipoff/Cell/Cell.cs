using PokemonRipoff.Types;

namespace PokemonRipoff.Cells;

public enum CellType {
	None,
	Grass,
	Sand,
	Water,
	Shop,
	Hospital
}

public abstract class Cell(CellType type) {
	public CellType Type { get; } = type;

	public abstract Color Color { get; }

	public static Cell FromType(CellType type) {
		switch (type) {
			case CellType.None:
				return new NoneCell();
			case CellType.Grass:
				return new GrassCell();
			case CellType.Sand:
				return new SandCell();
			case CellType.Water:
				return new WaterCell();
			case CellType.Shop:
				return new ShopCell();
			case CellType.Hospital:
				return new HospitalCell();
			default:
				return new NoneCell();
		}
	}
}
