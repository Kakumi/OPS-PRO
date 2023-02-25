using Godot;
using Serilog;
using Serilog.Configuration;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public static class Extension
{
    public static void QueueFreeAll(this Godot.Collections.Array array)
    {
        var enumerator = array.GetEnumerator();
        while (enumerator.MoveNext())
        {
            if (enumerator.Current is Node)
            {
                (enumerator.Current as Node).QueueFree();
            }
        }
    }

    public static void RemoveChildren(this Node node)
    {
        var enumerator = node.GetChildren().GetEnumerator();
        while (enumerator.MoveNext())
        {
            if (enumerator.Current is Node)
            {
                node.RemoveChild((enumerator.Current as Node));
            }
        }
    }

    public static T SearchOne<T>(this Godot.Collections.Array array) where T : Node
    {
        var enumerator = array.GetEnumerator();
        while (enumerator.MoveNext())
        {
            if (enumerator.Current is T)
            {
                return (T)enumerator.Current;
            }
            else if (enumerator.Current is Node)
            {
                var value = SearchOne<T>((enumerator.Current as Node).GetChildren());
                if (value != default(T))
                {
                    return value;
                }
            }
        }

        return default(T);
    }

    public static List<T> SearchChild<T>(this Godot.Collections.Array array)
    {
        var list = new List<T>();

        var enumerator = array.GetEnumerator();
        while (enumerator.MoveNext())
        {
            if (enumerator.Current is T)
            {
                list.Add((T)enumerator.Current);
            }
        }

        return list;
    }

    public static List<T> SearchAll<T>(this Godot.Collections.Array array)
    {
        var list = new List<T>();
        var enumerator = array.GetEnumerator();
        while (enumerator.MoveNext())
        {
            if (enumerator.Current is T)
            {
                list.Add((T)enumerator.Current);
            }
            else if (enumerator.Current is Node)
            {
                list.AddRange(SearchAll<T>((enumerator.Current as Node).GetChildren()));
            }
        }

        return list;
    }

    public static List<T> ToList<T>(this System.Array array)
    {
        var list = new List<T>();
        var enumerator = array.GetEnumerator();
        while (enumerator.MoveNext())
        {
            if (enumerator.Current is T)
            {
                list.Add((T)enumerator.Current);
            }
        }

        return list;
    }

    public static List<T> ToList<T>(this Godot.Collections.Array array)
    {
        var list = new List<T>();
        var enumerator = array.GetEnumerator();
        while (enumerator.MoveNext())
        {
            if (enumerator.Current is T)
            {
                list.Add((T)enumerator.Current);
            }
        }

        return list;
    }

    public static Node FirstOrNull(this Godot.Collections.Array array)
    {
        var enumerator = array.GetEnumerator();
        if (enumerator.MoveNext())
        {
            return enumerator.Current as Node;
        }

        return null;
    }

    public static void ConnectIfMissing(this Node node, string signal, Godot.Object target, string method, Godot.Collections.Array binds = null, uint flags = 0)
    {
        if (!node.IsConnected(signal, target, method))
        {
            node.Connect(signal, target, method, binds, flags);
        }
    }

    public static Color FromRGBA(this Color color, int r, int g, int b, float a)
    {
        color.r = r / 255f;
        color.g = g / 255f;
        color.b = b / 255f;
        color.a = a;

        return color;
    }

    public static T SearchParent<T>(this Node node) where T : Node
    {
        T result = null;
        Node parent = node;
        do
        {
            parent = parent.GetParent();
            if (parent is T)
            {
                result = parent as T;
            }
        } while (parent != null && result == null);

        return result;
    }

    public static List<string> GetFiles(this Directory directory, string path, string pattern)
    {
        List<string> files = new List<string>();
        Regex regex = new Regex(pattern);

        directory.Open(path);
        directory.ListDirBegin(true, true);
        while(true)
        {
            var file = directory.GetNext().Replace(".import", "");
            if (string.IsNullOrEmpty(file))
            {
                break;
            }

            if (regex.IsMatch(file))
            {
                files.Add(System.IO.Path.Combine(path, file));
            }
        }

        directory.ListDirEnd();

        return files;
    }
}
