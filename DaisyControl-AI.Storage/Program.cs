using System.Text.Json.Serialization;
using DaisyControl_AI.Storage;
using DaisyControl_AI.Storage.Dtos.JsonConverters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services
    .AddControllers(options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true)
    .AddJsonOptions(option =>
{
    option.JsonSerializerOptions.AllowTrailingCommas = true;
    option.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    option.JsonSerializerOptions.Converters.Add(new DateTimeUnixJsonConverter());
});

builder.Services.AddEndpointsApiExplorer();

Services.ConfigureServices(builder.Services);

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
