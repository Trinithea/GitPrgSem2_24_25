namespace PokemonRipoff.Pokemons.Moves;

public abstract class Move(string displayName, PType type, int power, int accuracy) {
	public string DisplayName { get; } = displayName;
	public PType Type { get; } = type;
	public int Power { get; } = power;
	public int Accuracy { get; } = accuracy;

	/// <summary>
	/// Returns a message to be displayed to the user.
	/// Damage calculated via https://bulbapedia.bulbagarden.net/wiki/Damage
	/// </summary>
	/// <param name="gameState"></param>
	/// <returns></returns>
	public abstract string Use(GameState gameState, Pokemon player, Pokemon opponent);
}