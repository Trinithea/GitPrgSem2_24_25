using PokemonRipoff.Pokemons.Moves;

namespace PokemonRipoff.Pokemons;

public class Squirtle : Pokemon {
	public Squirtle(string displayName = "Squirtle") : base(2, displayName, new HashSet<PType>([PType.Water]), 22, 48,
		65, 45) {
	}

	public override List<Move> Moves {
		get {
			List<Move> result = new List<Move>();
			result.Add(new TailWhip());
			result.Add(new Tackle());
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
			return SpriteManager.PokemonSprites.TryGetValue(2, out Sprite[,]? result) ? result : new Sprite[0, 0];
		}
	}
}