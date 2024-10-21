namespace PokemonRipoff.Pokemons.Moves;

public class Scratch() : Move("Scratch", PType.Normal, 40, 1) {
	public override string Use(GameState gameState, Pokemon player, Pokemon opponent) {
		Random rnd = new Random();

		double baseDamage = (2 * player.Level / 5.0 + 2) * Power * (player.Attack / (double)opponent.Defense) / 50 + 2;
		double stab = 1 + 0.5 * (player.Type.Contains(Type) ? 1 : 0); // same type attack bonus
		int damage = (int)Math.Floor(baseDamage * stab * rnd.Next(217, 256) / 255);

		opponent.Health -= damage;
		return $"{player.DisplayName} attacked with {DisplayName}, it dealt {damage} damage!";
	} 
}