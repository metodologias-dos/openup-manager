using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using OpenUpMan.Data;
using OpenUpMan.Domain;

namespace OpenUpMan.UI.Views;

public partial class IterationCreateWindow : Window
{
    public IterationCreateWindow()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public void SetPhases(System.Collections.Generic.IEnumerable<Phase> phases)
    {
        var combo = this.FindControl<ComboBox>("PhaseCombo");
        if (combo != null)
        {
            combo.ItemsSource = phases;
            combo.SelectedIndex = 0;
        }
    }

    private void OnCancel(object? sender, RoutedEventArgs e)
    {
        Close(null);
    }

    private async void OnCreate(object? sender, RoutedEventArgs e)
    {
        var combo = this.FindControl<ComboBox>("PhaseCombo");
        var nameBox = this.FindControl<TextBox>("NameBox");
        var goalBox = this.FindControl<TextBox>("GoalBox");
        var startPicker = this.FindControl<DatePicker>("StartDatePicker");
        var endPicker = this.FindControl<DatePicker>("EndDatePicker");

        if (combo == null || nameBox == null || goalBox == null || startPicker == null || endPicker == null)
        {
            await MessageBox.Show(this, "Error interno al crear iteración.", "Error");
            return;
        }

        if (combo.SelectedItem is not Phase phase)
        {
            await MessageBox.Show(this, "Seleccione una fase.", "Validación");
            return;
        }

        var name = nameBox.Text;
        var goal = goalBox.Text;
        DateTime? start = startPicker.SelectedDate?.DateTime;
        DateTime? end = endPicker.SelectedDate?.DateTime;

        var iteration = new Iteration(phase.Id, name, goal);
        if (start.HasValue || end.HasValue)
            iteration.UpdateDetails(name, goal, start, end);

        Close(iteration);
    }
}

// Small helper message box, simple implementation (Avalonia doesn't include MessageBox by default)
static class MessageBox
{
    public static async System.Threading.Tasks.Task Show(Window parent, string text, string title)
    {
        var w = new Window
        {
            Content = new TextBlock { Text = text, Margin = new Avalonia.Thickness(12) },
            Width = 320,
            Height = 120,
            Title = title,
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        };

        if (parent != null)
            await w.ShowDialog(parent);
        else
            w.Show();
    }
}
