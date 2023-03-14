using Godot;
using System;
using System.Globalization;
using System.IO;
using System.Linq;

public partial class AudioSettings : TabSettings
{
	public OptionButton SelectMusic { get; protected set; }
	public CheckBox BackgroundMusicCheckbox { get; protected set; }
	public HSlider BackgroundMusicSlider { get; protected set; }
	public Label BackgroundMusicSliderPercent { get; protected set; }

	private string _musicPath;

	public override void _Ready()
	{
		base._Ready();

		_musicPath = "res://app/resources/sounds/background/";

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

		var dir = DirAccess.Open(_musicPath);
		var files = dir.GetFiles(@".*\.ogg$");
		for (int i = 0; i < files.Count; i++)
		{
			var file = files[i];
			var name = Path.GetFileNameWithoutExtension(file);
			SelectMusic.AddItem(name, i);

			if (file == Settings.OriginalConfig.BackgroundMusic)
			{
				SelectMusic.Selected = i;
			}
		}
	}

	private void OnSelectMusicItemSelected(int idx)
	{
		var musicName = SelectMusic.GetItemText(idx);
		var music = Path.Combine(_musicPath, $"{musicName}.ogg");
		Settings.UpdatedConfig.BackgroundMusic = music;
		Settings.UpdatedConfig.ApplyChanges();
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
