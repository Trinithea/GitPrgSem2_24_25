using System.Reflection;
using PRMap = PokemonRipoff.Map;
using PRUI = PokemonRipoff.UI;

namespace PokemonRipoff.Scenes;

public delegate void KeyEventHook(ConsoleKeyInfo keyInfo, Game game);

public delegate void PreRenderHook(Renderer renderer, Game game);

public delegate void PostRenderHook(Game game);

/*public class SceneValue(object value) {
	private object _value = value;
	public long UpdatedAt { get; private set; } = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

	public object Value {
		get => _value;
		set {
			_value = value;
			UpdatedAt = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
		}
	}
}

public class BoolSceneValue(bool value) : SceneValue(value) {
	private bool _value = value;
	public bool Value {
		get => _value;
	}
}
public class CharSceneValue(char value) : SceneValue<char>(value) {}
public class IntSceneValue(int value) : SceneValue<int>(value) {}*/

public class Scene(string id) {
	public string Id { get; } = id;

	// ReSharper disable once InconsistentNaming
	protected Dictionary<string, PRUI.UIElement> UIElements = new Dictionary<string, PRUI.UIElement>();
	public KeyEventHook KeyEventHook { get; protected set; } = (info, game) => { };
	public PreRenderHook PreRenderHook { get; protected set; } = (renderer, game) => { };
	public PostRenderHook PostRenderHook { get; protected set; } = (game) => { };

	// ReSharper disable once InconsistentNaming
	public Scene AddUIElement(PRUI.UIElement uiElement) {
		UIElements.Add(uiElement.Id, uiElement);
		return this;
	}

	public Types.Option<T> GetUIElement<T>(string elementId) where T : PRUI.UIElement {
		if (!UIElements.TryGetValue(elementId, out var element)) {
			return new Types.None<T>();
		}

		return new Types.Some<T>((T)element);
	}

	public void RegisterKeyEventHook(KeyEventHook hook) {
		KeyEventHook = hook;
	}

	public void RegisterPreRenderHook(PreRenderHook hook) {
		PreRenderHook = hook;
	}

	public void RegisterPostRenderHook(PostRenderHook hook) {
		PostRenderHook = hook;
	}

	public void Render(Renderer renderer, Game game) {
		foreach (PRUI.UIElement uiElement in UIElements.Values) {
			renderer.Buffer(uiElement);
		}

		PreRenderHook(renderer, game);

		renderer.Render();

		PostRenderHook(game);
	}
}

