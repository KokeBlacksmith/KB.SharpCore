using KB.SharpCore.Utils;

namespace KB.SharpCore.IO;

/// <summary>
/// Compatibility extensions for legacy path-based file operations expected by the editor codebase.
/// </summary>
public static class PathCompatibilityExtensions
{
    /// <summary>
    /// Copies a file asynchronously from the source path to the destination path.
    /// </summary>
    public static async Task<Result> CopyFileAsync(this Path sourceFilePath, Path destinationFilePath, int bufferSize, CancellationToken cancellationToken)
    {
        if (!sourceFilePath.IsFile)
        {
            return Result.CreateFailure($"The source path '{sourceFilePath.StringPath}' is not a file.");
        }

        if (!destinationFilePath.IsFile)
        {
            return Result.CreateFailure($"The destination path '{destinationFilePath.StringPath}' is not a file.");
        }

        if (!File.Exists(sourceFilePath.StringPath))
        {
            return Result.CreateFailure($"The source file '{sourceFilePath.StringPath}' does not exist.");
        }

        if (File.Exists(destinationFilePath.StringPath))
        {
            return Result.CreateFailure($"The destination file '{destinationFilePath.StringPath}' already exists.");
        }

        try
        {
            string? destinationDirectory = System.IO.Path.GetDirectoryName(destinationFilePath.StringPath);
            if (!string.IsNullOrWhiteSpace(destinationDirectory))
            {
                Directory.CreateDirectory(destinationDirectory);
            }

            await using FileStream sourceStream = new FileStream(sourceFilePath.StringPath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, useAsync: true);
            await using FileStream destinationStream = new FileStream(destinationFilePath.StringPath, FileMode.CreateNew, FileAccess.Write, FileShare.None, bufferSize, useAsync: true);
            await sourceStream.CopyToAsync(destinationStream, bufferSize, cancellationToken);
            await destinationStream.FlushAsync(cancellationToken);
            return Result.CreateSuccess();
        }
        catch (Exception exception)
        {
            return Result.CreateFailure(exception);
        }
    }
}