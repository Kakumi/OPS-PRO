using Godot;
using System.Collections.Generic;
using System.Linq;
using Serilog;
using System;
using System.IO;

public partial class DeckManager : Node
{
    private static DeckManager _instance;
    public string Path3D { get; private set; }

    public static DeckManager Instance => _instance;

    public override void _Ready()
    {
        _instance = this;
        Path3D = ProjectSettings.GlobalizePath("user://decks");
        Directory.CreateDirectory(Path3D);
    }

    public List<DeckResource> LoadDecks()
    {
        List<DeckResource> decks = new List<DeckResource>();
        var files = Directory.GetFiles(Path3D, "*.ops").ToList<string>();
        files.ForEach(f =>
        {
            try
            {
                FileInfo fileInfo = new FileInfo(f);
                var filename = Path.GetFileNameWithoutExtension(fileInfo.Name);
                var deck = new DeckResource(filename);

                var lines = File.ReadAllLines(f).ToList<string>();
                lines.ForEach(line =>
                {
                    var infos = line.Split(" ");
                    if (infos.Count() == 2)
                    {
                        var number = int.Parse(infos[0]);
                        var id = infos[1];
                        var card = CardManager.Instance.Cards.Where(x => id == x.Id).FirstOrDefault();

                        if (card == null)
                        {
                            Log.Warning($"Card for id {id} is not found for deck {filename}.");
                        }
                        else
                        {
                            deck.Cards.Add(card, number);
                        }
                    }
                });
                
                decks.Add(deck);
            } catch(Exception e)
            {
                Log.Error(e, e.Message);
            }
        });

        return decks;
    }

    public DeckResource Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            name = Guid.NewGuid().ToString();
        }

        var deck = new DeckResource(name);

        var filename = GetDeckFilename(deck);
        if (File.Exists(filename))
        {
            throw new Exception(string.Format(Tr("DECKMANAGER_ERROR_EXIST"), filename));
        }

        Save(deck);

        return deck;
    }

    public void Save(DeckResource deck, string newName = null)
    {
        if (!string.IsNullOrWhiteSpace(newName) && deck.Name != newName)
        {
            var oldFilename = GetDeckFilename(deck);
            var isNewNameValid = newName.IndexOfAny(Path.GetInvalidFileNameChars()) < 0;
            if (!isNewNameValid)
            {
                throw new Exception(string.Format(Tr("DECKMANAGER_ERROR_FILENAME"), newName));
            }

            File.Delete(oldFilename);
            deck.Name = newName;
        }

        var isNameValid = !string.IsNullOrEmpty(deck.Name) && deck.Name.IndexOfAny(Path.GetInvalidFileNameChars()) < 0;
        if (!isNameValid)
        {
            throw new Exception(string.Format(Tr("DECKMANAGER_ERROR_FILENAME"), deck.Name));
        }

        var filename = GetDeckFilename(deck);
        File.WriteAllLines(filename, deck.Cards.Select(x => $"{x.Value} {x.Key.Id}"));
    }

    public void Delete(DeckResource deck)
    {
        var filename = GetDeckFilename(deck);
        File.Delete(filename);
    }

    public DeckResource Duplicate(DeckResource deck)
    {
        var newDeck = deck.Clone(Guid.NewGuid().ToString());

        Save(newDeck);
        return newDeck;
    }

    public void Clear(DeckResource deck)
    {
        deck.Cards = new Godot.Collections.Dictionary<CardResource, int>();
    }

    private string GetDeckFilename(DeckResource deck)
    {
        return Path.Combine(Path3D, $"{deck.Name}.ops");
    }
}
