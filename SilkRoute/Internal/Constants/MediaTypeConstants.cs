using System.Text.RegularExpressions;

namespace SilkRoute.Internal.Constants;

internal static class MediaTypeConstants
{
    public const string Json = "application/json";

    public const string TextPrefix = "text/";

    public static readonly Regex JsonStructuredSuffixPattern =
        new(@"\+json$", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

    public static readonly Regex XmlStructuredSuffixPattern =
        new(@"\+xml$", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

    public static readonly HashSet<string> TextLikeMediaTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "application/xml",
        "application/xhtml+xml",
        "application/soap+xml",
        "application/rss+xml",
        "application/atom+xml"
    };

    public static readonly HashSet<string> FileMediaTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "application/octet-stream",
        "application/pdf",
        "application/zip",
        "application/gzip",
        "application/x-7z-compressed",
        "application/x-tar",
        "application/vnd.rar",
        "application/x-bzip2"
    };

    public static readonly HashSet<string> FileMediaTypePrefixes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/",
        "audio/",
        "video/",
        "font/"
    };
}