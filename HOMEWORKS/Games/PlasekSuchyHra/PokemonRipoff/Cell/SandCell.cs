using PokemonRipoff.Types;

namespace PokemonRipoff.Cells;

public class SandCell() : Cell(CellType.Sand) {
	public override Color Color => new(253, 253, 150);
}