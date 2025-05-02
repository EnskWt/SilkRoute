using Microsoft.AspNetCore.Http;
using SilkRoute.Demo.Shared.Constants;

namespace SilkRoute.Demo.Shared.Extensions;

public static class HttpRequestMediaTypeExtensions
{
    public static string GetMediaType(this HttpRequest request)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var contentType = request.ContentType;
        if (string.IsNullOrWhiteSpace(contentType))
        {
            return string.Empty;
        }

        var idx = contentType.IndexOf(';');
        return (idx >= 0 ? contentType.Substring(0, idx) : contentType).Trim();
    }

    public static bool IsJsonMediaType(this HttpRequest request)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var mediaType = request.GetMediaType();
        if (string.IsNullOrWhiteSpace(mediaType))
        {
            return false;
        }

        if (mediaType.Equals(MediaTypeConstants.Json, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return MediaTypeConstants.JsonStructuredSuffixPattern.IsMatch(mediaType);
    }

    public static bool IsTextLikeMediaType(this HttpRequest request)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var mediaType = request.GetMediaType();
        if (string.IsNullOrWhiteSpace(mediaType))
        {
            return false;
        }

        if (mediaType.StartsWith(MediaTypeConstants.TextPrefix, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (MediaTypeConstants.TextLikeMediaTypes.Contains(mediaType))
        {
            return true;
        }

        return MediaTypeConstants.XmlStructuredSuffixPattern.IsMatch(mediaType);
    }

    public static bool IsFileMediaType(this HttpRequest request)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var mediaType = request.GetMediaType();
        if (string.IsNullOrWhiteSpace(mediaType))
        {
            return false;
        }

        if (MediaTypeConstants.FileMediaTypes.Contains(mediaType))
        {
            return true;
        }

        foreach (var prefix in MediaTypeConstants.FileMediaTypePrefixes)
        {
            if (mediaType.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }
}