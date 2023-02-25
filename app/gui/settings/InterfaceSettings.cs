using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class InterfaceSettings : TabSettings
{
    public OptionButton SelectTheme { get; protected set; }
    public OptionButton SelectLang { get; protected set; }
    public OptionButton SelectBackground { get; protected set; }

    private string _themePath;
    private string _backgroundPath;
    private Dictionary<string, string> _languages;

    public override void _Ready()
    {
        base._Ready();

        _themePath = "res://app/resources/themes/";
        _backgroundPath = "res://app/resources/images/backgrounds/";
        _languages = new Dictionary<string, string>();

        SelectTheme = GetNode<OptionButton>("MarginContainer/GridContainer/Theme/SelectTheme");
        SelectLang = GetNode<OptionButton>("MarginContainer/GridContainer/Lang/SelectLang");
        SelectBackground = GetNode<OptionButton>("MarginContainer/GridContainer/Background/SelectBackground");
    }

    public override void Init()
    {
        InitThemes();
        InitBackgrounds();
        InitLanguages();
    }

    private void InitThemes()
    {
        SelectTheme.Clear();

        var path = ProjectSettings.GlobalizePath(_themePath);
        var files = System.IO.Directory.GetFiles(path, "*_theme.tres", System.IO.SearchOption.TopDirectoryOnly).ToList();
        for(int i = 0; i < files.Count; i++)
        {
            var file = files[i];
            var name = System.IO.Path.GetFileNameWithoutExtension(file);
            SelectTheme.AddItem(name, i);

            if (file == ProjectSettings.GlobalizePath(Settings.OriginalConfig.Theme))
            {
                SelectTheme.Selected = i;
            }
        }
    }

    private void InitBackgrounds()
    {
        SelectBackground.Clear();

        var path = ProjectSettings.GlobalizePath(_backgroundPath);
        var files = System.IO.Directory.GetFiles(path, "*.jpg", System.IO.SearchOption.TopDirectoryOnly).ToList();
        for (int i = 0; i < files.Count; i++)
        {
            var file = files[i];
            var name = System.IO.Path.GetFileNameWithoutExtension(file);
            SelectBackground.AddItem(name, i);

            if (file == ProjectSettings.GlobalizePath(Settings.OriginalConfig.Background))
            {
                SelectBackground.Selected = i;
            }
        }
    }

    private void InitLanguages()
    {
        SelectLang.Clear();
        _languages = new Dictionary<string, string>();

        var langCodes = TranslationServer.GetLoadedLocales().ToList<string>();
        for (int i = 0; i < langCodes.Count; i++)
        {
            var langCode = langCodes[i];
            var name = Tr($"LANGUAGE_{langCode.ToUpper()}");
            _languages.Add(name, langCode);

            SelectLang.AddItem(name, i);

            if (langCode == Settings.OriginalConfig.Language)
            {
                SelectLang.Selected = i;
            }
        }
    }

    public void OnSelectThemeItemSelected(int idx)
    {
        var themeName = SelectTheme.GetItemText(idx);
        var theme = System.IO.Path.Combine(_themePath, $"{themeName}.tres");
        Settings.UpdatedConfig.Theme = theme;
        Settings.UpdatedConfig.ApplyChanges();
    }

    public void OnSelectBackgroundItemSelected(int idx)
    {
        var backgoundName = SelectBackground.GetItemText(idx);
        var background = System.IO.Path.Combine(_backgroundPath, $"{backgoundName}.jpg");
        Settings.UpdatedConfig.Background = background;
        Settings.UpdatedConfig.ApplyChanges();
    }

    public void OnSelectLangItemSelected(int idx)
    {
        var langName = SelectLang.GetItemText(idx);
        var langCode = _languages[langName];
        Settings.UpdatedConfig.Language = langCode;
        Settings.UpdatedConfig.ApplyChanges();
    }
}
