# 01. Service Invocation

We are creating a practice management solution for scheduling appointments for a new or existing patient.

There are two microservices as part of this solution,
- An appointment service, `Dapr.Appointment`, responsible for creating and maintaining appointment information.
- A patient service, `Dapr.Patient`, responsible for creating and maintaining patient information.

There is a third, gRPC implementation of the patient service, `Dapr.Patient-Grpc`, as an example.

## Review

1. VSCode
   - Extensions
     - Dapr extension
       - `ms-azuretools.vscode-dapr`
     - REST Client
       - `humao.rest-client`
   - Task setup
     - `launch-patient.ps1`
     - `launch-appointment.ps1`
2. Dapr init
   - `dapr init`
   - containers
     - dapr_redis
     - dapr_placement
     - dapr_zipkin
3. Service invocation to "dapr-ized" apps
   - HTTP
   - gRPC
4. Service invocation to external apps
   - `components/robohash.yaml`
5. Dapr Dashboard
   - `dapr dashboard`

