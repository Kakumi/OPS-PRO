using Godot;
using Godot.Collections;

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
                FrontTexture = CardManager.Instance.GetBackTexture(CardTypeList);
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
                BackTexture = CardManager.Instance.GetBackTexture(CardTypeList);
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

    [Signal]
    public delegate void FrontTextureChangedEventHandler(Texture2D texture);

    [Signal]
    public delegate void BackTextureChangedEventHandler(Texture2D texture);

    [Signal]
    public delegate void AskDownloadTextureEventHandler(CardResource cardResource);

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

    public void StartDownloading()
    {
        if (!TextureSet)
        {
            EmitSignal(SignalName.AskDownloadTexture, this);
        }
    }
}
