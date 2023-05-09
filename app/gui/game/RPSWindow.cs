using Godot;
using OPSProServer.Contracts.Contracts;
using Serilog;
using System;
using System.Linq;
using System.Collections.Generic;

public partial class RPSWindow : Window
{
    [Export]
    public int RandomAfter { get; set; }

    private int _elapsed;

	public Label OpponentStatus { get; private set; }
    public Label Status { get; private set; }
    public RPSButtons RPS { get; private set; }
    public Timer Timer { get; private set; }
    public Timer CloseTimer { get; private set; }

    public override void _ExitTree()
    {
        GameSocketConnector.Instance.RPSExecuted -= RPSExecuted;

        base._ExitTree();
    }

    public override void _EnterTree()
    {
        GameSocketConnector.Instance.RPSExecuted += RPSExecuted;

        base._ExitTree();
    }

    public override void _Ready()
	{
		OpponentStatus = GetNode<Label>("PanelContainer/MarginContainer/VBoxContainer/Up/Opponent/Status");
        Status = GetNode<Label>("PanelContainer/MarginContainer/VBoxContainer/Down/My/Status");
        RPS = GetNode<RPSButtons>("PanelContainer/MarginContainer/VBoxContainer/Down/My/RPSButtons");

        Timer = GetNode<Timer>("Timer");
        CloseTimer = GetNode<Timer>("CloseTimer");
    }

    public void OnVisibilityChanged()
    {
        if (Visible)
        {
            ResetTimer();
        }
    }

    private void ResetTimer()
    {
        _elapsed = 0;
        Timer.Start();

        UpdateRandomButton();
    }

    private void RPSExecuted(object sender, RockPaperScissorsResult result)
    {
        try
        {
            RPS.SetDisabled(true);

            var opponentValue = result.Signs.First(x => x.Key != GameSocketConnector.Instance.UserId);

            if (result.Winner == null)
            {
                OpponentStatus.Text = string.Format(Tr("GAME_RPS_SELECTION_OPPONENT_TIE"), Tr(opponentValue.Value.GetTrKey()));
                Status.Text = Tr("GAME_RPS_WAITING_SELECTION");
                RPS.SetDisabled(false);
                ResetTimer();
            } else if (result.Winner == GameSocketConnector.Instance.UserId)
            {
                OpponentStatus.Text = string.Format(Tr("GAME_RPS_SELECTION_OPPONENT_LOSE"), Tr(opponentValue.Value.GetTrKey()));
            } else
            {
                OpponentStatus.Text = string.Format(Tr("GAME_RPS_SELECTION_OPPONENT_WON"), Tr(opponentValue.Value.GetTrKey()));
            }
        } catch(Exception ex)
        {
            Log.Error(ex, ex.Message);
            Status.Text = string.Format(Tr("GENERAL_ERROR_OCCURED"), ex.Message);
        }
    }

    private void OnCloseTimeout()
    {
        Hide();
    }


    private async void OnClickRPS(int id)
    {
		try
        {
            Timer.Stop();
			RPS.SetDisabled(true);
            RPS.RandomButton.Text = Tr("GAME_RPS_RANDOM");

            var rps = (RockPaperScissors) id;
            Status.Text = string.Format(Tr("GAME_RPS_SELECTED"), Tr(rps.GetTrKey()).ToLower());
            var success = await GameSocketConnector.Instance.SetRockPaperScissors(rps);
			if (!success)
            {
                Log.Error("Server return error for user {UserId} and RPS id {Id}", GameSocketConnector.Instance.UserId, id);
                Status.Text = Tr("GAME_RPS_SELECTION_FAILED");
            }
        } catch(Exception ex)
        {
			Log.Error(ex, ex.Message);
			Status.Text = string.Format(Tr("GENERAL_ERROR_OCCURED"), ex.Message);
        } finally
        {
			RPS.SetDisabled(false);
        }
    }

    private void OnTimerTimeout()
    {
        _elapsed += 1;
        UpdateRandomButton();

        if (_elapsed == RandomAfter)
        {
            RPS.OnClickRandom();
        }
    }

    private void UpdateRandomButton()
    {
        RPS.RandomButton.Text = $"{Tr("GAME_RPS_RANDOM")} ({RandomAfter - _elapsed})";
    }
}
