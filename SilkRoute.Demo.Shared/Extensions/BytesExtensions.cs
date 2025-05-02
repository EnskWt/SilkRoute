using System.Text;

namespace SilkRoute.Demo.Shared.Extensions;

public static class BytesExtensions
{
    public static string DecodeUtf8(this byte[] bytes)
    {
        if (bytes == null || bytes.Length == 0)
        {
            return string.Empty;
        }

        return Encoding.UTF8.GetString(bytes);
    }
}