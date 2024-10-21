namespace PokemonRipoff.Pokemons.Moves;

public class Charm() : Move("Charm", PType.Normal, 0, 1) {
	public override string Use(GameState gameState, Pokemon player, Pokemon opponent) {
		opponent.Attack = Math.Max(opponent.Attack - 1, 1);
		return $"{player.DisplayName} attacked with {DisplayName}, it lowered the opponent's attack by 1!";
	}
}