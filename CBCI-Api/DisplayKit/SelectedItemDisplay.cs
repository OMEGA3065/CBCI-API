using DisplayKit.Elements;
using LabApi.Features.Wrappers;
using UnityEngine;

namespace CustomItemLib.DisplayKit;

public partial class SelectedItemDisplay : ICanvasReference
{
    public DisplayCanvas DisplayCanvas => Canvas;

    public SelectedItemDisplay(Player owner)
    {
        Owner = owner;
        InitializeComponent();
        Canvas.Spawn(Owner.ReferenceHub);
    }

    public Player Owner { get; }

    public string Item { set => ItemName.Content = value; }
    public Color ItemColor { set => ItemName.Text.Color = value; }
    public string Description { set => ItemDescription.Content = value; }
    public object LastOwner { get; set; }
}