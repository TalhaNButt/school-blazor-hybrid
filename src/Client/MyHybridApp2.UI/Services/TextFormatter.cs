// Services/TextFormatter.cs
using System.Net;
using System.Text.RegularExpressions;

namespace MyHybridApp2.UI.Services;

public static class TextFormatter
{
    private static readonly Regex HashtagRegex = new(@"#\w+", RegexOptions.Compiled);
    private static readonly Regex MentionRegex = new(@"@\w+", RegexOptions.Compiled);
    private static readonly Regex UrlRegex = new(@"https?://[^\s<>""']+", RegexOptions.Compiled);

    public static string ToHighlightedHtml(
        string? text,
        string hashtagColor = "#6d4aff",
        string mentionColor = "#007bff")
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        // Encode FIRST so any user-supplied markup is neutralised
        var result = WebUtility.HtmlEncode(text);

        result = HashtagRegex.Replace(result, m =>
            $"<span style=\"color: {hashtagColor}; font-weight: 500;\">{m.Value}</span>");

        result = MentionRegex.Replace(result, m =>
            $"<span style=\"color: {mentionColor}; font-weight: 500;\">{m.Value}</span>");

        result = UrlRegex.Replace(result, m =>
            $"<a href=\"{m.Value}\" target=\"_blank\" style=\"color: #6d4aff; text-decoration: underline;\">{m.Value}</a>");

        return result.Replace("\n", "<br/>");
    }
}
