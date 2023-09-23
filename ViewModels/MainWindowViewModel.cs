using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

namespace ParseAmazonMusic.ViewModels;
partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _url;
    [ObservableProperty]
    private Playlist _playlist;
    [ObservableProperty]
    private ObservableCollection<Song> _songs;
    public MainWindowViewModel()
    {
        Url = "https://music.amazon.com/playlists/B01M11SBC8";
        Playlist = new Playlist();
        Songs = new ObservableCollection<Song>();
    }
    private void ParsePlaylist(IWebDriver driver)
    {
        try
        {
            Playlist.Type = "PLAYLIST";

            var songs = driver.FindElements(By.CssSelector("#Web\\.TemplatesInterface\\.v1_0\\.Touch\\.DetailTemplateInterface\\.DetailTemplate_1 > music-container > div > div > div._1xpp05rcYIwWA_tv8t2Aac > div > div > music-image-row"));

            for (int i = 0; i < songs.Count; i++)
            {
                Songs.Add(new Song
                {
                    Id = i + 1,
                    Name = songs[i].GetAttribute("primary-text"),
                    Album = songs[i].GetAttribute("secondary-text-2"),
                    Author = songs[i].GetAttribute("secondary-text-1"),
                    Image = songs[i].GetAttribute("image-src"),
                    Duration = driver.FindElement(By.CssSelector($"#Web\\.TemplatesInterface\\.v1_0\\.Touch\\.DetailTemplateInterface\\.DetailTemplate_1 > music-container > div > div > div._1xpp05rcYIwWA_tv8t2Aac > div > div > music-image-row:nth-child({i + 1}) > div > div.col4 > music-link > span")).Text,
                    AlbumTxtHeight = string.Empty
                });
            }
        }
        catch (Exception)
        {
            throw;
        }
    }
    private void ParseAlbum(IWebDriver driver)
    {
        try
        {
            Playlist.Type = "ALBUM";

            var songs = driver.FindElements(By.CssSelector("#Web\\.TemplatesInterface\\.v1_0\\.Touch\\.DetailTemplateInterface\\.DetailTemplate_1 > music-container > div > div > div._1xpp05rcYIwWA_tv8t2Aac > div > div > music-text-row"));

            for (int i = 0; i < songs.Count; i++)
            {
                Songs.Add(new Song
                {
                    Id = i + 1,
                    Name = songs[i].GetAttribute("primary-text"),
                    Duration = driver.FindElement(By.CssSelector($"#Web\\.TemplatesInterface\\.v1_0\\.Touch\\.DetailTemplateInterface\\.DetailTemplate_1 > music-container > div > div > div._1xpp05rcYIwWA_tv8t2Aac > div > div > music-text-row:nth-child({i + 1}) > div.content > div.col4 > music-link > span")).Text,
                    AlbumTxtHeight = "0"
                });
            }
        }
        catch (Exception)
        {
            throw;
        }
    }
    [RelayCommand]
    public async Task LoadSongs()
    {
        Songs.Clear();
        Playlist.Clear();

        await Task.Run(() =>
        {
            if (!Url.Contains("albums") && !Url.Contains("playlists"))
            {
                Playlist.Title = "Incorrect link";
                return;
            }
            new DriverManager().SetUpDriver(new ChromeConfig());

            IWebDriver driver = new ChromeDriver();

            driver.Navigate().GoToUrl(Url);

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(20);

            var desc = driver.FindElement(By.XPath("//*[@id=\"atf\"]/music-detail-header"));

            Playlist.Title = desc.GetAttribute("headline");
            Playlist.PrimaryText = desc.GetAttribute("primary-text");
            Playlist.SecondaryText = desc.GetAttribute("secondary-text");
            Playlist.TertiaryText = desc.GetAttribute("tertiary-text");
            Playlist.Image = desc.GetAttribute("image-src");

            if (Url.Contains("albums"))
            {
                ParseAlbum(driver);
            }
            else if (Url.Contains("playlists"))
            {
                ParsePlaylist(driver);
            }

            driver.Quit();
        });
    }

}
