using Godot;
using Godot.Collections;
using OPSProServer.Contracts.Models;
using System.Linq;

public partial class DeckResource : Resource
{
    [Export]
    public string Name { get; set; }
    [Export]
    public Dictionary<CardResource, int> Cards { get; set; }

    public DeckResource(string name)
    {
        Name = name;
        Cards = new Dictionary<CardResource, int>();
    }

    public void AddCard(CardResource cardResource, int amount = 1)
    {
        if (!Cards.ContainsKey(cardResource))
        {
            Cards.Add(cardResource, 0);
        }

        Cards[cardResource] += amount;
    }

    public void RemoveCard(CardResource cardResource, int amount = 1)
    {
        if (Cards.ContainsKey(cardResource))
        {
            Cards[cardResource] -= amount;
            if (Cards[cardResource] <= 0)
            {
                Cards.Remove(cardResource);
            }
        }
    }

    public int NumberOfCards => Cards.Sum(x => x.Value);

    public int NumberOfCardsTypes(params CardCategory[] types)
    {
        return Cards.Where(x => types.Contains(x.Key.CardCategory)).Sum(x => x.Value);
    }

    public DeckResource Clone(string name)
    {
        var deck = new DeckResource(name);
        deck.Cards = new Dictionary<CardResource, int>(Cards.ToDictionary(entry => entry.Key, entry => entry.Value));

        return deck;
    }

    public int NumberOfCardsNumber(string number)
    {
        return Cards.Count(x => x.Key.GetScriptCode() == number);
    }

    public bool IsValid()
    {
        var totalCards = NumberOfCardsTypes(CardCategory.CHARACTER, CardCategory.EVENT, CardCategory.STAGE);
        var totalLeader = NumberOfCardsTypes(CardCategory.LEADER);
        var leader = Cards.FirstOrDefault(x => x.Key.CardCategory == CardCategory.LEADER).Key;
        var exceedsSameCard = Cards.Any(x => NumberOfCardsNumber(x.Key.GetScriptCode()) > 4);

        return totalCards == 50 && totalLeader == 1 && leader != null && !exceedsSameCard && Cards.All(x => x.Key.Colors.Any(y => leader.Colors.Contains(y)));
    }

    public System.Collections.Generic.List<string> GetCardsId()
    {
        var list = new System.Collections.Generic.List<string>();
        foreach (var card in Cards)
        {
            for (int i = 0; i < card.Value; i++)
            {
                list.Add(card.Key.Id);
            }
        }

        return list;
    }
}