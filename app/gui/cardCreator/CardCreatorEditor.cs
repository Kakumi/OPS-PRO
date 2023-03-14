using Godot;
using Serilog;
using System;
using System.IO;

public partial class CardCreatorEditor : VBoxContainer
{
    public string CardFolder { get; private set; }
    public TextureRect CustomBackground { get; private set; }
    public TextureRect TemplateCard { get; private set; }

	public override void _Ready()
	{
        CardFolder = "user://card_creator/";
        CustomBackground = GetNode<TextureRect>("CardViewer/PanelContainer/MarginContainer/BoxContainer/CustomBackground");
        TemplateCard = GetNode<TextureRect>("CardViewer/PanelContainer/MarginContainer/BoxContainer/Template");

        Directory.CreateDirectory(ProjectSettings.GlobalizePath(CardFolder));
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
