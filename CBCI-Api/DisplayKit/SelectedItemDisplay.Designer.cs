using UnityEngine;
using UnityEngine.UIElements;
using DisplayKit.Elements;

namespace CustomItemLib.DisplayKit;

public partial class SelectedItemDisplay
{
    public DisplayCanvas Canvas;
    public DisplayText ItemName;
    public DisplayText ItemDescription;
    private DisplayElement Element0;

    private void InitializeComponent()
    {
        // Canvas
        Canvas = DisplayCanvas.Create();
        Canvas.Flex.Grow = 1f;
        Canvas.Align.JustifyContent = Justify.FlexEnd;
        Canvas.Align.AlignSelf = Align.Center;
        Canvas.Align.AlignItems = Align.Center;

        // ItemName
        ItemName = Canvas.AddText();
        ItemName.Content = "CustomItemName";
        ItemName.Text.FontSize = new Length(64f, LengthUnit.Pixel);
        ItemName.Text.TextShadow = new TextShadow() { offset = new Vector2(3f, 3f), blurRadius = 3f, color = new Color(0f, 0f, 0f, 1f) };
        ItemName.Text.Color = new Color(1f, 0f, 0f, 1f);
        ItemName.Text.Align = TextAnchor.UpperCenter;

        // ItemDescription
        ItemDescription = Canvas.AddText();
        ItemDescription.Content = "My amazing custom item has an amazing description.";
        ItemDescription.Text.FontSize = new Length(24f, LengthUnit.Pixel);
        ItemDescription.Text.Color = new Color(1f, 1f, 1f, 1f);
        ItemDescription.Text.TextShadow = new TextShadow() { offset = new Vector2(3f, 3f), blurRadius = 3f, color = new Color(0f, 0f, 0f, 1f) };
        ItemDescription.Text.Wrap = WhiteSpace.Normal;
        ItemDescription.Text.Align = TextAnchor.UpperCenter;
        ItemDescription.Spacing.MarginBottom = new Length(4f, LengthUnit.Pixel);

        // Element0
        Element0 = Canvas.AddElement();
        Element0.Flex.Grow = 0f;
        Element0.Flex.Shrink = 0f;
        Element0.Size.MinHeight = new Length(10f, LengthUnit.Percent);
    }
}
