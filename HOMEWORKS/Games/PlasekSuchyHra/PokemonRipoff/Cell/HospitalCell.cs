using PokemonRipoff.Types;

namespace PokemonRipoff.Cells;

public class HospitalCell() : Cell(CellType.Hospital) {
	public override Color Color => new Color(220, 0, 0);
}