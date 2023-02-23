using Godot;

public class SoundManager : Node
{
    public AudioStreamPlayer AudioStreamPlayer { get; private set; }

    public override void _Ready()
    {
        AudioStreamPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
        AudioStreamPlayer.Play();
    }

    public void OnBackgroundMusicFinished()
    {
        AudioStreamPlayer.Play();
    }
}
