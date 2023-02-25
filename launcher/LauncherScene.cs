using Godot;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

public class LauncherScene : PanelContainer
{
    private readonly string _serverUrl = "https://launcher.opbluesea.fr/opspro/";

    public Label Title { get; protected set; }
    public RichTextLabel Message { get; protected set; }
    public ProgressBar BytesProgressBar { get; protected set; }
    public ProgressBar FilesProgressBar { get; protected set; }
    public ServerConfig ServerConfig { get; protected set; }

    private string _path;
    private Queue<ServerFile> _downloadableFiles;

    public override void _Ready()
    {
        Title = GetNode<Label>("MarginContainer/HBoxContainer/Title");
        Message = GetNode<RichTextLabel>("MarginContainer/HBoxContainer/Message");
        BytesProgressBar = GetNode<ProgressBar>("MarginContainer/HBoxContainer/BytesProgressBar");
        FilesProgressBar = GetNode<ProgressBar>("MarginContainer/HBoxContainer/FilesProgressBar");

        _path = ProjectSettings.GlobalizePath("user://patches");
        System.IO.Directory.CreateDirectory(_path);

        _downloadableFiles = new Queue<ServerFile>();

        ServicePointManager.ServerCertificateValidationCallback = (a, b, c, d) => true;

        OS.WindowResizable = false;
        OS.WindowBorderless = true;
        OS.WindowMaximized = false;
        OS.WindowSize = new Vector2(960, 540);

        var screenSize = OS.GetScreenSize(0);
        var windowSize = OS.WindowSize;
        var multiplier = new Vector2(0.5f, 0.5f);

        OS.WindowPosition = screenSize * multiplier - windowSize * multiplier;

        Init();
    }

    public void Init()
    {
        try
        {
            Log.Information($"Contacting web server...");
            ChangeMessage(Tr("LAUNCHER_CONTACTING_WEB_SERVER"));

            using (var client = new WebClient())
            {
                client.DownloadStringCompleted += ServerConfigDownloaded;
                client.DownloadStringAsync(new Uri(_serverUrl));
            }
        } catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            ChangeMessage(string.Format(Tr("LAUNCHER_INIT_FAILED"), ex.Message), true);
        }
    }

    private void ServerConfigDownloaded(object sender, DownloadStringCompletedEventArgs e)
    {
        try
        {
            ServerConfig = JsonConvert.DeserializeObject<ServerConfig>(e.Result);
            ChangeMessage(Tr("LAUNCHER_CHECKING"));

            if (ServerConfig.Files.Count == 0)
            {
                Log.Error($"ServerConfig return 0 files, this is a fatal error.");
                ChangeMessage(Tr("LAUNCHER_FILES_EMPTY"));
            }
            else
            {
                _downloadableFiles = new Queue<ServerFile>(GetDownloadableFiles(ServerConfig.Files));
                Log.Information($"Found {_downloadableFiles.Count} to download ({string.Join(", ", _downloadableFiles.Select(x => x.File))}).");

                FilesProgressBar.MaxValue = _downloadableFiles.Count;
                FilesProgressBar.Value = 0;

                if (_downloadableFiles.Count > 0)
                {
                    ChangeMessage(string.Format(Tr("LAUNCHER_DOWNLOAD_MISSING"), _downloadableFiles.Count));
                    DownloadFile(_downloadableFiles.Dequeue());
                }
                else
                {
                    BytesProgressBar.MaxValue = 0;
                    FilesProgressBar.Value = 0;

                    LoadApp(ServerConfig.Files);
                }
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            ChangeMessage(string.Format(Tr("LAUNCHER_INIT_FAILED"), ex.Message), true);
        }
    }

    private void DownloadFile(ServerFile file)
    {
        var localPath = System.IO.Path.Combine(_path, file.File);
        var serverPath = System.IO.Path.Combine(_serverUrl, file.File);

        Log.Information($"Deleting " + localPath);
        if (System.IO.File.Exists(localPath))
        {
            System.IO.File.Delete(localPath);
        }

        Log.Information($"Downloading " + file.File + " to " + localPath);

        BytesProgressBar.Value = 0;

        using (var client = new WebClient())
        {
            client.DownloadProgressChanged += (s, e) => DownloadProgressChanged(s, e, file);
            client.DownloadFileCompleted += (s, e) => DownloadFileCompleted(s, e, file);
            client.DownloadFileAsync(new Uri(serverPath), localPath);
        }
    }

    private List<ServerFile> GetDownloadableFiles(List<ServerFile> files)
    {
        return files.Where(file =>
        {
            var path = System.IO.Path.Combine(_path, file.File);
            if (System.IO.File.Exists(path))
            {
                using (var md5 = MD5.Create())
                using (var stream = System.IO.File.OpenRead(path))
                {
                    var bytes = md5.ComputeHash(stream);
                    var hash = BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();

                    return hash != file.Hash;
                }
            }

            return true;
        }).ToList();
    }

    private void LoadApp(List<ServerFile> files)
    {
        try
        {
            Log.Information($"Download finished. Loading PCK files...");
            ChangeMessage(string.Format(Tr("LAUNCHER_LOADING_PCK")));

            if (!Engine.EditorHint && !OS.IsDebugBuild())
            {
                files.ForEach(file =>
                {
                    var localPath = System.IO.Path.Combine(_path, file.File);
                    var success = ProjectSettings.LoadResourcePack(localPath);

                    if (!success)
                    {
                        Log.Error($"Failed to load PCK file at {localPath}");
                    }
                });
            }

            var importedScene = ResourceLoader.Load<PackedScene>("res://app/AppInstance.tscn");
            var instance = importedScene.Instance();
            GetTree().Root.CallDeferred("add_child", instance);
            QueueFree();
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            ChangeMessage(string.Format(Tr("LAUNCHER_LOAD_FAILED"), ex.Message), true);
        }
    }

    private void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e, ServerFile serverFile)
    {
        try
        {
            double bytesIn = double.Parse(e.BytesReceived.ToString());
            double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
            double percentage = bytesIn / totalBytes * 100;

            BytesProgressBar.Value = percentage;
            ChangeMessage(string.Format(Tr("LAUNCHER_DOWNLOAD_MESSAGE"), serverFile.Name, FilesProgressBar.Value, FilesProgressBar.MaxValue));
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
        }
    }
    private void DownloadFileCompleted(object sender, AsyncCompletedEventArgs e, ServerFile serverFile)
    {
        try
        {
            Log.Information($"Download for file {serverFile.Name} is finished.");
            FilesProgressBar.Value++;

            if (_downloadableFiles.Count > 0)
            {
                DownloadFile(_downloadableFiles.Dequeue());
            }
            else
            {
                LoadApp(ServerConfig.Files);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            ChangeMessage(string.Format(Tr("LAUNCHER_DOWNLOAD_FAILED"), ex.Message), true);
        }
    }

    public void ChangeMessage(string message, bool error = false)
    {
        var sBuilder = new StringBuilder();
        sBuilder.Append("[center]");
        if (error)
        {
            sBuilder.Append("[color=red]").Append(Tr("LAUNCHER_ERROR")).Append(":").Append("[/color] ");
        }

        sBuilder.Append(message);
        sBuilder.Append("[/center]");

        if (Message.BbcodeText != sBuilder.ToString())
        {
            Message.BbcodeText = sBuilder.ToString();
        }
    }
}
