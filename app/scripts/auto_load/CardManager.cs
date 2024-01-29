using Godot;
using Newtonsoft.Json;
using OPSProServer.Contracts.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

public partial class CardManager : Node
{
    //public static string CARD_FILE_JSON = @"res://app/resources/json/cards.json";

    private List<string> _cardTextureDownloaders;
    private string _path;

    public List<CardResource> Cards { get; private set; }
    public Texture2D CardTexture { get; private set; }
    public Texture2D LeaderTexture { get; private set; }
    public Texture2D DonTexture { get; private set; }

    private static CardManager _instance;

    public static CardManager Instance => _instance;

    public bool Loaded { get; private set; }

    [Signal]
    public delegate void CardLoadedEventHandler();

    [Signal]
    public delegate void LoadFailedEventHandler();

    public override void _Ready()
    {
        _instance = this;
        _cardTextureDownloaders = new List<string>();

        Loaded = false;

        ServicePointManager.ServerCertificateValidationCallback = (a, b, c, d) => true;
        Cards = new List<CardResource>();

        CardTexture = GD.Load<Texture2D>("res://app/resources/images/card_back.jpg");
        LeaderTexture = GD.Load<Texture2D>("res://app/resources/images/leader_back.png");
        DonTexture = GD.Load<Texture2D>("res://app/resources/images/DonCardBack.jpg");

        _path = ProjectSettings.GlobalizePath($"user://cards");
        Directory.CreateDirectory(_path);

        FetchCards();
    }

    private void FetchCards()
    {
        try
        {
            using(var client = new WebClient())
            {
                client.DownloadStringCompleted += ServerConfigDownloaded;
                client.DownloadStringAsync(new Uri("http://localhost:5282/Cards"));
            }
        } catch(Exception ex)
        {
            Log.Error(ex, $"Failed to fetch cards from the API because {ex.Message}");
        }
    }

    private void ServerConfigDownloaded(object sender, DownloadStringCompletedEventArgs e)
    {
        try
        {
            Cards = JsonConvert.DeserializeObject<List<CardResource>>(e.Result);
            Cards.ForEach(card =>
            {
                card.AskDownloadTexture += Card_AskDownloadTexture;
            });

            Log.Information($"Loaded {Cards.Count} cards");

            Loaded = true;
            EmitSignal(SignalName.CardLoaded);
        }
        catch (Exception ex)
        {
            EmitSignal(SignalName.LoadFailed);
            Log.Error(ex, $"Failed to deserialize cards from the API because {ex.Message}");
        }
    }

    public List<CardResource> Search(string text, double cost, double counter, double power, string color, string set, string type, string cardtype)
    {
        var number = Cards.Count;
        Log.Debug("Searching inside {Number} cards with Text: {Text}, Cost: {Cost}, Counter: {Counter}, Power: {Power}, Color: {Color}, Set: {Set}, Type: {Type}, CardType: {CardType}", number, text, cost, counter, power, color, set, type, cardtype);
        
        List<CardResource> filteredCards = Cards.ToList();
        if (!string.IsNullOrEmpty(text))
        {
            filteredCards = filteredCards.Where(x => x.Name.ToLower().Contains(text.ToLower()) || x.Effects.Any(y => y.ToLower().Contains(text.ToLower()))).ToList();
        }

        if (cost != 0)
        {
            filteredCards = filteredCards.Where(x => x.Cost == cost).ToList();
        }

        if (counter != 0)
        {
            filteredCards = filteredCards.Where(x => x.Counter == counter).ToList();
        }

        if (power != 0)
        {
            filteredCards = filteredCards.Where(x => x.Power == power).ToList();
        }

        if (!string.IsNullOrEmpty(color))
        {
            filteredCards = filteredCards.Where(x => x.Colors.Contains(color)).ToList();
        }

        if (!string.IsNullOrEmpty(set))
        {
            filteredCards = filteredCards.Where(x => x.Set == set).ToList();
        }

        if (!string.IsNullOrEmpty(type))
        {
            filteredCards = filteredCards.Where(x => x.Types.Contains(type)).ToList();
        }

        if (!string.IsNullOrEmpty(cardtype))
        {
            filteredCards = filteredCards.Where(x => x.CardType == cardtype).ToList();
        }

        Log.Debug($"Found {filteredCards.Count} cards");

        return filteredCards;
    }

    public List<string> GetColors()
    {
        return Cards.SelectMany(x => x.Colors).Distinct().ToList();
    }

    public List<string> GetSets()
    {
        return Cards.GroupBy(x => x.Set).Select(x => x.Key).ToList();
    }

    public List<string> GetTypes()
    {
        return Cards.SelectMany(x => x.Types).Distinct().ToList();
    }

    public List<string> GetCardTypes()
    {
        return Cards.GroupBy(x => x.CardType).Select(x => x.Key).ToList();
    }

    public string GetTexturePath(CardResource cardResource)
    {
        return Path.Combine(_path, $"{cardResource.Id}.png");
    }

    public bool TextureExists(CardResource cardResource)
    {
        string path = GetTexturePath(cardResource);
        return Godot.FileAccess.FileExists(path);
    }

    private void Card_AskDownloadTexture(CardResource cardResource)
    {
        DownloadTextureAsync(cardResource);
    }

    public Texture2D GetTexture(CardResource cardResource)
    {
        string path = GetTexturePath(cardResource);
        if (TextureExists(cardResource))
        {
            try
            {
                //Log.Debug($"Getting image for card ID: {cardResource.Id} at {path}");
                var image = new Image();
                Error error = image.Load(path);
                if (error == Error.FileCorrupt)
                {
                    File.Delete(path);
                    return GetBackTexture(cardResource);
                }

                return ImageTexture.CreateFromImage(image);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
            }
        }

        return GetBackTexture(cardResource);
    }

    public CardScript GetCardScript(CardResource cardResource)
    {
        var path = @$"res://app/resources/card_scripts/{cardResource.Id.ToLower().Replace("-", "_")}.cs";
        var csharScript = GD.Load<CSharpScript>(path);
        var script = (CardScript)csharScript?.New();

        return script;
    }

    public void DownloadTextureAsync(CardResource cardResource)
    {
        string path = GetTexturePath(cardResource);

        try
        {
            if (!TextureExists(cardResource))
            {
                if (!_cardTextureDownloaders.Contains(cardResource.Id))
                {
                    _cardTextureDownloaders.Add(cardResource.Id);

                    Log.Debug($"Image for card ID: {cardResource.Id} doesn't exist, downloading {cardResource.Images.First()}...");
                    try
                    {
                        using (var client = new WebClient())
                        {
                            client.DownloadFileCompleted += (s, e) => DownloadFileCompleted(s, e, cardResource);
                            client.DownloadFileAsync(new Uri(cardResource.Images.First()), path);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, $"Failed to download file {cardResource.Id} because {ex.Message}.");
                        _cardTextureDownloaders.Remove(cardResource.Id);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
        }
    }

    private void DownloadFileCompleted(object sender, AsyncCompletedEventArgs e, CardResource cardResource)
    {
        Log.Debug($"Download completed for card ID: {cardResource.Id}");
        _cardTextureDownloaders.Remove(cardResource.Id);

        cardResource.FrontTexture = GetTexture(cardResource);
    }

    public Texture2D GetBackTexture(CardResource cardResource)
    {
        if (cardResource.CardCategory == CardCategory.LEADER)
        {
            return LeaderTexture;
        }

        return CardTexture;
    }

    public Texture2D GetBackTexture(CardCategory CardCategory)
    {
        if (CardCategory == CardCategory.LEADER)
        {
            return LeaderTexture;
        }

        return CardTexture;
    }
}
