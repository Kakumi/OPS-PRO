using Godot;
using System;

public partial class GameSettings : TabSettings
{
	public LineEdit UsernameEdit { get; private set; }

	public override void _Ready()
	{
		base._Ready();
		UsernameEdit = GetNode<LineEdit>("MarginContainer/GridContainer/Username/LineEdit");
	}

    public override void Init()
    {
		UsernameEdit.Text = Settings.OriginalConfig.Username;
	}

	private void OnUsernameChanged(string username)
    {
		Settings.UpdatedConfig.Username = username;
    }
}
