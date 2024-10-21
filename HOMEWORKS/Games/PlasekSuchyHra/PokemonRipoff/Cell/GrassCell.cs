using PokemonRipoff.Types;

namespace PokemonRipoff.Cells;

public class GrassCell() : Cell(CellType.Grass) {
	public override Color Color => new(0, 220, 0);
}