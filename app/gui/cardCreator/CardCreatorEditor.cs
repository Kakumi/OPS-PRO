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
    public TextureRect CustomBackground { get; private set; }
    public TextureRect TemplateCard { get; private set; }
    public OptionButton ColorOptions { get; private set; }
    public OptionButton CardTypeOptions { get; private set; }


    public override void _Ready()
	{
        CardFolder = "user://card_creator/";
        CustomBackground = GetNode<TextureRect>("CardViewer/PanelContainer/MarginContainer/BoxContainer/CustomBackground");
        TemplateCard = GetNode<TextureRect>("CardViewer/PanelContainer/MarginContainer/BoxContainer/Template");

        ColorOptions = GetNode<OptionButton>("PanelContainer/CardEditor/MarginContainer/HBoxContainer/Column1/ColorOptions");
        CardTypeOptions = GetNode<OptionButton>("PanelContainer/CardEditor/MarginContainer/HBoxContainer/Column1/CardTypeOptions");

        ColorOptions.ItemSelected += ColorOptions_ItemSelected;
        CardTypeOptions.ItemSelected += CardTypeOptions_ItemSelected;

        ColorOptions.Clear();
        ColorOptions.Selected = -1;
        ColorOptions.Text = "Card color";
        foreach (var color in CardCreatorSettings.Colors)
        {
            ColorOptions.AddItem(color.Name);
        }
        ColorOptions.Selected = -1;

        Directory.CreateDirectory(ProjectSettings.GlobalizePath(CardFolder));
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
        }
    }

    private void OnExportPressed()
    {
        try
        {
            GetViewport().GetTexture().GetImage().GetRegion((Rect2I)TemplateCard.GetGlobalRect()).SavePng(Path.Combine(CardFolder, "test_export.png"));
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
        }
    }

	private void OnResetPressed()
	{

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
