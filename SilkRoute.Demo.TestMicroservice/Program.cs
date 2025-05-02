using SilkRoute.Demo.TestMicroservice.RequestSnapshotting;
using SilkRoute.Demo.TestMicroservice.RequestSnapshotting.RequestBodyContentParsing;
using SilkRoute.Demo.TestMicroservice.RequestSnapshotting.RequestBodyContentParsing.RequestBodyContentParserStrategy;
using SilkRoute.Demo.TestMicroservice.RequestSnapshotting.RequestFormContentParsing;
using SilkRoute.Demo.TestMicroservice.RequestSnapshotting.RequestFormContentParsing.RequestFormItemContentParsing.
    RequestFormItemContentParserStrategy;
using SilkRoute.Demo.TestMicroservice.TestFilesProviding;
using SilkRoute.Public.InputFormatters;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IRequestSnapshotBuilder, RequestSnapshotBuilder>();

builder.Services.AddSingleton<IRequestBodyContentParser, RequestBodyContentParser>();

builder.Services.AddSingleton<IRequestBodyContentParserStrategy, JsonRequestBodyContentParserStrategy>();
builder.Services.AddSingleton<IRequestBodyContentParserStrategy, TextRequestBodyContentParserStrategy>();
builder.Services.AddSingleton<IRequestBodyContentParserStrategy, FileRequestBodyContentParserStrategy>();

builder.Services.AddSingleton<IRequestFormContentParser, RequestFormContentParser>();

builder.Services.AddSingleton<IRequestFormItemContentParserStrategy, FieldRequestFormItemContentParserStrategy>();
builder.Services.AddSingleton<IRequestFormItemContentParserStrategy, FileRequestFormItemContentParserStrategy>();

builder.Services.AddSingleton<ITestFileProvider, TestFileProvider>();

builder.Services
    .AddControllers(options =>
    {
        options.InputFormatters.Insert(0, new BinaryBodyInputFormatter());
    })
    .AddNewtonsoftJson();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.Use(async (context, next) =>
{
    context.Request.EnableBuffering();
    await next();
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();