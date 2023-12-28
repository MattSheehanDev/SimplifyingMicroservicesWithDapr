using Dapr.AppCallback.Autogen.Grpc.v1;
using Dapr.Client;
using Dapr.Client.Autogen.Grpc.v1;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace Patient_Grpc;

public class PatientService : AppCallback.AppCallbackBase
{
    private readonly ILogger<PatientService> _logger;
    private readonly DaprClient _daprClient;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="daprClient"></param>
    /// <param name="logger"></param>
    public PatientService(DaprClient daprClient, ILogger<PatientService> logger)
    {
        _daprClient = daprClient;
        _logger = logger;
    }

    public override async Task<InvokeResponse> OnInvoke(InvokeRequest request, ServerCallContext context)
    {
        Console.WriteLine($"Method {request.Method}");

        var response = new InvokeResponse();
        switch (request.Method)
        {
            case "patient":
                var patient = request.Data.Unpack<Dapr.Patient_Grpc.Generated.Item>();

                patient.Id = string.IsNullOrEmpty(patient.Id) ? Guid.NewGuid().ToString() : patient.Id;

                // Generate patient placeholder image
                var req = _daprClient.CreateInvokeMethodRequest(HttpMethod.Get, "robohash", patient.Id);
                _ = await _daprClient.InvokeMethodWithResponseAsync(req);

                response.Data = Any.Pack(patient);
                break;
            default:
                Console.WriteLine("Method not supported");
                break;
        }
        return response;
    }

    /// <inheritdoc/>
    public override Task<ListInputBindingsResponse> ListInputBindings(Empty request, ServerCallContext context)
    {
        return Task.FromResult(new ListInputBindingsResponse());
    }

    /// <inheritdoc/>
    public override Task<ListTopicSubscriptionsResponse> ListTopicSubscriptions(Empty request, ServerCallContext context)
    {
        return Task.FromResult(new ListTopicSubscriptionsResponse());
    }

    public override Task<TopicEventResponse> OnTopicEvent(TopicEventRequest request, ServerCallContext context)
    {
        return Task.FromResult(new TopicEventResponse());
    }

    public override Task<BindingEventResponse> OnBindingEvent(BindingEventRequest request, ServerCallContext context)
    {
        return Task.FromResult(new BindingEventResponse());
    }
}