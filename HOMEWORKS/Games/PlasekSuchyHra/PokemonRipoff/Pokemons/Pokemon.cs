namespace PokemonRipoff.Pokemons;


public abstract class Pokemon(int id, string displayName, HashSet<PType> types, int health, int attack, int defense, int catchRate) {
	public int Id { get; } = id;
	public string DisplayName { get; } = displayName;
	public HashSet<PType> Type { get; } = types;
	public int Experience { get; protected set; } = 0;
	public int Level { get; private set; } = 1;
	public int Health { get; set; } = health;
	public int MaxHealth { get; } = health;
	public int Attack { get; set; } = attack;
	public int MaxAttack { get; } = attack;
	public int Defense { get; set; } = defense;
	public int MaxDefense { get; } = defense;
	public int CatchRate { get; set; } = catchRate;
	
	public abstract List<Moves.Move> Moves { get; }
	
	public abstract Sprite[,] Sprite { get; }

	public void RecalcLevel() {
		Level = (int) Math.Floor(Math.Pow(Experience, 3.0));
	}

	public void ResetAfterFight() {
		Attack = MaxAttack;
		Defense = MaxDefense;
	}

	public abstract Moves.Move CalcMove(Pokemon _opponent);

	public static int[] GetAllIds() {
		return [0, 1, 2, 3, 4];
	}

	public static Pokemon FromId(int id) {
		switch (id) {
			case 0:
				return new Bulbasaur();
			case 1:
				return new Charmander();
			case 2:
				return new Squirtle();
			case 3:
				return new Pikachu();
			case 4:
				return new Caterpie();
			default:
				throw new ArgumentOutOfRangeException(nameof(id), id, null);
		}
	}
}