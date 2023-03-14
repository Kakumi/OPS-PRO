using Godot;
using System;
using System.Collections.Generic;

public partial class MainMenu : Control
{
    public Button DeckBuilder { get; protected set; }
    public Button Settings { get; protected set; }
    public Button Quit { get; protected set; }
    public Settings SettingsContainer { get; protected set; }

    public override void _Ready()
    {
        DeckBuilder = GetNode<Button>("PanelContainer/MarginContainer/VBoxContainer/DeckBuilder");
        Settings = GetNode<Button>("PanelContainer/MarginContainer/VBoxContainer/Settings");
        Quit = GetNode<Button>("PanelContainer/MarginContainer/VBoxContainer/Quit");
        SettingsContainer = GetNode<Settings>("Settings");

        DeckBuilder.Text = Tr(DeckBuilder.Text);
        Settings.Text = Tr(Settings.Text);
        Quit.Text = Tr(Quit.Text);
    }

    public void OnQuitPressed()
    {
        GetTree().Quit();
    }

    public void OnSettingsPressed()
    {
        SettingsContainer.Show();
    }
}
