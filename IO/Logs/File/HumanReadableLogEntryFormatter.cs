using System.Text;

namespace KB.SharpCore.IO.Logs.File;

public sealed class HumanReadableLogEntryFormatter
{
    public string Format(LogEntry entry)
    {
        StringBuilder builder = new();
        builder.Append(entry.TimestampUtc.ToString("O"));
        builder.Append(' ');
        builder.Append('[').Append(entry.Level).Append(']');
        builder.Append(' ');
        builder.Append(entry.Category);

        if (!string.IsNullOrWhiteSpace(entry.Source))
        {
            builder.Append(" (").Append(entry.Source).Append(')');
        }

        builder.Append(": ");
        builder.Append(entry.Message);
        return builder.ToString();
    }
}