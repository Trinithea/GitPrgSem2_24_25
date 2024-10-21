using PokemonRipoff.Types;

namespace PokemonRipoff.Cells;

public class WaterCell() : Cell(CellType.Water) {
	public override Color Color => new(0, 0, 255);
}