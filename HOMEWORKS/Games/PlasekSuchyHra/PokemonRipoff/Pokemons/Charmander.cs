using PokemonRipoff.Pokemons.Moves;

namespace PokemonRipoff.Pokemons;

public class Charmander : Pokemon {
	public Charmander(string displayName = "Charmander") : base(1, displayName, new HashSet<PType>([PType.Fire]), 18, 52, 43, 45) {
	}

	public override List<Move> Moves {
		get
		{
			List<Move> result = new List<Move>();
			result.Add(new Growl());
			result.Add(new Scratch());
			return result;
		}
	}
	
	public override Move CalcMove(Pokemon _opponent) {
		if (_opponent.Defense > _opponent.MaxDefense - 2) {
			Random rnd = new Random();
			if (rnd.Next(0, 2) == 0) {
				return new Growl();
			} else {
				return new Scratch();
			}
		} else {
			return new Scratch();
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