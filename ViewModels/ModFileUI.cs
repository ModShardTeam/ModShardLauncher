using System.ComponentModel;
using System.Runtime.CompilerServices;
using ModShardLauncher.Core.Models;

namespace ModShardLauncher.ViewModels;

public class ModFileUI : INotifyPropertyChanged
{
    public ModFile Data { get; }
    public bool Enabled
    {
        get => Data.Enabled;
        set
        {
            if (Data.Enabled != value)
            {
                Data.Enabled = value;
                OnPropertyChanged();
            }
        }
    }
    public event PropertyChangedEventHandler? PropertyChanged;
    public ModFileUI(ModFile data)
    {
        Data = data;
    }
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}