namespace PokemonRipoff.Pokemons.Moves;

public class Growl() : Move("Growl", PType.Normal, 0, 1) {
	public override string Use(GameState gameState, Pokemon player, Pokemon opponent) {
		opponent.Defense = Math.Max(opponent.Defense - 1, 1);
		return $"{player.DisplayName} attacked with {DisplayName}, it lowered the opponent's defense by 1!";
	}
}