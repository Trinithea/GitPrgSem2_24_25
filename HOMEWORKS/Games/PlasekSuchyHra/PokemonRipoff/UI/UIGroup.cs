namespace PokemonRipoff.UI;

// ReSharper disable once InconsistentNaming
public abstract class UIGroup<T>(string id, int padding, Anchor anchor = Anchor.Center, int offset = 0)
	: UIElement(id, anchor, offset) where T : UIGroup<T> {
	public List<UIElement> Children { get; } = new List<UIElement>();
	private int _padding = padding;

	public T AddChild(UIElement uiElement) {
		Children.Add((UIElement)uiElement);
		return (T)this;
	}
}

// ReSharper disable once InconsistentNaming
public class HorizontalUIGroup(string id, int padding, Anchor anchor = Anchor.Center, int offset = 0)
	: UIGroup<HorizontalUIGroup>(id, padding, anchor, offset) {
	public override Sprite[,] Render() {
		List<Sprite[,]> renderedChildren = new List<Sprite[,]>();
		int resultLines = 0;
		for (int i = 0; i < Children.Count; i++) {
			Sprite[,] renderedChild = Children[i].Render();

			if (renderedChild.GetLength(0) > resultLines) {
				resultLines = renderedChild.GetLength(0);
			}

			renderedChildren.Add(renderedChild);
		}

		int fixPad = 0;
		switch (anchor) {
			case Anchor.Top:
			case Anchor.Center:
			case Anchor.Bottom:
				fixPad = renderedChildren.First().GetLength(1) - renderedChildren.Last().GetLength(1);
				break;
		}


		int resultWidth = padding * (renderedChildren.Count - 1) + Math.Abs(fixPad);
		for (int i = 0; i < renderedChildren.Count; i++) {
			resultWidth += renderedChildren[i].GetLength(1);
		}

		Sprite[,] result = new Sprite[resultLines, resultWidth];

		// If fixPad is smaller than 0, we have to pad strings from the left
		// i = line index
		for (int i = 0; i < resultLines; i++) {
			int charOffset = 0;
			if (fixPad < 0) {
				// centering fix pad
				for (int j = 0; j < Math.Abs(fixPad); j++) {
					result[i, charOffset] = new Sprite(' ');
					charOffset++;
				}
			}

			// print first child
			for (int j = 0; j < renderedChildren[0].GetLength(1); j++) {
				result[i, charOffset] = renderedChildren[0][i, j];
				charOffset++;
			}

			// print padding + child combo
			// p = curr child
			for (int p = 1; p < renderedChildren.Count; p++) {
				if (renderedChildren[p].GetLength(0) > i) {
					// padding
					for (int j = 0; j < padding; j++) {
						result[i, charOffset] = new Sprite(' ');
						charOffset++;
					}

					// child
					// j = character index
					for (int j = 0; j < renderedChildren[p].GetLength(1); j++) {
						result[i, charOffset] = renderedChildren[p][i, j];
						charOffset++;
					}
				}
			}

			if (fixPad > 0) {
				// centering fix pad
				for (int j = 0; j < fixPad; j++) {
					result[i, charOffset] = new Sprite(' ');
					charOffset++;
				}
			}
		}

		return result;
	}
}

// ReSharper disable once InconsistentNaming
public class VerticalUIGroup(string id, int padding, Anchor anchor = Anchor.Center, int offset = 0)
	: UIGroup<VerticalUIGroup>(id, padding, anchor, offset) {
	public override Sprite[,] Render() {
		List<Sprite[,]> renderedChildren = new List<Sprite[,]>();
		int resultWidth = 0;
		for (int i = 0; i < Children.Count; i++) {
			Sprite[,] renderedChild = Children[i].Render();

			if (renderedChild.GetLength(1) > resultWidth) {
				resultWidth = renderedChild.GetLength(1);
			}

			renderedChildren.Add(renderedChild);
		}

		int resultLines = padding * (renderedChildren.Count - 1);
		for (int i = 0; i < renderedChildren.Count; i++) {
			resultLines += renderedChildren[i].GetLength(0);
		}
		
		int fixPad = 0;
		
		Sprite[,] result = new Sprite[resultLines, resultWidth];
		int lineIndex = 0;

		// print first child
		for (int i = 0; i < renderedChildren[0].GetLength(0); i++) {
			int childLineWidth = renderedChildren[0].GetLength(1);
			for (int j = 0; j < childLineWidth; j++) {
				result[lineIndex, j] = renderedChildren[0][i, j];
			}

			for (int j = 0; j < resultWidth - childLineWidth; j++) {
				result[lineIndex, childLineWidth + j] = new Sprite(' ');
			}

			lineIndex++;
		}

		// print padding + child combo
		for (int p = 1; p < renderedChildren.Count; p++) {
			// pad
			for (int i = 0; i < padding; i++) {
				for (int j = 0; j < result.GetLength(1); j++) {
					result[lineIndex, j] = new Sprite(' ');
				}

				lineIndex++;
			}

			// child
			for (int i = 0; i < renderedChildren[p].GetLength(0); i++) {
				int childLineWidth = renderedChildren[p].GetLength(1);
				for (int j = 0; j < childLineWidth; j++) {
					result[lineIndex, j] = renderedChildren[p][i, j];
				}

				for (int j = 0; j < resultWidth - childLineWidth; j++) {
					result[lineIndex, childLineWidth + j] = new Sprite(' ');
				}

				lineIndex++;
			}
		}

		return result;
	}
}