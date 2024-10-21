using PokemonRipoff.Pokemons.Moves;

namespace PokemonRipoff.Pokemons;

public class Pikachu(string displayName = "Pikachu")
	: Pokemon(3, displayName, new HashSet<PType>([PType.Electric]), 17, 55, 40, 190) {
	public override List<Move> Moves {
		get {
			List<Move> result = new List<Move>();
			result.Add(new TailWhip());
			result.Add(new Charm());
			result.Add(new Scratch());
			return result;
		}
	}

	public override Move CalcMove(Pokemon _opponent) {
		if (_opponent.Defense > _opponent.MaxDefense - 2) {
			Random rnd = new Random();
			if (rnd.Next(0, 2) == 0) {
				return new TailWhip();
			} else {
				return new Tackle();
			}
		} else {
			return new Tackle();
		}
	}

	public override Sprite[,] Sprite {
		get {
			return new Sprite[,] {
				{
					new Sprite('\u2580')
				}
			};
		}
	}
}