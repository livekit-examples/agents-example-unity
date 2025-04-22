using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class Fade: VisualElement
{
    public Fade()
    {
        generateVisualContent += GenerateVisualContent;
    }

    private void GenerateVisualContent(MeshGenerationContext context)
    {
        float width = contentRect.width;
        float height = contentRect.height;

        var p = context.painter2D;
        p.lineWidth = width;
        p.strokeGradient = new Gradient
        {
            alphaKeys = new[] { new GradientAlphaKey(0, 0), new GradientAlphaKey(1, 1) },
            colorKeys = new[] { new GradientColorKey(Color.black, 0), new GradientColorKey(Color.black, 1) }
        };
        p.BeginPath();
        p.MoveTo(new Vector2(width / 2, 0));
        p.LineTo(new Vector2(width / 2, height));
        p.Stroke();
    }
}
