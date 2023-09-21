using CommunityToolkit.Mvvm.ComponentModel;

namespace ParseAmazonMusic.ViewModels;

partial class Playlist : ObservableObject
{
    [ObservableProperty]
    private string _image;
    [ObservableProperty]
    public string _title;
    [ObservableProperty]
    public string _primaryText;
    [ObservableProperty]
    public string _secondaryText;
    [ObservableProperty]
    public string _tertiaryText;
    [ObservableProperty]
    public string _type;

    public void Clear()
    {
        Image = string.Empty;
        Title = string.Empty;
        PrimaryText = string.Empty;
        SecondaryText = string.Empty;
        TertiaryText = string.Empty;
        Type = string.Empty;
    }
}
