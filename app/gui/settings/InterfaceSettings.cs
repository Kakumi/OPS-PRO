using Godot;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public partial class InterfaceSettings : TabSettings
{
	public OptionButton SelectTheme { get; protected set; }
	public OptionButton SelectLang { get; protected set; }
	public OptionButton SelectBackground { get; protected set; }

	private List<string> _themePaths;
	private Dictionary<int, string> _themeFiles;
	private List<string> _backgroundPaths;
	private Dictionary<int, string> _backgroundFiles;
	private Dictionary<string, string> _languages;

	public override void _Ready()
	{
		base._Ready();

		_themePaths = new List<string>()
		{
			"res://app/resources/themes/",
			"user://resources/themes/"
		};
		_themeFiles = new Dictionary<int, string>();

		_backgroundPaths = new List<string>()
		{
			"res://app/resources/images/backgrounds/",
			"user://resources/backgrounds/"
		};
		_backgroundFiles = new Dictionary<int, string>();

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
		_themeFiles = new Dictionary<int, string>();

		SearchFiles(_themePaths, @".*_theme\.tres", SelectTheme, ref _themeFiles, SettingsManager.Instance.Config.Theme);
	}

	private void InitBackgrounds()
	{
		SelectBackground.Clear();
		_backgroundFiles = new Dictionary<int, string>();

		SearchFiles(_backgroundPaths, @".*\.jpg", SelectBackground, ref _backgroundFiles, SettingsManager.Instance.Config.Background);
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

			if (langCode == SettingsManager.Instance.Config.Language)
			{
				SelectLang.Selected = i;
			}
		}
	}

	public void OnSelectThemeItemSelected(int idx)
	{
        if (_themeFiles.ContainsKey(idx))
        {
            var file = _themeFiles[idx];
            Settings.UpdatedConfig.Theme = file;
            Settings.UpdatedConfig.ApplyChanges();
        }
    }

	public void OnSelectBackgroundItemSelected(int idx)
	{
        if (_backgroundFiles.ContainsKey(idx))
        {
            var file = _backgroundFiles[idx];
            Settings.UpdatedConfig.Background = file;
            Settings.UpdatedConfig.ApplyChanges();
        }
    }

	public void OnSelectLangItemSelected(int idx)
	{
		var langName = SelectLang.GetItemText(idx);
		var langCode = _languages[langName];
		Settings.UpdatedConfig.Language = langCode;
		Settings.UpdatedConfig.ApplyChanges();
	}
}
