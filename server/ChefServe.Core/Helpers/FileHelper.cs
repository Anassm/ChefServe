using System.Text.RegularExpressions;

public static class FileHelper
{
    public static string SanitizeFileName(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        input = Regex.Replace(input, @"[\x00-\x1F\x7F]", "");

        var invalidChars = Path.GetInvalidFileNameChars();
        input = string.Concat(input.Where(ch => !invalidChars.Contains(ch)));

        input = input.Trim(' ', '.', '\u00A0', '\u200B');

        string[] reservedNames = {
            "CON", "PRN", "AUX", "NUL",
            "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9",
            "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"
        };

        string nameWithoutExt = Path.GetFileNameWithoutExtension(input);
        string ext = Path.GetExtension(input);

        if (reservedNames.Contains(nameWithoutExt.ToUpperInvariant()))
            nameWithoutExt = "_" + nameWithoutExt;

        input = nameWithoutExt + ext;

        if (input.Length > 255)
            input = input.Substring(0, 255);

        if (string.IsNullOrWhiteSpace(input))
            input = "unnamed";

        return input;
    }
}
