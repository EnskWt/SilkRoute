using SilkRoute.Internal.Constants;

namespace SilkRoute.Internal.Extensions.HttpResponse;

internal static class HttpResponseMessageExtensions
{
    public static bool HasContentDisposition(this HttpResponseMessage responseMessage)
    {
        if (responseMessage is null)
        {
            throw new ArgumentNullException(nameof(responseMessage));
        }

        return responseMessage.Content.Headers.ContentDisposition is not null;
    }

    public static string GetMediaType(this HttpResponseMessage responseMessage)
    {
        if (responseMessage is null)
        {
            throw new ArgumentNullException(nameof(responseMessage));
        }

        var contentType = responseMessage.Content.Headers.ContentType;
        if (contentType is null)
        {
            return string.Empty;
        }

        return contentType.MediaType ?? string.Empty;
    }

    public static bool HasMediaType(this HttpResponseMessage responseMessage)
    {
        if (responseMessage is null)
        {
            throw new ArgumentNullException(nameof(responseMessage));
        }

        return !string.IsNullOrWhiteSpace(responseMessage.GetMediaType());
    }

    public static bool IsJsonMediaType(this HttpResponseMessage responseMessage)
    {
        if (responseMessage is null)
        {
            throw new ArgumentNullException(nameof(responseMessage));
        }

        var mediaType = responseMessage.GetMediaType();
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

    public static bool IsTextLikeMediaType(this HttpResponseMessage responseMessage)
    {
        if (responseMessage is null)
        {
            throw new ArgumentNullException(nameof(responseMessage));
        }

        var mediaType = responseMessage.GetMediaType();
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

    public static bool IsFileMediaType(this HttpResponseMessage responseMessage)
    {
        if (responseMessage is null)
        {
            throw new ArgumentNullException(nameof(responseMessage));
        }

        var mediaType = responseMessage.GetMediaType();
        if (string.IsNullOrWhiteSpace(mediaType))
        {
            return false;
        }

        if (MediaTypeConstants.FileMediaTypes.Contains(mediaType))
        {
            return true;
        }

        foreach (var fileMediaTypePrefix in MediaTypeConstants.FileMediaTypePrefixes)
        {
            if (mediaType.StartsWith(fileMediaTypePrefix, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }
}