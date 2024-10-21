namespace PokemonRipoff.Items;

public abstract class Item(string displayName) {
	public string DisplayName { get; } = displayName;
}