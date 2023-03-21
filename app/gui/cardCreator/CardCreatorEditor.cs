using Godot;
using Serilog;
using System;
using System.IO;
using System.Linq;

public partial class CardCreatorEditor : Container
{
    [Export]
    public CardCreatorSettings CardCreatorSettings { get; set; }

    public FileDialog FileDialog { get; private set; }

    public LineEdit FileLineEdit { get; private set; }
    public string CardFolder { get; private set; }
    public Container EditorContainer { get; private set; }
    public Container ViewerContainer { get; private set; }
    public TextureRect CustomBackground { get; private set; }
    public OptionButton ColorOptions { get; private set; }
    public OptionButton CardTypeOptions { get; private set; }
    public OptionButton AttributeOptions { get; private set; }
    public OptionButton RarityOptions { get; private set; }
    public LineEdit EditCardName { get; private set; }
    public LineEdit EditTypes { get; private set; }
    public LineEdit EditNumber { get; private set; }
    public SpinBox CostText { get; private set; }
    public SpinBox CounterText { get; private set; }
    public SpinBox PowerText { get; private set; }
    public SpinBox CardNamePx { get; private set; }
    public SpinBox NumberPx { get; private set; }
    public SpinBox TypePx { get; private set; }
    public SpinBox PowerPx { get; private set; }
    public SpinBox CounterPx { get; private set; }

    public CardTemplate CardTemplate { get; private set; }


    public override void _Ready()
	{
        CardFolder = "user://card_creator/";

        FileDialog = GetNode<FileDialog>("FileDialog");

        CustomBackground = GetNode<TextureRect>("Viewer/CardViewer/PanelContainer/MarginContainer/BoxContainer/CustomBackground");

        ColorOptions = GetNode<OptionButton>("Viewer/PanelContainer/CardEditor/MarginContainer/HBoxContainer/ColorOptions");
        CardTypeOptions = GetNode<OptionButton>("Viewer/PanelContainer/CardEditor/MarginContainer/HBoxContainer/CardTypeOptions");

        EditorContainer = GetNode<Container>("Editor");
        ViewerContainer = GetNode<Container>("Viewer/CardViewer/PanelContainer/MarginContainer/BoxContainer");

        FileLineEdit = EditorContainer.GetNode<LineEdit>("MarginContainer/ScrollContainer/VBoxContainer/CardImageContainer/MarginContainer/VBoxContainer/HBoxContainer/LineEdit");

        EditCardName = EditorContainer.GetNode<LineEdit>("MarginContainer/ScrollContainer/VBoxContainer/CardInfoContainer/MarginContainer/VBoxContainer/HBoxContainer/EditCardName");
        EditTypes = EditorContainer.GetNode<LineEdit>("MarginContainer/ScrollContainer/VBoxContainer/CardInfoContainer/MarginContainer/VBoxContainer/HBoxContainer2/EditTypes");
        EditNumber = EditorContainer.GetNode<LineEdit>("MarginContainer/ScrollContainer/VBoxContainer/CardInfoContainer/MarginContainer/VBoxContainer/HBoxContainer3/EditNumber");

        CostText = EditorContainer.GetNode<SpinBox>("MarginContainer/ScrollContainer/VBoxContainer/CardInfoContainer/MarginContainer/VBoxContainer/HBoxContainer4/CostText");
        CounterText = EditorContainer.GetNode<SpinBox>("MarginContainer/ScrollContainer/VBoxContainer/CardInfoContainer/MarginContainer/VBoxContainer/HBoxContainer5/CounterText");
        PowerText = EditorContainer.GetNode<SpinBox>("MarginContainer/ScrollContainer/VBoxContainer/CardInfoContainer/MarginContainer/VBoxContainer/HBoxContainer6/PowerText");

        CardNamePx = EditorContainer.GetNode<SpinBox>("MarginContainer/ScrollContainer/VBoxContainer/CardInfoContainer/MarginContainer/VBoxContainer/HBoxContainer/SpinBox");
        TypePx = EditorContainer.GetNode<SpinBox>("MarginContainer/ScrollContainer/VBoxContainer/CardInfoContainer/MarginContainer/VBoxContainer/HBoxContainer2/SpinBox");
        NumberPx = EditorContainer.GetNode<SpinBox>("MarginContainer/ScrollContainer/VBoxContainer/CardInfoContainer/MarginContainer/VBoxContainer/HBoxContainer3/SpinBox");
        CounterPx = EditorContainer.GetNode<SpinBox>("MarginContainer/ScrollContainer/VBoxContainer/CardInfoContainer/MarginContainer/VBoxContainer/HBoxContainer5/SpinBox");
        PowerPx = EditorContainer.GetNode<SpinBox>("MarginContainer/ScrollContainer/VBoxContainer/CardInfoContainer/MarginContainer/VBoxContainer/HBoxContainer6/SpinBox");

        AttributeOptions = EditorContainer.GetNode<OptionButton>("MarginContainer/ScrollContainer/VBoxContainer/CardInfoContainer/MarginContainer/VBoxContainer/AttributeOptions");
        RarityOptions = EditorContainer.GetNode<OptionButton>("MarginContainer/ScrollContainer/VBoxContainer/CardInfoContainer/MarginContainer/VBoxContainer/RarityOptions");

        EditorContainer.Hide();

        CostText.Prefix = Tr(CostText.Prefix);
        CounterText.Prefix = Tr(CounterText.Prefix);
        PowerText.Prefix = Tr(PowerText.Prefix);

        var screenSize = GetWindow().Size / 2;
        var popupSize = FileDialog.GetWindow().Size / 2;
        var centeredPosition = new Vector2I(screenSize.X - popupSize.X, screenSize.Y - popupSize.Y);

        FileDialog.Position = centeredPosition;

        ColorOptions.ItemSelected += ColorOptions_ItemSelected;
        CardTypeOptions.ItemSelected += CardTypeOptions_ItemSelected;
        AttributeOptions.ItemSelected += AttributeOptions_ItemSelected;
        RarityOptions.ItemSelected += RarityOptions_ItemSelected;
        EditCardName.TextChanged += CardNameChanged;
        EditTypes.TextChanged += CardTypeChanged;
        EditNumber.TextChanged += CardNumberChanged;
        CostText.ValueChanged += CostChanged;
        CounterText.ValueChanged += CounterChanged;
        PowerText.ValueChanged += PowerChanged;
        CardNamePx.ValueChanged += CardNamePxChanged;
        NumberPx.ValueChanged += NumberPxChanged;
        TypePx.ValueChanged += TypePxChanged;
        PowerPx.ValueChanged += PowerPxChanged;
        CounterPx.ValueChanged += CounterPxChanged;

        OnResetPressed();

        Directory.CreateDirectory(ProjectSettings.GlobalizePath(CardFolder));
    }

    private void CardNamePxChanged(double value)
    {
        CardTemplate?.UpdateCardTitlePx(value);
    }

    private void NumberPxChanged(double value)
    {
        CardTemplate?.UpdateCardNumberPx(value);
    }

    private void TypePxChanged(double value)
    {
        CardTemplate?.UpdateCardTypePx(value);
    }

    private void PowerPxChanged(double value)
    {
        CardTemplate?.UpdateCardPowerPx(value);
    }

    private void CounterPxChanged(double value)
    {
        CardTemplate?.UpdateCardCounterPx(value);
    }

    private void CostChanged(double value)
    {
        if (value < CardCreatorSettings.CostTextures.Count)
        {
            CardTemplate?.UpdateCardCost(CardCreatorSettings.CostTextures[(int)Math.Round(value)]);
        }
    }

    private void CounterChanged(double value)
    {
        CardTemplate?.UpdateCardCounter(value);
    }

    private void PowerChanged(double value)
    {
        CardTemplate?.UpdateCardPower(value);
    }

    private void CardNameChanged(string newText)
    {
        CardTemplate?.UpdateCardTitle(newText);
    }

    private void CardTypeChanged(string newText)
    {
        CardTemplate?.UpdateCardType(newText);
    }

    private void CardNumberChanged(string newText)
    {
        CardTemplate?.UpdateCardNumber(newText);
    }

    private void ColorOptions_ItemSelected(long index)
    {
        CardTypeOptions.Clear();
        var color = CardCreatorSettings.Colors.FirstOrDefault(x => x.Name == ColorOptions.GetItemText((int)index));
        if (color != null)
        {
            foreach (var type in color.Types)
            {
                CardTypeOptions.AddItem(type.Name);
            }
        }

        CardTypeOptions.Selected = -1;
        CardTypeOptions.Name =  Tr("CARDCREATOR_TYPE");

        RemoveCurrentTemplate();
        EditorContainer.Hide();
    }

    private void CardTypeOptions_ItemSelected(long index)
    {
        var text = CardTypeOptions.GetItemText((int) index); 
        var color = CardCreatorSettings.Colors.FirstOrDefault(x => x.Name == ColorOptions.GetItemText(ColorOptions.Selected));
        if (color != null)
        {
            var type = color.Types.FirstOrDefault(x => x.Name == text);
            UpdateTemplate(type);
        }
    }

    private void RemoveCurrentTemplate()
    {
        ViewerContainer.GetChildren().Where(x => x is CardTemplate).ToList().ForEach(x => x.QueueFree());
    }

    private void UpdateTemplate(CardCreatorCardTypeResource type)
    {
        if (type != null)
        {
            RemoveCurrentTemplate();
            CardTemplate = type.Template.Instantiate<CardTemplate>();
            ViewerContainer.AddChild(CardTemplate);

            EditCardName.Visible = type.HasTitle;
            CardNamePx.Visible = type.HasTitle;
            EditTypes.Visible = type.HasType;
            TypePx.Visible = type.HasType;
            EditNumber.Visible = type.HasNumber;
            NumberPx.Visible = type.HasNumber;
            RarityOptions.Visible = type.HasRarity;
            AttributeOptions.Visible = type.HasAttribute;
            CostText.Visible = type.HasCost;
            CounterText.Visible = type.HasCounter;
            CounterPx.Visible = type.HasCounter;
            PowerText.Visible = type.HasPower;
            PowerPx.Visible = type.HasPower;

            CardTemplate.Texture = type.Texture;
            CardNameChanged(EditCardName.Text);
            CardNamePxChanged(CardNamePx.Value);
            CardTypeChanged(EditTypes.Text);
            TypePxChanged(TypePx.Value);
            CardNumberChanged(EditNumber.Text);
            NumberPxChanged(NumberPx.Value);
            RarityOptions_ItemSelected(RarityOptions.Selected);
            AttributeOptions_ItemSelected(AttributeOptions.Selected);
            CostChanged(CostText.Value);
            CounterChanged(CounterText.Value);
            CounterPxChanged(CounterPx.Value);
            PowerChanged(PowerText.Value);
            PowerPxChanged(PowerPx.Value);
        }

        CardTemplate.Show();
        EditorContainer.Show();
    }

    private void RarityOptions_ItemSelected(long index)
    {
        var rarity = CardCreatorSettings.Rarities.FirstOrDefault(x => x.Key == RarityOptions.GetItemText((int)index));
        if (rarity.Value != null)
        {
            CardTemplate?.UpdateCardRarity(rarity.Value);
        }
    }

    private void AttributeOptions_ItemSelected(long index)
    {
        var attribute = CardCreatorSettings.Attributes.FirstOrDefault(x => x.Name == AttributeOptions.GetItemText((int)index));
        if (attribute != null)
        {
            CardTemplate?.UpdateCardAttribute(attribute.Texture);
        }
    }

    private void OnExportPressed()
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(EditCardName.Text))
            {
                var name = $"{EditCardName.Text}-{DateTime.Now:yyyyMMdd-hhmmss}.png";
                var isNewNameValid = name.IndexOfAny(Path.GetInvalidFileNameChars()) < 0;
                if (!isNewNameValid)
                {
                    throw new Exception(string.Format(Tr("CARDCREATOR_SAVE_ERROR_FILENAME"), name));
                }

                GetViewport().GetTexture().GetImage().GetRegion((Rect2I)CardTemplate.GetGlobalRect()).SavePng(Path.Combine(CardFolder, name));
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
        }
    }

	private void OnResetPressed()
	{
        try
        {
            RemoveCurrentTemplate();
            EditorContainer.Hide();

            CustomBackground.Texture = null;
            FileLineEdit.Text = null;

            ColorOptions.Clear();
            foreach (var color in CardCreatorSettings.Colors)
            {
                ColorOptions.AddItem(color.Name);
            }
            ColorOptions.Selected = -1;
            ColorOptions.Text = Tr("CARDCREATOR_COLOR");

            CardTypeOptions.Clear();

            RarityOptions.Clear();
            foreach (var color in CardCreatorSettings.Rarities)
            {
                RarityOptions.AddItem(color.Key);
            }
            RarityOptions.Selected = -1;
            RarityOptions.Text = Tr("CARDCREATOR_RARITY");

            AttributeOptions.Clear();
            foreach (var color in CardCreatorSettings.Attributes)
            {
                AttributeOptions.AddItem(color.Name);
            }
            AttributeOptions.Selected = -1;
            AttributeOptions.Text = Tr("CARDCREATOR_ATTRIBUTE");

            EditCardName.Text = null;
            CardNamePx.Value = 31;
            EditTypes.Text = null;
            TypePx.Value = 14;
            EditNumber.Text = null;
            NumberPx.Value = 11;
            CostText.Value = 0;
            CounterText.Value = 0;
            CounterPx.Value = 21;
            PowerText.Value = 0;
            PowerPx.Value = 42;
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
        }
    }

	private void OnQuitPressed()
    {
        try
        {
            var parent = this.SearchParent<CardCreator>();
            if (parent == null)
            {
                Log.Error("CardCreator not found, can't close pane.");
                //ChangeInfoMessage(Tr($"CARDCREATOR_QUIT_FAIL_PARENT"), "red");
            }
            else
            {
                parent?.QueueFree();
                AppInstance.Instance.ShowMainMenu();
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
        }
    }

    private void OnExploreFilePressed()
    {
        FileDialog.Show();
    }

    private void OnFileSelected(string path)
    {
        FileLineEdit.Text = path;

        var image = new Image();
        var error = image.Load(path);
        if (error == Error.Ok)
        {
            CustomBackground.Texture = ImageTexture.CreateFromImage(image);
        }
    }
}
