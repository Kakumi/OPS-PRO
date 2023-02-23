using Godot;
using System.Collections.Generic;
using System.Linq;

public class Deck : Object
{
    public string Name { get; set; }
    public Dictionary<CardInfo, int> Cards { get; set; }

    public Deck(string name)
    {
        Name = name;
        Cards = new Dictionary<CardInfo, int>();
    }

    public void AddCard(CardInfo cardInfo, int amount = 1)
    {
        if (!Cards.ContainsKey(cardInfo))
        {
            Cards.Add(cardInfo, 0);
        }

        Cards[cardInfo] += amount;
    }

    public void RemoveCard(CardInfo cardInfo, int amount = 1)
    {
        if (Cards.ContainsKey(cardInfo))
        {
            Cards[cardInfo] -= amount;
            if (Cards[cardInfo] <= 0)
            {
                Cards.Remove(cardInfo);
            }
        }
    }

    public int NumberOfCards => Cards.Sum(x => x.Value);

    public int NumberOfCardsTypes(params CardTypeList[] types)
    {
        return Cards.Where(x => types.Contains(x.Key.CardTypeList)).Sum(x => x.Value);
    }

    public Deck Clone(string name)
    {
        var deck = new Deck(name);
        deck.Cards = Cards.ToDictionary(entry => entry.Key, entry => entry.Value);

        return deck;
    }
}