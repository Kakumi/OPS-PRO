using Godot;
using Godot.Collections;
using OPSProServer.Contracts.Models;
using System.Linq;

public partial class CardResource : Resource
{
    public bool TextureSet { get; set; } = false;

    [Export]
    private string _id;
    public string Id
    {
        set
        {
            _id = value;
            if ((FrontTexture == null || FrontTexture == BackTexture) && CardManager.Instance.TextureExists(this))
            {
                TextureSet = true;
                FrontTexture = CardManager.Instance.GetTexture(this);
            }
        }
        get => _id;
    }
    [Export]
    public Array<string> Images { get; set; }

    private Texture2D _frontTexture;
    [Export]
    public Texture2D FrontTexture
    {
        set { 
            _frontTexture = value;
            EmitSignal(SignalName.FrontTextureChanged, value);
        }
        get
        {
            if (_frontTexture == null)
            {
                FrontTexture = CardManager.Instance.GetBackTexture(CardCategory);
            }

            return _frontTexture;
        }
    }

    private Texture2D _backTexture;
    [Export]
    public Texture2D BackTexture
    {
        set
        {
            _backTexture = value;
            EmitSignal(SignalName.BackTextureChanged, value);
        }
        get
        {
            if (_backTexture == null)
            {
                BackTexture = CardManager.Instance.GetBackTexture(CardCategory);
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
    [Export]
    public bool IsBlocker { get; private set; }
    [Export]
    public bool IsRush { get; private set; }
    [Export]
    public bool IsDoubleAttack { get; private set; }
    [Export]
    public bool IsBanish { get; private set; }
    [Export]
    public bool IsTrigger { get; private set; }
    public bool CanGainBlocker { get; private set; }
    public bool CanGainRush { get; private set; }
    public bool CanGainDoubleAttack { get; private set; }
    public bool CanGainBanish { get; private set; }
    public bool IsEventCounter { get; private set; }
    public bool HasActivateEffect { get; private set; }

    [Signal]
    public delegate void FrontTextureChangedEventHandler(Texture2D texture);

    [Signal]
    public delegate void BackTextureChangedEventHandler(Texture2D texture);

    [Signal]
    public delegate void AskDownloadTextureEventHandler(CardResource cardResource);

    public CardCategory CardCategory
    {
        get
        {
            if (CardType == "LEADER") return CardCategory.LEADER;
            if (CardType == "CHARACTER") return CardCategory.CHARACTER;
            if (CardType == "STAGE") return CardCategory.STAGE;
            if (CardType == "EVENT") return CardCategory.EVENT;

            return CardCategory.NONE;
        }
    }

    public string GetScriptCode()
    {
        var args = Number.Split("-");
        var serie = args[0];
        var number = args[1].Split("_")[0];
        return $"{serie}-{number}";
    }

    public void StartDownloading()
    {
        if (!TextureSet)
        {
            EmitSignal(SignalName.AskDownloadTexture, this);
        }
    }

    public CardInfo Generate()
    {
        return new CardInfo(Id, Images.ToList(), Number, Rarity, CardType, Name, Cost, AttributeImage, Attribute, Power, Counter, Colors.ToList(), Types.ToList(), Effects.ToList(), Set, IsBlocker, IsRush, IsDoubleAttack, IsBanish, CanGainBlocker, CanGainRush, CanGainDoubleAttack, CanGainBanish, IsTrigger, IsEventCounter, HasActivateEffect);
    }
}
