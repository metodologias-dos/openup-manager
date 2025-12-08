using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace OpenUpMan.UI.ViewModels;

public partial class MicroincrementItemViewModel : ViewModelBase
{
    
    public string DateFormatted => Date.ToString("dd/MM/yyyy");

    public bool HasArtifact => ArtifactId.HasValue;

    [ObservableProperty]
    private string? _evidenceUrl;

    [ObservableProperty]
    private string? _artifactName;

    [ObservableProperty]
    private int? _artifactId;

    [ObservableProperty]
    private string _authorName = "Desconocido";

    [ObservableProperty]
    private DateTime _date;

    [ObservableProperty]
    private string _type = "functional";

    [ObservableProperty]
    private string? _description;

    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    private string _iterationName = string.Empty;

    [ObservableProperty]
    private int _iterationId;

    [ObservableProperty]
    private int _id;
}




