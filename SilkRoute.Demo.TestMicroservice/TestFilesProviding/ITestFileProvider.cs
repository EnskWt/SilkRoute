namespace SilkRoute.Demo.TestMicroservice.TestFilesProviding;

public interface ITestFileProvider
{
    byte[] ReadTestPdfBytes();
    Stream OpenTestPdfStream();
    IFormFile CreateTestPdfFormFile();
}