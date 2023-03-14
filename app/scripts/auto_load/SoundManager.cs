using Godot;

public partial class SoundManager : Node
{
    public AudioStreamPlayer AudioStreamPlayer { get; private set; }
    private static SoundManager _instance;
    public static SoundManager Instance => _instance;

    public override void _Ready()
    {
        _instance = this;

        AudioStreamPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
        AudioStreamPlayer.Play();
    }

    public void OnBackgroundMusicFinished()
    {
        AudioStreamPlayer.Play();
    }

    public void UpdateSound(string path, bool musicEnabled)
    {
        if (musicEnabled)
        {
            var newResource = GD.Load<AudioStream>(path);
            if (newResource != null && AudioStreamPlayer.Stream.ResourcePath.GetFile() != newResource.ResourcePath.GetFile())
            {
                AudioStreamPlayer.Stream = newResource;
                AudioStreamPlayer.Play();
            }
        } else
        {
            AudioStreamPlayer.Stop();
        }
    }
}
