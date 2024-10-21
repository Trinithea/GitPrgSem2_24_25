using PokemonRipoff.Types;

namespace PokemonRipoff.Cells;

public class NoneCell() : Cell(CellType.None) {
	public override Color Color => new Color(40, 40, 40);
}