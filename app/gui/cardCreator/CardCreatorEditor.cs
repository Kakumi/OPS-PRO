using Godot;
using Serilog;
using System;
using System.IO;
using System.Linq;

public partial class CardCreatorEditor : VBoxContainer
{
    [Export]
    public CardCreatorSettings CardCreatorSettings { get; set; }

    public string CardFolder { get; private set; }
    public Container EditorContainer { get; private set; }
    public TextureRect CustomBackground { get; private set; }
    public TextureRect TemplateCard { get; private set; }
    public TextureRect Attribute { get; private set; }
    public TextureRect Cost { get; private set; }
    public OptionButton ColorOptions { get; private set; }
    public OptionButton CardTypeOptions { get; private set; }
    public OptionButton AttributeOptions { get; private set; }
    public OptionButton RarityOptions { get; private set; }
    public LineEdit EditCardName { get; private set; }
    public LineEdit EditTypes { get; private set; }
    public LineEdit EditNumber { get; private set; }
    public Label CardTitle { get; private set; }
    public Label Number { get; private set; }
    public Label Rarity { get; private set; }
    public Label Power { get; private set; }
    public Label Counter { get; private set; }
    public Label Type { get; private set; }
    public SpinBox CostText { get; private set; }
    public SpinBox CounterText { get; private set; }
    public SpinBox PowerText { get; private set; }
    public SpinBox CardNamePx { get; private set; }
    public SpinBox NumberPx { get; private set; }
    public SpinBox TypePx { get; private set; }
    public SpinBox PowerPx { get; private set; }
    public SpinBox CounterPx { get; private set; }


    public override void _Ready()
	{
        CardFolder = "user://card_creator/";
        CustomBackground = GetNode<TextureRect>("CardViewer/PanelContainer/MarginContainer/BoxContainer/CustomBackground");
        TemplateCard = GetNode<TextureRect>("CardViewer/PanelContainer/MarginContainer/BoxContainer/Template");
        Attribute = GetNode<TextureRect>("CardViewer/PanelContainer/MarginContainer/BoxContainer/Template/Attribute");
        Cost = GetNode<TextureRect>("CardViewer/PanelContainer/MarginContainer/BoxContainer/Template/Cost");

        ColorOptions = GetNode<OptionButton>("PanelContainer/CardEditor/MarginContainer/HBoxContainer/ColorOptions");
        CardTypeOptions = GetNode<OptionButton>("PanelContainer/CardEditor/MarginContainer/HBoxContainer/CardTypeOptions");
        AttributeOptions = GetNode<OptionButton>("PanelContainer/CardEditor/MarginContainer2/HBoxContainer/Column1/AttributeOptions");
        RarityOptions = GetNode<OptionButton>("PanelContainer/CardEditor/MarginContainer2/HBoxContainer/Column1/RarityOptions");

        EditorContainer = GetNode<Container>("PanelContainer/CardEditor/MarginContainer2");

        EditCardName = GetNode<LineEdit>("PanelContainer/CardEditor/MarginContainer2/HBoxContainer/Column2/HBoxContainer/EditCardName");
        EditTypes = GetNode<LineEdit>("PanelContainer/CardEditor/MarginContainer2/HBoxContainer/Column2/HBoxContainer2/EditTypes");
        EditNumber = GetNode<LineEdit>("PanelContainer/CardEditor/MarginContainer2/HBoxContainer/Column2/HBoxContainer3/EditNumber");

        CardTitle = TemplateCard.GetNode<Label>("CardTitle");
        Number = TemplateCard.GetNode<Label>("Number");
        Rarity = TemplateCard.GetNode<Label>("Rarity");
        Power = TemplateCard.GetNode<Label>("Power");
        Counter = TemplateCard.GetNode<Label>("Counter");
        Type = TemplateCard.GetNode<Label>("Type");

        CostText = GetNode<SpinBox>("PanelContainer/CardEditor/MarginContainer2/HBoxContainer/Column3/HBoxContainer3/CostText");
        CounterText = GetNode<SpinBox>("PanelContainer/CardEditor/MarginContainer2/HBoxContainer/Column3/HBoxContainer4/CounterText");
        PowerText = GetNode<SpinBox>("PanelContainer/CardEditor/MarginContainer2/HBoxContainer/Column3/HBoxContainer5/PowerText");

        CardNamePx = GetNode<SpinBox>("PanelContainer/CardEditor/MarginContainer2/HBoxContainer/Column2/HBoxContainer/SpinBox");
        TypePx = GetNode<SpinBox>("PanelContainer/CardEditor/MarginContainer2/HBoxContainer/Column2/HBoxContainer2/SpinBox");
        NumberPx = GetNode<SpinBox>("PanelContainer/CardEditor/MarginContainer2/HBoxContainer/Column2/HBoxContainer3/SpinBox");
        PowerPx = GetNode<SpinBox>("PanelContainer/CardEditor/MarginContainer2/HBoxContainer/Column3/HBoxContainer4/SpinBox");
        CounterPx = GetNode<SpinBox>("PanelContainer/CardEditor/MarginContainer2/HBoxContainer/Column3/HBoxContainer5/SpinBox");

        TemplateCard.Hide();
        EditorContainer.Hide();

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
        CardTitle.Set("theme_override_font_sizes/font_size", value);
    }

    private void NumberPxChanged(double value)
    {
        Number.Set("theme_override_font_sizes/font_size", value);
    }

    private void TypePxChanged(double value)
    {
        Type.Set("theme_override_font_sizes/font_size", value);
    }

    private void PowerPxChanged(double value)
    {
        Power.Set("theme_override_font_sizes/font_size", value);
    }

    private void CounterPxChanged(double value)
    {
        Counter.Set("theme_override_font_sizes/font_size", value);
    }

    private void CostChanged(double value)
    {
        if (value < CardCreatorSettings.CostTextures.Count)
        {
            Cost.Texture = CardCreatorSettings.CostTextures[(int) Math.Round(value)];
        }
    }

    private void CounterChanged(double value)
    {
        Counter.Text = value.ToString();
    }

    private void PowerChanged(double value)
    {
        Power.Text = value.ToString();
    }

    private void CardNameChanged(string newText)
    {
        CardTitle.Text = newText;
    }

    private void CardTypeChanged(string newText)
    {
        Type.Text = newText;
    }

    private void CardNumberChanged(string newText)
    {
        Number.Text = newText;
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
    }

    private void CardTypeOptions_ItemSelected(long index)
    {
        var text = CardTypeOptions.GetItemText((int) index); 
        var color = CardCreatorSettings.Colors.FirstOrDefault(x => x.Name == ColorOptions.GetItemText(ColorOptions.Selected));
        if (color != null)
        {
            var type = color.Types.FirstOrDefault(x => x.Name == text);
            if (type != null)
            {
                TemplateCard.Texture = type.Texture;
            }

            TemplateCard.Show();
            EditorContainer.Show();
        }
    }

    private void RarityOptions_ItemSelected(long index)
    {
        var rarity = CardCreatorSettings.Rarities.FirstOrDefault(x => x.Key == RarityOptions.GetItemText((int)index));
        if (rarity.Value != null)
        {
            Rarity.Text = rarity.Value;
        }
    }

    private void AttributeOptions_ItemSelected(long index)
    {
        var attribute = CardCreatorSettings.Attributes.FirstOrDefault(x => x.Name == AttributeOptions.GetItemText((int)index));
        if (attribute != null)
        {
            Attribute.Texture = attribute.Texture;
        }
    }

    private void OnExportPressed()
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(CardTitle.Text))
            {
                var name = $"{CardTitle.Text}-{DateTime.Now:yyyyMMdd-hhmmss}.png";
                var isNewNameValid = name.IndexOfAny(Path.GetInvalidFileNameChars()) < 0;
                if (!isNewNameValid)
                {
                    throw new Exception($"Name is invalid because it contains unsupported characters for a filename ({name}).");
                }

                GetViewport().GetTexture().GetImage().GetRegion((Rect2I)TemplateCard.GetGlobalRect()).SavePng(Path.Combine(CardFolder, name));
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
            TemplateCard.Hide();
            EditorContainer.Hide();

            ColorOptions.Clear();
            foreach (var color in CardCreatorSettings.Colors)
            {
                ColorOptions.AddItem(color.Name);
            }
            ColorOptions.Selected = -1;
            ColorOptions.Text = "Card color";

            CardTypeOptions.Clear();

            RarityOptions.Clear();
            foreach (var color in CardCreatorSettings.Rarities)
            {
                RarityOptions.AddItem(color.Key);
            }
            RarityOptions.Selected = -1;
            RarityOptions.Text = "Card rarity";

            AttributeOptions.Clear();
            foreach (var color in CardCreatorSettings.Attributes)
            {
                AttributeOptions.AddItem(color.Name);
            }
            AttributeOptions.Selected = -1;
            AttributeOptions.Text = "Card attribute";

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
}
