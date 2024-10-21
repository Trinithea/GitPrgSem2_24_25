using PRCell = PokemonRipoff.Cells;

namespace PokemonRipoff.Map;

public static class MapParser {
	public static Map Parse(byte[] data) {
		int mapSize = data[0];

		Map result = new Map(mapSize);
		for (int i = 0; i < mapSize; i++) {
			for (int j = 0; j < mapSize; j++) {
				result.Cells[i, j] = PRCell.Cell.FromType((PRCell.CellType) data[i * mapSize + j + 1]);
			}
		}

		return result;
	}
}