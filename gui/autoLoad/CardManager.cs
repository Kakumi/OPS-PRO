using Godot;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

public class CardManager : Node
{
    public static string CARD_FILE_JSON = @"user://all.json";

    private Dictionary<string, Task> _downloaders;

    public List<CardInfo> Cards { get; private set; }
    public Texture CardTexture { get; private set; }
    public Texture LeaderTexture { get; private set; }
    public Texture DonTexture { get; private set; }

    private static CardManager _instance;
    private static object _lock = new object();

    public override void _Ready()
    {
        _instance = this;
        _downloaders = new Dictionary<string, Task>();

        ServicePointManager.ServerCertificateValidationCallback = (a, b, c, d) => true;
        Cards = new List<CardInfo>();

        CardTexture = GD.Load<Texture>("res://resources/images/card_back.jpg");
        LeaderTexture = GD.Load<Texture>("res://resources/images/leader_back.png");
        DonTexture = GD.Load<Texture>("res://resources/images/don_back.jpg");

        File file = new File();
        file.Open(CARD_FILE_JSON, File.ModeFlags.Read);
        Cards = JsonConvert.DeserializeObject<List<CardInfo>>(file.GetAsText());
        Log.Information($"Loaded {Cards.Count} cards");
    }

    public static CardManager Instance => _instance;

    public List<CardInfo> Search(string text, double cost, double counter, double power, string color, string set, string type, string cardtype)
    {
        var number = Cards.Count;
        Log.Debug("Searching inside {Number} cards with Text: {Text}, Cost: {Cost}, Counter: {Counter}, Power: {Power}, Color: {Color}, Set: {Set}, Type: {Type}, CardType: {CardType}", number, text, cost, counter, power, color, set, type, cardtype);
        
        List<CardInfo> filteredCards = Cards.ToList();
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

    public Texture GetTexture(CardInfo cardInfo)
    {
        string path = ProjectSettings.GlobalizePath($"user://cards/{cardInfo.Id}.png");
        File file = new File();
        if (file.FileExists(path))
        {
            try
            {
                //Log.Debug($"Getting image for card ID: {cardInfo.Id} at {path}");
                var image = new Image();
                Error error = image.Load(path);
                if (error == Error.FileCorrupt)
                {
                    var dir = new Directory();
                    dir.Remove(path);
                    return GetBackTexture(cardInfo);
                }

                var imageTexture = new ImageTexture();
                imageTexture.CreateFromImage(image);
                return imageTexture;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
            }
        }

        return GetBackTexture(cardInfo);
    }

    public async Task<Texture> DownloadAndGetTexture(CardInfo cardInfo)
    {
        string path = ProjectSettings.GlobalizePath($"user://cards/{cardInfo.Id}.png");
        File file = new File();
        if (!file.FileExists(path))
        {
            try
            {
                Task task;
                lock (_lock)
                {
                    if (_downloaders.ContainsKey(cardInfo.Id))
                    {
                        Log.Debug($"Downloader contains key {cardInfo.Id}, waiting current task...");
                        task = _downloaders[cardInfo.Id];
                    }
                    else
                    {
                        Log.Debug($"Downloader is empty for {cardInfo.Id}, creating new task...");
                        task = Task.Run(() =>
                        {
                            Log.Debug($"Image for card ID: {cardInfo.Id} doesn't exist, downloading {cardInfo.Images.First()}...");
                            using (var webClient = new WebClient())
                            {
                                webClient.DownloadFile(cardInfo.Images.First(), path);
                                Log.Debug($"Done for card ID: {cardInfo.Id}");
                            }
                        });

                        _downloaders.Add(cardInfo.Id, task);
                    }
                }

                await task;
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return GetBackTexture(cardInfo);
            }
            finally
            {
                _downloaders.Remove(cardInfo.Id);
                //Log.Debug($"Downloader task for {cardInfo.Id} removed");
            }
        }

        return GetTexture(cardInfo);
    }

    public Texture GetBackTexture(CardInfo cardInfo)
    {
        if (cardInfo.CardTypeList == CardTypeList.LEADER)
        {
            return LeaderTexture;
        }

        return CardTexture;
    }
}
