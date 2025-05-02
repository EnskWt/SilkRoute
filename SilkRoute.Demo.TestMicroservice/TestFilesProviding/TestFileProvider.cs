namespace SilkRoute.Demo.TestMicroservice.TestFilesProviding;

public sealed class TestFileProvider : ITestFileProvider
{
    private const string RelativePdfPath = "TestAssets/test.pdf";
    private const string PdfContentType = "application/pdf";

    public byte[] ReadTestPdfBytes()
    {
        var path = GetTestPdfPath();
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Test PDF not found: {path}");
        }

        return File.ReadAllBytes(path);
    }

    public Stream OpenTestPdfStream()
    {
        var path = GetTestPdfPath();
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Test PDF not found: {path}");
        }

        return File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
    }

    public IFormFile CreateTestPdfFormFile()
    {
        var bytes = ReadTestPdfBytes();
        var ms = new MemoryStream(bytes);

        return new FormFile(ms, 0, bytes.Length, "file", "test.pdf")
        {
            Headers = new HeaderDictionary(),
            ContentType = PdfContentType
        };
    }

    private static string GetTestPdfPath()
    {
        return Path.Combine(AppContext.BaseDirectory, RelativePdfPath);
    }
}