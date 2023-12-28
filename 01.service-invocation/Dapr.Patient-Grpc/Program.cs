using System.Text.Json;
using Patient_Grpc;

var builder = WebApplication.CreateBuilder(args);

var jsonOpt = new JsonSerializerOptions()
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true,
};

// builder.WebHost.ConfigureKestrel(opt =>
// {
//     opt.ListenLocalhost(5002, p => p.Protocols = HttpProtocols.Http2);
// });

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddDaprClient(opt => opt.UseJsonSerializationOptions(jsonOpt));

var app = builder.Build();

app.UseRouting();
app.MapGrpcService<PatientService>();

app.Run();
