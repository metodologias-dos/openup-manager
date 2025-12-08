using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using OpenUpMan.UI.ViewModels;

namespace OpenUpMan.UI.Views;

public partial class IterationEditWindow : Window
{
    private int _iterationId;
    
    public IterationEditWindow()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public void SetIteration(IterationItemViewModel iteration, string phaseName)
    {
        _iterationId = iteration.Id;
        
        var phaseText = this.FindControl<TextBlock>("PhaseNameText");
        var nameBox = this.FindControl<TextBox>("NameBox");
        var goalBox = this.FindControl<TextBox>("GoalBox");
        var startPicker = this.FindControl<DatePicker>("StartDatePicker");
        var endPicker = this.FindControl<DatePicker>("EndDatePicker");

        if (phaseText != null)
            phaseText.Text = phaseName;

        if (nameBox != null)
            nameBox.Text = iteration.Name;

        if (goalBox != null)
            goalBox.Text = iteration.Goal;

        if (startPicker != null && iteration.StartDate.HasValue)
            startPicker.SelectedDate = new DateTimeOffset(iteration.StartDate.Value);

        if (endPicker != null && iteration.EndDate.HasValue)
            endPicker.SelectedDate = new DateTimeOffset(iteration.EndDate.Value);
    }

    public void OnCancel(object? sender, RoutedEventArgs e)
    {
        Close(null);
    }

    public async void OnSave(object? sender, RoutedEventArgs e)
    {
        var nameBox = this.FindControl<TextBox>("NameBox");
        var goalBox = this.FindControl<TextBox>("GoalBox");
        var startPicker = this.FindControl<DatePicker>("StartDatePicker");
        var endPicker = this.FindControl<DatePicker>("EndDatePicker");

        if (nameBox == null || goalBox == null || startPicker == null || endPicker == null)
        {
            await MessageBox.Show(this, "Error interno al editar iteración.", "Error");
            return;
        }

        if (string.IsNullOrWhiteSpace(nameBox.Text))
        {
            await MessageBox.Show(this, "El nombre de la iteración es requerido.", "Validación");
            return;
        }

        var name = nameBox.Text;
        var goal = goalBox.Text;
        DateTime? start = startPicker.SelectedDate?.DateTime;
        DateTime? end = endPicker.SelectedDate?.DateTime;

        // Return the updated iteration data
        var result = new
        {
            IterationId = _iterationId,
            Name = name,
            Goal = goal,
            StartDate = start,
            EndDate = end
        };

        Close(result);
    }
}

