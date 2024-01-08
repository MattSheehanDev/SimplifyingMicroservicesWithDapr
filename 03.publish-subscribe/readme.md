# 03. Publish and Subscribe

Continuing to add functionality to our practice management example

A new feature requirement has been added for our practice management solution, after a patient's appointment, we need to bill the patient and mark the appointment as closed.

We are adding a new microservice to handle patient balances called `Dapr.Balance`. The appointment service will queue a message after an appointment is closed which the balance service will read from.

## Review

1. Pubsub component
   - `components/pubsub.yaml`
2. Test and verify with Redis Streams
   - `appointment.http`
3. Content-based routing
   - change handler based off of cloudevent type
4. Switch to RabbitMQ
   - `rabbitmq.ps1`
   - `guest:guest`
