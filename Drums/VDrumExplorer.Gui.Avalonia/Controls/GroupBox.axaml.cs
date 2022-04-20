using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace VDrumExplorer.Gui.Controls;

public class GroupBox : ContentControl
{
    public static readonly StyledProperty<object> HeaderProperty =
        AvaloniaProperty.Register<GroupBox, object>(nameof(Header));

    public object Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }
}