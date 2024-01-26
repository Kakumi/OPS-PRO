using Godot;
using OPSProServer.Contracts.Models;
using System;

public partial class GamePlayerInfo : PanelContainer
{
    [Export]
    public NodePath PlayerAreaPath { get; set; }

    public PlayerArea PlayerArea { get; private set; }

    public Label Label { get; private set; }
    public RichTextLabel InfoMessage { get; private set; }

	public override void _Ready()
    {
        PlayerArea = GetNode<PlayerArea>(PlayerAreaPath);

        Label = GetNode<Label>("MarginContainer/VBoxContainer/Top/Label");
        InfoMessage = GetNode<RichTextLabel>("MarginContainer/VBoxContainer/Bottom/RichTextLabel");
	}

    public void UpdateMessage(string message, string color)
    {
        if (message == null)
        {
            InfoMessage.Text = null;
        } else
        {
            InfoMessage.Text = $"[center][color={color}]{Tr(message)}[/color][/center]";
        }
    }

    internal void Update(Game game, PlayerGameInformation gameInfo)
    {
        Label.Text = gameInfo.Username;
        string infoMessage = Tr("GAME_PLAYER_INFO");
        infoMessage = infoMessage.Replace("{phase_name}", Tr(gameInfo.CurrentPhase.PhaseType.GetTrKey()));
        infoMessage = infoMessage.Replace("{turn}", game.Turn + "");
        infoMessage = infoMessage.Replace("{deck_name}", gameInfo.SelectedDeck.Name);
        InfoMessage.Text = infoMessage;
    }
}
