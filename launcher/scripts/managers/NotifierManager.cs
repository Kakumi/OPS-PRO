using Godot;
using System;
using System.Collections.Generic;

public class NotifierManager
{
    private static readonly Lazy<NotifierManager> _instance = new Lazy<NotifierManager>(() => new NotifierManager());
    public static NotifierManager Instance => _instance.Value;

    private readonly Dictionary<string, List<Action<object []>>> _listeners;

    public NotifierManager()
    {
        _listeners = new Dictionary<string, List<Action<object[]>>>();
    }

    public void Listen(string code, Action<object[]> action)
    {
        if (!_listeners.ContainsKey(code))
        {
            _listeners[code] = new List<Action<object[]>>();
        }

        _listeners[code].Add(action);
    }

    public void StopListener(string code, Action<object[]> action)
    {
        if (_listeners.ContainsKey(code))
        {
            _listeners[code].Remove(action);

            if (_listeners[code].Count == 0)
            {
                _listeners.Remove(code);
            }
        }
    }

    public void Send(string code, params object[] vars)
    {
        if (_listeners.ContainsKey(code))
        {
            _listeners[code].ForEach(x => x?.Invoke(vars));
        }
    }
}
