using PokemonRipoff.Pokemons.Moves;

namespace PokemonRipoff.Pokemons;

public class Bulbasaur : Pokemon {
	public Bulbasaur(string displayName = "Bulbasaur") : base(0, displayName, new HashSet<PType>([PType.Grass, PType.Poison]), 22, 49, 49, 45) {
	}

	public override List<Move> Moves {
		get {
			List<Move> result = new List<Move>();
			result.Add(new Growl());
			result.Add(new Tackle());
			return result;
		}
	}
	
	public override Move CalcMove(Pokemon _opponent) {
		if (_opponent.Defense > _opponent.MaxDefense - 2) {
			Random rnd = new Random();
			if (rnd.Next(0, 2) == 0) {
				return new Growl();
			} else {
				return new Tackle();
			}
		} else {
			return new Tackle();
		}
	}

	public override Sprite[,] Sprite {
		get {
			return SpriteManager.PokemonSprites.TryGetValue(0, out Sprite[,]? result) ? result : new Sprite[0,0];
		}
	}
}