using Godot;
using System.Collections.Generic;
using System.Linq;
using Serilog;
using System;

using SFile = System.IO.File;
using SDirectory = System.IO.Directory;
using SFileInfo = System.IO.FileInfo;
using SPath = System.IO.Path;

public class DeckManager
{
    private static readonly Lazy<DeckManager> _instance = new Lazy<DeckManager>(() => new DeckManager());
    public string Path { get; private set; }

    public static DeckManager Instance => _instance.Value;

    public DeckManager()
    {
        Path = ProjectSettings.GlobalizePath("user://decks");
        SDirectory.CreateDirectory(Path);
    }

    public List<Deck> LoadDecks()
    {
        List<Deck> decks = new List<Deck>();
        var files = SDirectory.GetFiles(Path, "*.ops").ToList<string>();
        files.ForEach(f =>
        {
            try
            {
                SFileInfo fileInfo = new SFileInfo(f);
                var filename = SPath.GetFileNameWithoutExtension(fileInfo.Name);
                var deck = new Deck(filename);

                var lines = SFile.ReadAllLines(f).ToList<string>();
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

    public Deck Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            name = Guid.NewGuid().ToString();
        }

        var deck = new Deck(name);

        var filename = GetDeckFilename(deck);
        if (SFile.Exists(filename))
        {
            throw new Exception($"A deck already exists for name {filename}.");
        }

        Save(deck);

        return deck;
    }

    public void Save(Deck deck, string newName = null)
    {
        if (!string.IsNullOrWhiteSpace(newName) && deck.Name != newName)
        {
            var oldFilename = GetDeckFilename(deck);
            var isNewNameValid = newName.IndexOfAny(SPath.GetInvalidFileNameChars()) < 0;
            if (!isNewNameValid)
            {
                throw new Exception($"Name is invalid because it contains unsupported characters for a filename ({newName}).");
            }

            SFile.Delete(oldFilename);
            deck.Name = newName;
        }

        var isNameValid = !string.IsNullOrEmpty(deck.Name) && deck.Name.IndexOfAny(SPath.GetInvalidFileNameChars()) < 0;
        if (!isNameValid)
        {
            throw new Exception($"Name is invalid because it contains unsupported characters for a filename ({deck.Name}).");
        }

        var filename = GetDeckFilename(deck);
        SFile.WriteAllLines(filename, deck.Cards.Select(x => $"{x.Value} {x.Key.Id}"));
    }

    public void Delete(Deck deck)
    {
        var filename = GetDeckFilename(deck);
        SFile.Delete(filename);
    }

    public Deck Duplicate(Deck deck)
    {
        var newDeck = deck.Clone(Guid.NewGuid().ToString());

        Save(newDeck);
        return newDeck;
    }

    public void Clear(Deck deck)
    {
        deck.Cards = new Dictionary<CardInfo, int>();
    }

    private string GetDeckFilename(Deck deck)
    {
        return SPath.Combine(Path, $"{deck.Name}.ops");
    }
}
