using Godot;
using Serilog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

public partial class AudioSettings : TabSettings
{
	public OptionButton SelectMusic { get; protected set; }
	public CheckBox BackgroundMusicCheckbox { get; protected set; }
	public HSlider BackgroundMusicSlider { get; protected set; }
	public Label BackgroundMusicSliderPercent { get; protected set; }

	private List<string> _musicPaths;
	private Dictionary<int, string> _musicFiles;

	public override void _Ready()
	{
		base._Ready();

		_musicPaths = new List<string>()
		{
			"res://app/resources/sounds/background/",
			"user://resources/songs/"
		};
		_musicFiles = new Dictionary<int, string>();

		SelectMusic = GetNode<OptionButton>("MarginContainer/GridContainer/MusicSound/SelectMusic");
		BackgroundMusicCheckbox = GetNode<CheckBox>("MarginContainer/GridContainer/MusicEnabled/BackgroundMusicCheckbox");
		BackgroundMusicSlider = GetNode<HSlider>("MarginContainer/GridContainer/MusicVolume/BackgroundMusicSlider");
		BackgroundMusicSliderPercent = GetNode<Label>("MarginContainer/GridContainer/MusicVolume/BackgroundMusicSliderPercent");
	}

	public override void Init()
	{
		InitBackgroundMusic();
		BackgroundMusicCheckbox.ButtonPressed = Settings.OriginalConfig.BackgroundMusicEnabled;
		BackgroundMusicSlider.Value = Settings.OriginalConfig.BackgroundMusicVolume;
	}

	private void InitBackgroundMusic()
	{
		SelectMusic.Clear();

		SearchFiles(_musicPaths, @".*\.ogg", SelectMusic, ref _musicFiles, Settings.OriginalConfig.BackgroundMusic);
	}

	private void OnSelectMusicItemSelected(int idx)
	{
		if (_musicFiles.ContainsKey(idx))
        {
			var file = _musicFiles[idx];
			Settings.UpdatedConfig.BackgroundMusic = file;
			Settings.UpdatedConfig.ApplyChanges();
		}
    }

	private void OnBackgroundMusicSliderValueChanged(float value)
	{
		BackgroundMusicSliderPercent.Text = (value / BackgroundMusicSlider.MaxValue).ToString("P0", CultureInfo.InvariantCulture);
		Settings.UpdatedConfig.BackgroundMusicVolume = value;
		Settings.UpdatedConfig.ApplyChanges();
	}

	private void OnBackgroundMusicCheckboxToggled(bool pressed)
	{
		Settings.UpdatedConfig.BackgroundMusicEnabled = pressed;
		Settings.UpdatedConfig.ApplyChanges();
	}
}
