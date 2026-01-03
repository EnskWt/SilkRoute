using SilkRoute.Demo.Shared.Contracts;
using SilkRoute.Public.Extensions;
using SilkRoute.Public.InputFormatters;
using SilkRoute.Public.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var settings = new MicroserviceClientOptions
{
    HttpClientConfiguration = client =>
    {
        client.BaseAddress = new Uri("https://localhost:7237");
    }
};

builder.Services.AddMicroserviceClient<ITestMicroservice>(settings);

builder.Services
    .AddControllers(options =>
    {
        options.InputFormatters.Insert(0, new BinaryBodyInputFormatter());
        options.ReturnHttpNotAcceptable = false;
    })
    .AddNewtonsoftJson();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

//// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
