using Godot;
using Serilog;
using Serilog.Events;
using System;

public class Logger : Node
{
    public override void _Ready()
    {
        var minLevel = LogEventLevel.Information;
        if (Engine.EditorHint || OS.IsDebugBuild())
        {
            minLevel = LogEventLevel.Debug;
        }

        var path = ProjectSettings.GlobalizePath("user://logs-.logs");

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Is(minLevel)
            .WriteTo.File(path, outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff}] [{Level:u3}] {Message:lj}{NewLine}{Exception}", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 10)
            .WriteTo.Sink(new GodotSink(null))
            .CreateLogger();
    }
}
