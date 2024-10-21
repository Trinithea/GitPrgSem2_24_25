using PRPokemons = PokemonRipoff.Pokemons;

namespace PokemonRipoff.Pokeballs;

public abstract class Pokeball(int id, string displayName, float catchChanceMultiplier, int cost) {
	public int Id { get; }
	public string DisplayName { get; } = displayName;
	public float CatchChanceMultiplier { get; } = catchChanceMultiplier;
	public int Cost { get; } = cost;
	
	public bool Catch(PRPokemons.Pokemon pokemon) {
		Random rnd = new Random();
		return pokemon.CatchRate * CatchChanceMultiplier >= rnd.Next(0, 100);
	}


	public static int[] GetAllIds() {
		return [0, 1];
	}

	public static Pokeball FromId(int id) {
		switch (id) {
			case 0:
				return new BasicPokeball();
			case 1:
				return new GreatPokeball();
			default:
				throw new ArgumentOutOfRangeException(nameof(id), id, null);
		}
	}
}