using System.Runtime.CompilerServices;

namespace KB.SharpCore.IO;

public readonly struct Path
{
    private readonly EPathType _pathType;

    public Path(string path)
    {
        StringPath = path;
        _pathType = System.IO.Path.HasExtension(StringPath) ? EPathType.File : EPathType.Directory;

        if (!this.IsValidPath())
        {
            throw new Exception($"[Path constructor] Invalid path! Value: '{path}'");
        }
    }

    public Path()
    {
        StringPath = String.Empty;
        _pathType = EPathType.Directory;
    }

    public bool IsEmpty
    {
        get { return String.IsNullOrWhiteSpace(StringPath); }
    }

    public bool IsFile
    {
        get { return _pathType == EPathType.File; }
    }

    public bool IsDirectory
    {
        get { return _pathType == EPathType.Directory; }
    }

    public string StringPath { get; }

    public static string FullPathOrEmpty(Path? path)
    {
        return path?.StringPath ?? String.Empty;
    }
    
    public bool TryGetParentDirectory(out Path? parentPath)
    {
        DirectoryInfo? parent = Directory.GetParent(StringPath);
        if (parent != null)
        {
            parentPath = new Path(parent.FullName);
            return true;
        }

        parentPath = null;
        return false;
    }

    public static Path Empty { get; } = new Path(String.Empty);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Path Combine(params string[] paths)
    {
        if (paths == null)
        {
            throw new ArgumentNullException(nameof(paths));
        }

        bool endsWithSeparator = !System.IO.Path.HasExtension(paths[^1]) && !paths[^1].EndsWith(System.IO.Path.DirectorySeparatorChar);
        return new Path(System.IO.Path.Combine(paths) + (endsWithSeparator ? System.IO.Path.DirectorySeparatorChar.ToString() : default(string)));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Path Join(Path join1, string join2)
    {
        return Path.Combine(join1.StringPath, join2);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Path Join(Path join1, Path join2)
    {
        return Path.Combine(join1.StringPath, join2.StringPath);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Path Join(string join1, string join2)
    {
        return Path.Combine(join1, join2);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Path Combine(params Path[] paths)
    {
        return Path.Combine(paths.Select(p => p.StringPath).ToArray());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Exists()
    {
        return _pathType == EPathType.Directory ? Directory.Exists(StringPath) : File.Exists(StringPath);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsValidPath()
    {
        try
        {
            System.IO.Path.GetFullPath(this.StringPath);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string GetPath()
    {
        return StringPath;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string GetFileName()
    {
        return _pathType == EPathType.File ? System.IO.Path.GetFileName(StringPath) : String.Empty;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string? GetDirectoryName()
    {
        return System.IO.Path.GetDirectoryName(StringPath);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string? GetShortDirectoryName()
    {
        return System.IO.Path.GetFileName(System.IO.Path.GetDirectoryName(StringPath));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string GetExtension()
    {
        return System.IO.Path.GetExtension(StringPath);
    }

    public Path ConvertToDirectory()
    {
        if (this.IsFile)
        {
            return new Path(this.StringPath + System.IO.Path.DirectorySeparatorChar);
        }

        return this;
    }

    public override string ToString()
    {
        return $"Type ´{_pathType}´ Path ´{StringPath}´";
    }


    public static Path operator +(in Path path1, in Path path2)
    {
        return Path.Combine(path1.GetPath(), path2.GetPath());
    }

    public static explicit operator Path(string a)
    {
        return new Path(a);
    }

    public static explicit operator string(in Path a)
    {
        return a.GetPath() ?? String.Empty;
    }

    public static bool operator ==(in Path a, in Path b)
    {
        return a.StringPath == b.StringPath;
    }

    public static bool operator !=(in Path a, in Path b)
    {
        return !(a.StringPath == b.StringPath);
    }

    public override bool Equals(object? obj)
    {
        if (obj is Path path)
        {
            return this == path;
        }

        return false;
    }

    public override int GetHashCode()
    {
        return StringPath.GetHashCode();
    }
}