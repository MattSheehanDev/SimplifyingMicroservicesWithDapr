# 05. Actors

After running our solution in production and being a massive success, it turns out some doctors are having multiple appointments scheduled in the same time slots (must be super popular doctors).

Dapr actors can be used to manage concurrency when scheduling appointments for each doctor. A new service, `Dapr.AppointmentActor`, is added to implement the actor pattern.

## Review

1. Actor state store
   - `components/statestore.yaml`
2. Actor implementation
3. Dapr Dashboard
4. Inspect actor state
