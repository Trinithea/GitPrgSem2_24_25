namespace PokemonRipoff.Types;

// ReSharper disable once InconsistentNaming
public interface Option<out T> {
	T Unwrap();
	bool IsSome();
}

public class Some<T>(T value) : Option<T> {
	public T Unwrap() => value;
	public bool IsSome() => true;
}

public class None<T> : Option<T> {
	public T Unwrap() => throw new InvalidOperationException();
	public bool IsSome() => false;
}
