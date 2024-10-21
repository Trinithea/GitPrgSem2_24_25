using PokemonRipoff.Pokemons.Moves;

namespace PokemonRipoff.Pokemons;

public class Caterpie(string displayName = "Caterpie") : Pokemon(4, displayName, new HashSet<PType>([PType.Bug]), 20, 30, 35, 255) {
	public override List<Moves.Move> Moves
	{
		get
		{
			List<Move> result = new List<Move>();
			result.Add(new Tackle());
			return result;
		}
	}

	public override Move CalcMove(Pokemon _opponent)
	{
		return new Tackle();
	}

	public override Sprite[,] Sprite
	{
		get
		{
			return new Sprite[,] {
				{
					new Sprite('\u2580')
				}
			};
		}
	}
}