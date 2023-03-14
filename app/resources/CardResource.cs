using Godot;
using Godot.Collections;
using System.Linq;

public partial class CardResource : Resource
{
    [Export]
    public string Id { get; set; }
    private Texture _frontTexture;
    [Export]
    public Texture FrontTexture
    {
        set { _frontTexture = value; }
        get
        {
            if (_frontTexture == null)
            {
                _frontTexture = GD.Load<Texture>("res://app/resources/OP01-001.png"); //CardManager.Instance.GetBackTexture(CardTypeList);
            }

            return _frontTexture;
        }
    }

    private Texture _backTexture;
    [Export]
    public Texture BackTexture
    {
        set { _backTexture = value; }
        get
        {
            if (_backTexture == null)
            {
                _backTexture = CardManager.Instance.GetBackTexture(CardTypeList);
            }

            return _backTexture;
        }
    }
    [Export]
    public string Number { get; set; }
    [Export]
    public string Rarity { get; set; }
    [Export]
    public string CardType { get; set; }
    [Export]
    public string Name { get; set; }
    [Export]
    public int Cost { get; set; }
    [Export]
    public string AttributeImage { get; set; }
    [Export]
    public string Attribute { get; set; }
    [Export]
    public int Power { get; set; }
    [Export]
    public int Counter { get; set; }
    [Export]
    public Array<string> Colors { get; set; }
    [Export]
    public Array<string> Types { get; set; }
    [Export]
    public Array<string> Effects { get; set; }
    [Export]
    public new string Set { get; set; }

    public static CardResource Create(CardInfo cardInfo)
    {
        var resource = new CardResource();
        resource.Id = cardInfo.Id;
        resource.Number = cardInfo.Number;
        resource.Rarity = cardInfo.Rarity;
        resource.CardType = cardInfo.CardType;
        resource.Name = cardInfo.Name;
        resource.Cost = cardInfo.Cost;
        resource.AttributeImage = cardInfo.AttributeImage;
        resource.Attribute = cardInfo.Attribute;
        resource.Power = cardInfo.Power;
        resource.Counter = cardInfo.Counter;
        resource.Colors = new Array<string>(cardInfo.Colors);
        resource.Types = new Array<string>(cardInfo.Types);
        resource.Effects = new Array<string>(cardInfo.Effects);
        resource.Set = cardInfo.Set;

        return resource;
    }

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
