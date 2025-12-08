using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace OpenUpMan.UI.Views;

public partial class MessageBoxWindow : Window
{
    private TextBlock? _titleTextBlock;
    private TextBlock? _messageTextBlock;

    public MessageBoxWindow()
    {
        InitializeComponent();
    }

    public MessageBoxWindow(string title, string message, MessageBoxType type) : this()
    {
        Title = title;
        
        _titleTextBlock = this.FindControl<TextBlock>("TitleTextBlock");
        _messageTextBlock = this.FindControl<TextBlock>("MessageTextBlock");

        if (_titleTextBlock != null)
        {
            _titleTextBlock.Text = title;
            
            // Set color based on type
            _titleTextBlock.Foreground = type switch
            {
                MessageBoxType.Success => new SolidColorBrush(Color.FromRgb(34, 139, 34)),
                MessageBoxType.Warning => new SolidColorBrush(Color.FromRgb(255, 140, 0)),
                MessageBoxType.Error => new SolidColorBrush(Color.FromRgb(220, 20, 60)),
                _ => new SolidColorBrush(Color.FromRgb(0, 0, 0))
            };
        }

        if (_messageTextBlock != null)
        {
            _messageTextBlock.Text = message;
        }
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void OnOkClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}

public enum MessageBoxType
{
    Success,
    Warning,
    Error,
    Info
}

