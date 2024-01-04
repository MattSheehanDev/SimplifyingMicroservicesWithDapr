using System.Text.Json;
using Dapr.Appointment.Dto;
using Microsoft.AspNetCore.Mvc;
using Dapr.Client;
using Dapr.Balance;
using Microsoft.AspNetCore.Http.HttpResults;
using Dapr.Common;
using Dapr;

var builder = WebApplication.CreateBuilder(args);

var jsonOpt = new JsonSerializerOptions()
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true,
};

// Add services to the container.
builder.Services.AddControllers().AddDapr(opt => opt.UseJsonSerializationOptions(jsonOpt));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCloudEvents();
app.MapControllers();
app.MapSubscribeHandler();

app.MapPost("v2/balance", async (AppointmentClaim claim, [FromServices] DaprClient daprClient) =>
{
    var state = await daprClient.GetStateEntryAsync<BalanceState>(Constants.StateStore, claim.PatientId.ToString());
    state.Value ??= new BalanceState {CreatedOn = DateTime.UtcNow};

    state.Value.UpdatedOn = DateTime.UtcNow;
    state.Value.Claims.Add(claim);
    state.Value.PatientId = claim.PatientId;
    await state.SaveAsync();

    return Results.Ok();
}).WithTopic(new TopicOptions
{
    PubsubName = Constants.PubSub,
    Name = Topics.OnClaimSubmitted,
    Match = "event.type ==\"balance.v2\"",
    Priority = 2
});

app.Run();
