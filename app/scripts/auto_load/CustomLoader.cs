using Godot;
using Serilog;
using System;

public class CustomLoader : Node
{
    public override void _Ready()
    {
        LoadTranslations();
    }

    private void LoadTranslations()
    {
        try
        {
            var files = new Directory().GetFiles("res://app/translations/", @".*\.translation");
            files.ForEach(x =>
            {
                Log.Information($"Loading translation at {x}");
                var translation = GD.Load<Translation>(x);
                Log.Information($"Translation {translation.Locale} loaded.");
                TranslationServer.AddTranslation(translation);
            });
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"Failed to load translation {ex.Message}");
        }
    }
}