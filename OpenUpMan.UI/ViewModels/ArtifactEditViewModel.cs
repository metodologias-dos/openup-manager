using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenUpMan.Data;
using OpenUpMan.Domain;

namespace OpenUpMan.UI.ViewModels;

public partial class ArtifactEditViewModel : ViewModelBase
{
  private readonly IArtifactRepository _artifactRepository;
  private readonly int _artifactId;

  public event Action? CloseRequested;

  [ObservableProperty]
  private string _name = string.Empty;

  [ObservableProperty]
  private string _artifactType = string.Empty;

  private int _currentUserId;
  private string _currentUserName = string.Empty;

  [ObservableProperty]
  private string _authorName = string.Empty; // For display or manual entry

  [ObservableProperty]
  private DateTimeOffset? _date = DateTimeOffset.Now;

  [ObservableProperty]
  private bool _isMandatory;

  [ObservableProperty]
  private string _filePath = string.Empty;

  [ObservableProperty]
  private string _link = string.Empty;

  [ObservableProperty]
  private bool _hasFile;

  public IRelayCommand SaveCommand { get; }
  public IRelayCommand CancelCommand { get; }
  public IRelayCommand BrowseFileCommand { get; }
  public IRelayCommand OpenContentCommand { get; }

  public ArtifactEditViewModel(IArtifactRepository artifactRepository, int artifactId, int userId, string userName)
  {
    _artifactRepository = artifactRepository;
    _artifactId = artifactId;
    _currentUserId = userId;
    _currentUserName = userName;

    SaveCommand = new AsyncRelayCommand(SaveAsync);
    CancelCommand = new RelayCommand(() => CloseRequested?.Invoke());
    BrowseFileCommand = new AsyncRelayCommand(BrowseFileAsync);
    OpenContentCommand = new AsyncRelayCommand(OpenContentAsync);
  }

  // Design-time constructor
  public ArtifactEditViewModel()
  {
    _artifactRepository = null!;
    _artifactId = 0;
    Name = "Artefacto de Ejemplo";
    ArtifactType = "Documento";
    IsMandatory = true;
    SaveCommand = new RelayCommand(() => { });
    CancelCommand = new RelayCommand(() => { });
    BrowseFileCommand = new RelayCommand(() => { });
    OpenContentCommand = new RelayCommand(() => { });
  }

  partial void OnLinkChanged(string value)
  {
    if (!string.IsNullOrEmpty(value))
    {
      FilePath = string.Empty;
    }
  }

  partial void OnFilePathChanged(string value)
  {
    if (!string.IsNullOrEmpty(value))
    {
      Link = string.Empty;
    }
  }

  public async Task LoadAsync()
  {
    if (_artifactRepository == null) return;

    // Set placeholder for current user since we don't have the context passed down yet
    AuthorName = _currentUserName;

    var artifact = await _artifactRepository.GetByIdAsync(_artifactId);
    if (artifact != null)
    {
      Name = artifact.Name;
      ArtifactType = artifact.ArtifactType ?? string.Empty;
      IsMandatory = artifact.Mandatory;

      // Load latest version info if exists
      var version = await _artifactRepository.GetLatestVersionAsync(_artifactId);
      if (version != null)
      {
        Link = version.BuildInfo ?? string.Empty;
        HasFile = version.FileBlob != null && version.FileBlob.Length > 0;

        if (HasFile)
        {
          // Convert UTC to local time for display
          var utcDate = DateTime.SpecifyKind(version.CreatedAt, DateTimeKind.Utc);
          Date = utcDate.ToLocalTime();
        }
        else
        {
          Date = null;
        }
      }
      else
      {
        Date = null;
      }
    }
  }

  private async Task OpenContentAsync()
  {
    if (!string.IsNullOrEmpty(Link))
    {
      OpenUrl(Link);
    }
    else if (!string.IsNullOrEmpty(FilePath))
    {
      OpenFile(FilePath);
    }
    else if (HasFile && _artifactRepository != null)
    {
      var version = await _artifactRepository.GetLatestVersionAsync(_artifactId);
      if (version?.FileBlob != null)
      {
        try
        {
          string ext = ".bin";
          if (!string.IsNullOrEmpty(version.FileMime))
          {
            // Simple mime mapping
            if (version.FileMime.Contains("pdf")) ext = ".pdf";
            else if (version.FileMime.Contains("image")) ext = ".png"; // simplified
            else if (version.FileMime.Contains("text")) ext = ".txt";
            else if (version.FileMime.Contains("word")) ext = ".docx";
          }

          string safeName = string.Join("_", Name.Split(Path.GetInvalidFileNameChars()));
          string tempPath = Path.Combine(Path.GetTempPath(), safeName + ext);

          await File.WriteAllBytesAsync(tempPath, version.FileBlob);
          OpenFile(tempPath);
        }
        catch (Exception ex)
        {
          Console.WriteLine($"Error opening file: {ex.Message}");
        }
      }
    }
  }

  private void OpenUrl(string url)
  {
    try
    {
      Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Error opening URL: {ex.Message}");
    }
  }

  private void OpenFile(string path)
  {
    try
    {
      Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Error opening file: {ex.Message}");
    }
  }

  private async Task SaveAsync()
  {
    if (_artifactRepository == null) return;

    var artifact = await _artifactRepository.GetByIdAsync(_artifactId);
    if (artifact != null)
    {
      // Update Artifact details
      artifact.UpdateDetails(artifact.Name, ArtifactType, IsMandatory, artifact.Description);
      await _artifactRepository.UpdateAsync(artifact);

      // Create new version if file or link is provided
      // Note: This is a simplified implementation. 
      // In a real app we would check if things changed.
      // Also we need to handle the file upload (reading bytes).

      byte[]? fileBytes = null;
      string? mimeType = null;

      if (!string.IsNullOrEmpty(FilePath))
      {
        try
        {
          fileBytes = await System.IO.File.ReadAllBytesAsync(FilePath);
          mimeType = "application/octet-stream"; // Simplified
        }
        catch (Exception ex)
        {
          // Handle error
          Console.WriteLine($"Error reading file: {ex.Message}");
        }
      }

      if (fileBytes != null || !string.IsNullOrEmpty(Link))
      {
        // Create a new version
        // We don't have the current user ID here easily without passing it.
        // Let's assume 1 (System/Admin) or 0 for now if not available.
        int userId = _currentUserId;

        var version = new ArtifactVersion(
            _artifactId,
            userId,
            notes: $"Updated by user. Author: {AuthorName}",
            fileBlob: fileBytes,
            fileMime: mimeType,
            buildInfo: Link
        );

        // We need a method to add version in repo
        await _artifactRepository.AddVersionAsync(version);
      }

      // Update state to REGISTERED if it has content
      if (fileBytes != null || !string.IsNullOrEmpty(Link))
      {
        artifact.SetState("REGISTERED");
        await _artifactRepository.UpdateAsync(artifact);
      }
    }

    CloseRequested?.Invoke();
  }

  private Task BrowseFileAsync()
  {
    // This needs to be handled by the View via an interaction or service
    // For now we will just simulate or use a dialog service if available.
    // Since we are in ViewModel, we can't open dialog directly.
    // We'll expose an event or use a service.
    // Let's use an event.
    BrowseFileRequested?.Invoke();
    return Task.CompletedTask;
  }

  public event Action? BrowseFileRequested;

  public void SetFilePath(string path)
  {
    FilePath = path;
  }
}
