using Godot;
using System.Collections.Generic;

public partial class CardInfo : GodotObject
{
    public string Id { get; set; }
    public List<string> Images { get; set; }
    public string Number { get; set; }
    public string Rarity { get; set; }
    public string CardType { get; set; }
    public string Name { get; set; }
    public int Cost { get; set; }
    public string AttributeImage { get; set; }
    public string Attribute { get; set; }
    public int Power { get; set; }
    public int Counter { get; set; }
    public List<string> Colors { get; set; }
    public List<string> Types { get; set; }
    public List<string> Effects { get; set; }
    public new string Set { get; set; }

    public CardTypeList CardTypeList
    {
        get
        {
            if (CardType == "LEADER") return CardTypeList.LEADER;
            if (CardType == "CHARACTER") return CardTypeList.CHARACTER;
            if (CardType == "STAGE") return CardTypeList.STAGE;
            if (CardType == "EVENT") return CardTypeList.EVENT;

            return CardTypeList.NONE;
        }
    }
}
