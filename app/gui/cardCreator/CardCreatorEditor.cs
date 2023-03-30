using Godot;
using Serilog;
using System;
using System.IO;
using System.Linq;

public partial class CardCreatorEditor : Container
{
    [Export]
    public CardCreatorSettings CardCreatorSettings { get; set; }

    [Export]
    public PackedScene CardEffectEditor { get; set; }

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
    public OptionButton EffectsOptions { get; private set; }
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
    public ColorPickerButton ColorPicker { get; private set; }
    public ColorPickerButton SecondaryColorPicker { get; private set; }
    public CheckButton ToggleBGEffectButton { get; private set; }
    public Container NameContainer { get; private set; }
    public Container TypeContainer { get; private set; }
    public Container NumberContainer { get; private set; }
    public Container CostContainer { get; private set; }
    public Container CounterContainer { get; private set; }
    public Container PowerContainer { get; private set; }
    public Container CardInfoContainer { get; private set; }
    public Container CardEffectContainer { get; private set; }
    public Container EffectEditorContainer { get; private set; }
    public Container CardStyleContainer { get; private set; }
    public Container CardColorContainer { get; private set; }
    public Container CardSecondaryColorContainer { get; private set; }

    public CardTemplate CardTemplate { get; private set; }

    public CardCreatorColorResource SelectedColorResource { get; private set; }
    public CardCreatorCardTypeResource SelectedCardType { get; private set; }
    public RichTextLabel InfoMessage { get; private set; }

    public override void _Ready()
	{
        CardFolder = "user://card_creator/";

        FileDialog = GetNode<FileDialog>("FileDialog");

        CustomBackground = GetNode<TextureRect>("Viewer/CardViewer/PanelContainer/MarginContainer/BoxContainer/CustomBackground");

        ColorOptions = GetNode<OptionButton>("Viewer/PanelContainer/CardEditor/MarginContainer/HBoxContainer/ColorOptions");
        CardTypeOptions = GetNode<OptionButton>("Viewer/PanelContainer/CardEditor/MarginContainer/HBoxContainer/CardTypeOptions");

        EditorContainer = GetNode<Container>("Editor");
        ViewerContainer = GetNode<Container>("Viewer/CardViewer/PanelContainer/MarginContainer/BoxContainer");
        CardInfoContainer = EditorContainer.GetNode<Container>("MarginContainer/ScrollContainer/VBoxContainer/CardInfoContainer");
        CardEffectContainer = EditorContainer.GetNode<Container>("MarginContainer/ScrollContainer/VBoxContainer/CardEffectContainer");
        EffectEditorContainer = CardEffectContainer.GetNode<Container>("MarginContainer/VBoxContainer/ScrollContainer/EffectsEditor");
        CardStyleContainer = EditorContainer.GetNode<Container>("MarginContainer/ScrollContainer/VBoxContainer/CardStyleContainer");

        FileLineEdit = EditorContainer.GetNode<LineEdit>("MarginContainer/ScrollContainer/VBoxContainer/CardImageContainer/MarginContainer/VBoxContainer/HBoxContainer/LineEdit");

        NameContainer = CardInfoContainer.GetNode<Container>("MarginContainer/VBoxContainer/NameContainer");
        TypeContainer = CardInfoContainer.GetNode<Container>("MarginContainer/VBoxContainer/TypeContainer");
        NumberContainer = CardInfoContainer.GetNode<Container>("MarginContainer/VBoxContainer/NumberContainer");
        CostContainer = CardInfoContainer.GetNode<Container>("MarginContainer/VBoxContainer/CostContainer");
        CounterContainer = CardInfoContainer.GetNode<Container>("MarginContainer/VBoxContainer/CounterContainer");
        PowerContainer = CardInfoContainer.GetNode<Container>("MarginContainer/VBoxContainer/PowerContainer");
        CardColorContainer = CardStyleContainer.GetNode<Container>("MarginContainer/VBoxContainer/Color");
        CardSecondaryColorContainer = CardStyleContainer.GetNode<Container>("MarginContainer/VBoxContainer/SecondaryColor");

        EditCardName = NameContainer.GetNode<LineEdit>("EditCardName");
        EditTypes = TypeContainer.GetNode<LineEdit>("EditTypes");
        EditNumber = NumberContainer.GetNode<LineEdit>("EditNumber");

        CostText = CostContainer.GetNode<SpinBox>("CostText");
        CounterText = CounterContainer.GetNode<SpinBox>("CounterText");
        PowerText = PowerContainer.GetNode<SpinBox>("PowerText");

        CardNamePx = NameContainer.GetNode<SpinBox>("SpinBox");
        TypePx = TypeContainer.GetNode<SpinBox>("SpinBox");
        NumberPx = NumberContainer.GetNode<SpinBox>("SpinBox");
        CounterPx = CounterContainer.GetNode<SpinBox>("SpinBox");
        PowerPx = PowerContainer.GetNode<SpinBox>("SpinBox");

        AttributeOptions = CardInfoContainer.GetNode<OptionButton>("MarginContainer/VBoxContainer/AttributeOptions");
        RarityOptions = CardInfoContainer.GetNode<OptionButton>("MarginContainer/VBoxContainer/RarityOptions");
        EffectsOptions = CardEffectContainer.GetNode<OptionButton>("MarginContainer/VBoxContainer/HBoxContainer/EffectsOptions");

        ColorPicker = CardColorContainer.GetNode<ColorPickerButton>("ColorPicker");
        SecondaryColorPicker = CardSecondaryColorContainer.GetNode<ColorPickerButton>("ColorPicker");

        ToggleBGEffectButton = CardEffectContainer.GetNode<CheckButton>("MarginContainer/VBoxContainer/ToggleBGEffectButton");

        InfoMessage = GetNode<RichTextLabel>("Viewer/PanelContainer/CardEditor/InfoContainer/InfoMessage");

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
        InfoMessage.Text = null;

        CardTemplate?.UpdateCardTitlePx(value);
    }

    private void NumberPxChanged(double value)
    {
        InfoMessage.Text = null;

        CardTemplate?.UpdateCardNumberPx(value);
    }

    private void TypePxChanged(double value)
    {
        InfoMessage.Text = null;

        CardTemplate?.UpdateCardTypePx(value);
    }

    private void PowerPxChanged(double value)
    {
        InfoMessage.Text = null;

        CardTemplate?.UpdateCardPowerPx(value);
    }

    private void CounterPxChanged(double value)
    {
        InfoMessage.Text = null;

        CardTemplate?.UpdateCardCounterPx(value);
    }

    private void CostChanged(double value)
    {
        InfoMessage.Text = null;

        if (SelectedCardType != null && SelectedCardType.Settings.Costs != null)
        {
            var realValue = value - SelectedCardType.Settings.Costs.MinCost;
            if (realValue < SelectedCardType.Settings.Costs.Textures.Count)
            {
                CardTemplate?.UpdateCardCost(SelectedCardType.Settings.Costs.Textures[(int)realValue]);
            }
        }
    }

    private void CounterChanged(double value)
    {
        InfoMessage.Text = null;

        CardTemplate?.UpdateCardCounter(value);
    }

    private void PowerChanged(double value)
    {
        InfoMessage.Text = null;

        CardTemplate?.UpdateCardPower(value);
    }

    private void CardNameChanged(string newText)
    {
        InfoMessage.Text = null;

        CardTemplate?.UpdateCardTitle(newText);
    }

    private void CardTypeChanged(string newText)
    {
        InfoMessage.Text = null;

        CardTemplate?.UpdateCardType(newText);
    }

    private void CardNumberChanged(string newText)
    {
        InfoMessage.Text = null;

        CardTemplate?.UpdateCardNumber(newText);
    }

    private void ColorOptions_ItemSelected(long index)
    {
        try
        {
            InfoMessage.Text = null;

            Log.Information($"Color item selected");
            if (index >= 0)
            {
                CardTypeOptions.Clear();
                SelectedColorResource = CardCreatorSettings.Colors.FirstOrDefault(x => x.Name == ColorOptions.GetItemText((int)index));
                if (SelectedColorResource != null)
                {
                    Log.Information($"Update color ({SelectedColorResource.Name})");
                    foreach (var type in SelectedColorResource.Types)
                    {
                        CardTypeOptions.AddItem(type.GetFullName());
                    }
                }

                CardTypeOptions.Selected = -1;
                CardTypeOptions.Text = Tr("CARDCREATOR_TYPE");

                RemoveCurrentTemplate();
                EditorContainer.Hide();
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            ChangeInfoMessage(string.Format(Tr("GENERAL_ERROR_OCCURED"), ex.Message), "red");
        }
    }

    private void CardTypeOptions_ItemSelected(long index)
    {
        try
        {
            InfoMessage.Text = null;

            Log.Information($"Card type item selected");
            if (index >= 0 && SelectedColorResource != null)
            {
                var text = CardTypeOptions.GetItemText((int)index);
                if (SelectedColorResource != null)
                {
                    Log.Information($"Update card type ({text})");
                    SelectedCardType = SelectedColorResource.Types.FirstOrDefault(x => x.GetFullName() == text);
                    UpdateTemplate(SelectedCardType);
                }
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            ChangeInfoMessage(string.Format(Tr("GENERAL_ERROR_OCCURED"), ex.Message), "red");
        }
    }

    private void RemoveCurrentTemplate()
    {
        ViewerContainer.GetChildren().Where(x => x is CardTemplate).ToList().ForEach(x => x.QueueFree());
    }

    private void UpdateTemplate(CardCreatorCardTypeResource type)
    {
        Log.Information($"Update template");
        InfoMessage.Text = null;

        if (type != null)
        {
            Log.Debug($"Remove current template");
            RemoveCurrentTemplate();
            Log.Debug($"Add new template");
            CardTemplate = type.Settings.Template.Instantiate<CardTemplate>();
            ViewerContainer.AddChild(CardTemplate);

            CardInfoContainer.Visible = type.Settings.HasTitle || type.Settings.HasType || type.Settings.HasNumber ||
                type.Settings.HasRarity || type.Settings.HasAttribute || type.Settings.HasCost;

            Log.Debug($"Hide or Show options based on new template");
            NameContainer.Visible = type.Settings.HasTitle;
            TypeContainer.Visible = type.Settings.HasType;
            NumberContainer.Visible = type.Settings.HasNumber;
            RarityOptions.Visible = type.Settings.HasRarity;
            AttributeOptions.Visible = type.Settings.HasAttribute;
            CostContainer.Visible = type.Settings.HasCost;
            if (type.Settings.HasCost)
            {
                CostText.MinValue = type.Settings.Costs.MinCost;
                CostText.Value = type.Settings.Costs.MinCost;
                CostText.MaxValue = type.Settings.Costs.MaxCost;
            }
            CounterContainer.Visible = type.Settings.HasCounter;
            PowerContainer.Visible = type.Settings.HasPower;

            Log.Debug($"Update pixels, texts and values to the new template");
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

            CardStyleContainer.Visible = type.HasColor || type.HasSecondaryColor;

            Log.Debug($"Update colors");
            CardColorContainer.Visible = type.HasColor;
            ColorPicker.Color = type.TextColor;
            OnColorPickerColorChanged(type.TextColor);

            CardSecondaryColorContainer.Visible = type.HasSecondaryColor;
            SecondaryColorPicker.Color = type.SecondaryTextColor;
            OnColorPickerColorSecondaryChanged(type.SecondaryTextColor);

            CardEffectContainer.Visible = type.Settings.Effects?.Values != null && type.Settings.Effects.Values.Count > 0;

            ToggleBGEffectButton.Visible = type.Settings.Effects?.ToggableBackgroundColor ?? false;

            EffectEditorContainer.GetChildren().ToList().ForEach(x => x.QueueFree());

            if (CardEffectContainer.Visible)
            {
                EffectsOptions.Clear();
                foreach(var effect in type.Settings.Effects.Values)
                {
                    EffectsOptions.AddItem(effect.EffectName);
                }
                EffectsOptions.Selected = -1;
                EffectsOptions.Text = Tr("");
            }
        }

        CardTemplate.Show();
        EditorContainer.Show();
    }

    private void RarityOptions_ItemSelected(long index)
    {
        try
        {
            InfoMessage.Text = null;

            Log.Information($"Rarity item selected");
            if (index >= 0)
            {
                var rarity = CardCreatorSettings.Rarities.FirstOrDefault(x => x.Key == RarityOptions.GetItemText((int)index));
                if (rarity.Value != null)
                {
                    Log.Information($"Update rarity ({rarity})");
                    CardTemplate?.UpdateCardRarity(rarity.Value);
                }
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            ChangeInfoMessage(string.Format(Tr("GENERAL_ERROR_OCCURED"), ex.Message), "red");
        }
    }

    private void AttributeOptions_ItemSelected(long index)
    {
        try
        {
            InfoMessage.Text = null;

            Log.Information($"Attribute item selected");
            if (index >= 0)
            {
                var attribute = CardCreatorSettings.Attributes.FirstOrDefault(x => x.Name == AttributeOptions.GetItemText((int)index));
                if (attribute != null)
                {
                    Log.Information($"Update attribute ({attribute})");
                    CardTemplate?.UpdateCardAttribute(attribute.Texture);
                }
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            ChangeInfoMessage(string.Format(Tr("GENERAL_ERROR_OCCURED"), ex.Message), "red");
        }
    }

    private void OnExportPressed()
    {
        try
        {
            InfoMessage.Text = null;

            Log.Information($"Start exporting image");
            if (string.IsNullOrWhiteSpace(EditCardName.Text))
            {
                ChangeInfoMessage(Tr("CARDCREATOR_ERROR_FILL_CARDNAME"), "red");
            } else
            {
                var name = $"{EditCardName.Text}-{DateTime.Now:yyyyMMdd-hhmmss}.png";
                var isNewNameValid = name.IndexOfAny(Path.GetInvalidFileNameChars()) < 0;
                if (!isNewNameValid)
                {
                    throw new Exception(string.Format(Tr("CARDCREATOR_SAVE_ERROR_FILENAME"), name));
                }

                string outputFile = Path.Combine(CardFolder, name);
                string outputFileGlobal = ProjectSettings.GlobalizePath(outputFile);

                Log.Information($"File created and exported at {outputFileGlobal}");
                ChangeInfoMessage(string.Format(Tr("CARDCREATOR_FILE_EXPORTED"), outputFileGlobal));

                GetViewport().GetTexture().GetImage().GetRegion((Rect2I)CardTemplate.GetGlobalRect()).SavePng(outputFile);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            ChangeInfoMessage(string.Format(Tr("GENERAL_ERROR_OCCURED"), ex.Message), "red");
        }
    }

	private void OnResetPressed()
	{
        try
        {
            InfoMessage.Text = null;

            Log.Information($"Reset card creator menu");

            Log.Debug($"Hide and remove current template");

            RemoveCurrentTemplate();
            EditorContainer.Hide();

            CustomBackground.Texture = null;
            FileLineEdit.Text = null;

            Log.Debug($"Reinit color option buttons");

            ColorOptions.Clear();
            foreach (var color in CardCreatorSettings.Colors)
            {
                ColorOptions.AddItem(color.Name);
            }
            ColorOptions.Selected = -1;
            ColorOptions.Text = Tr("CARDCREATOR_COLOR");

            Log.Debug($"Clear card type options");

            CardTypeOptions.Clear();

            Log.Debug($"Reinit rarity option buttons");

            RarityOptions.Clear();
            foreach (var color in CardCreatorSettings.Rarities)
            {
                RarityOptions.AddItem(color.Key);
            }
            RarityOptions.Selected = -1;
            RarityOptions.Text = Tr("CARDCREATOR_RARITY");

            Log.Debug($"Reinit attribute option buttons");

            AttributeOptions.Clear();
            foreach (var color in CardCreatorSettings.Attributes)
            {
                AttributeOptions.AddItem(color.Name);
            }
            AttributeOptions.Selected = -1;
            AttributeOptions.Text = Tr("CARDCREATOR_ATTRIBUTE");

            Log.Debug($"Reinit default values and size");

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
            ChangeInfoMessage(string.Format(Tr("GENERAL_ERROR_OCCURED"), ex.Message), "red");
        }
    }

	private void OnQuitPressed()
    {
        try
        {
            InfoMessage.Text = null;

            Log.Information($"Quit card creator");

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
            ChangeInfoMessage(string.Format(Tr("GENERAL_ERROR_OCCURED"), ex.Message), "red");
        }
    }

    private void OnOpenFolderPressed()
    {
        try
        {
            InfoMessage.Text = null;

            var path = ProjectSettings.GlobalizePath(CardFolder);

            Log.Information($"Open folder for created cards at {path}");

            OS.ShellOpen(path);
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            ChangeInfoMessage(string.Format(Tr("GENERAL_ERROR_OCCURED"), ex.Message), "red");
        }
    }

    private void OnExploreFilePressed()
    {
        try
        {
            InfoMessage.Text = null;

            Log.Information($"Start explore file");
            FileDialog.Show();
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            ChangeInfoMessage(string.Format(Tr("GENERAL_ERROR_OCCURED"), ex.Message), "red");
        }
    }

    private void OnFileSelected(string path)
    {
        try
        {
            InfoMessage.Text = null;

            Log.Information($"File {path} selected");
            FileLineEdit.Text = path;

            var image = new Image();
            var error = image.Load(path);
            if (error == Error.Ok)
            {
                CustomBackground.Texture = ImageTexture.CreateFromImage(image);
                Log.Information($"File impoted as image");
            }
            else
            {
                Log.Error($"An error occured with this file {path}, error type: {error}");
            }
        } catch(Exception ex)
        {
            Log.Error(ex, ex.Message);
            ChangeInfoMessage(string.Format(Tr("GENERAL_ERROR_OCCURED"), ex.Message), "red");
        }
    }

    private void OnColorPickerColorChanged(Color color)
    {
        InfoMessage.Text = null;

        CardTemplate?.UpdateColor(color);
    }

    private void OnColorPickerColorSecondaryChanged(Color color)
    {
        InfoMessage.Text = null;

        CardTemplate?.UpdateSecondaryColor(color);
    }

    private void ChangeInfoMessage(string message, string color = "white")
    {
        InfoMessage.Text = $"[color={color}]{message}[/color]";
    }

    private void OnAddEffectButtonPressed()
    {
        if (SelectedCardType != null && SelectedColorResource != null)
        {
            var effectName = EffectsOptions.GetItemText(EffectsOptions.Selected);
            var effectRes = SelectedCardType.Settings.Effects.Values.FirstOrDefault(x => x.EffectName == effectName);
            if (effectRes != null)
            {
                var instance = effectRes.TemplateText.Instantiate<TemplateCardEffect>();
                var cardEffectEditor = CardEffectEditor.Instantiate<CardEffectEditor>();
                if (instance != null && cardEffectEditor != null)
                {
                    if (CardTemplate?.AddEffect(instance) ?? false)
                    {
                        cardEffectEditor.TemplateCardEffect = instance;
                        EffectEditorContainer.AddChild(cardEffectEditor);
                        cardEffectEditor.EffectName.Text = effectRes.EffectName;
                        cardEffectEditor.CardEffectDeleted += CardEffectEditor_CardEffectDeleted;
                        cardEffectEditor.ClickGoUp += CardEffectEditor_ClickGoUp;
                        cardEffectEditor.ClickGoDown += CardEffectEditor_ClickGoDown;

                        //Refresh on add
                        RefreshCardEffectEditors();
                    }
                }
            }
        }
    }

    private void CardEffectEditor_ClickGoUp(CardEffectEditor cardEffectEditor)
    {
        if (CardTemplate?.MoveUp(cardEffectEditor.TemplateCardEffect) ?? false)
        {
            var indexOf = EffectEditorContainer.GetChildren().IndexOf(cardEffectEditor);
            if (indexOf > 0)
            {
                EffectEditorContainer.MoveChild(cardEffectEditor, indexOf - 1);
                RefreshCardEffectEditors();
            }
        }
    }

    private void CardEffectEditor_ClickGoDown(CardEffectEditor cardEffectEditor)
    {
        if (CardTemplate?.MoveDown(cardEffectEditor.TemplateCardEffect) ?? false)
        {
            var indexOf = EffectEditorContainer.GetChildren().IndexOf(cardEffectEditor);
            if (indexOf < (EffectEditorContainer.GetChildCount() - 1))
            {
                EffectEditorContainer.MoveChild(cardEffectEditor, indexOf + 1);
                RefreshCardEffectEditors();
            }
        }
    }

    //Refresh on delete
    private void CardEffectEditor_CardEffectDeleted(TemplateCardEffect templateCardEffect)
    {
        RefreshCardEffectEditors();
    }

    private void RefreshCardEffectEditors()
    {
        //Bug la suppression n'est pas retirée à ce moment
        var elements = EffectEditorContainer.GetChildren().OfType<CardEffectEditor>().Where(x => !x.IsQueuedForDeletion());
        var elementsNb = elements.Count();

        foreach (var cardEffect in elements)
        {
            var indexOf = EffectEditorContainer.GetChildren().IndexOf(cardEffect);
            cardEffect.Refresh(indexOf, elementsNb);
        }
    }

    private void OnToggleBGEffect(bool active)
    {
        CardTemplate?.UpdateEffectBackgroundVisibility(active);
    }
}
