# Atya.Messaging.InMemory

In-memory messaging transport for tests, local development, and contract validation in Atya services.

[![NuGet Version](https://img.shields.io/nuget/v/Atya.Messaging.InMemory?style=for-the-badge&logo=nuget&logoColor=white&label=NuGet&color=512BD4)](https://www.nuget.org/packages/Atya.Messaging.InMemory)
[![Downloads](https://img.shields.io/nuget/dt/Atya.Messaging.InMemory?style=for-the-badge&logo=nuget&logoColor=white&label=Downloads&color=512BD4)](https://www.nuget.org/packages/Atya.Messaging.InMemory)
![.NET 10.0](https://img.shields.io/badge/.NET_10.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
[![Build](https://img.shields.io/github/actions/workflow/status/AtyaLibraries/Messaging.InMemory/ci.yml?branch=development&style=for-the-badge&logo=githubactions&logoColor=white&label=Build)](https://github.com/AtyaLibraries/Messaging.InMemory/actions)
[![License: MIT](https://img.shields.io/badge/License-MIT-512BD4?style=for-the-badge)](https://github.com/AtyaLibraries/Messaging.InMemory/blob/development/LICENSE)

## Overview

`Atya.Messaging.InMemory` is a small in-process transport that implements the `Atya.Messaging.Abstractions` publisher and consumer registry contracts. It is intended for tests, local development, samples, and contract validation before a durable or broker-backed transport is introduced.

The bus stores accepted envelopes in memory and delivers them to subscribed consumers in registration order.

## Features

- `InMemoryMessageBus<TMessage>` implements publishing and consumer registration.
- `DelegateMessageConsumer<TMessage>` adapts a delegate to the consumer contract.
- Published envelopes are retained for assertions.
- Subscriptions can be removed through `IMessageSubscription`.
- No broker, hosting, serializer, or dependency injection dependency.

## Installation

```bash
dotnet add package Atya.Messaging.InMemory
```

```powershell
Install-Package Atya.Messaging.InMemory
```

```xml
<PackageReference Include="Atya.Messaging.InMemory" Version="<latest-stable>" />
```

## Quick Start

```csharp
using Atya.Messaging.Abstractions;
using Atya.Messaging.InMemory;

var bus = new InMemoryMessageBus<string>();

await bus.SubscribeAsync(
    new DelegateMessageConsumer<string>((envelope, _) =>
    {
        Console.WriteLine(envelope.Message);
        return ValueTask.CompletedTask;
    }));

await bus.PublishAsync(
    "customer.created",
    new MessagePublishOptions(messageId: "message-1", correlationId: "correlation-1"));
```

## Feature Tour

Assert published messages in a test:

```csharp
using Atya.Messaging.InMemory;

var bus = new InMemoryMessageBus<string>();

await bus.PublishAsync("customer.created");

Console.WriteLine(bus.PublishedMessages[0].Message);
```

Remove a consumer:

```csharp
using Atya.Messaging.InMemory;

var bus = new InMemoryMessageBus<string>();
var subscription = await bus.SubscribeAsync(
    new DelegateMessageConsumer<string>((_, _) => ValueTask.CompletedTask));

await subscription.UnsubscribeAsync();
```

## Error Codes

This package does not define Result error codes. Invalid programmer input is rejected with standard .NET argument exceptions through `Atya.Foundation.Guards`.

## Why These Dependencies

- `Atya.Messaging.Abstractions` provides the publisher, consumer, subscription, and envelope contracts.
- `Atya.Foundation.Guards` provides consistent argument validation for public entry points.

## Project Structure

```text
Atya.Messaging.InMemory
|-- src/InMemory
|-- tests/InMemory.UnitTests
|-- samples/InMemory.Samples.Console
|-- benchmarks/InMemory.Benchmarks
|-- Messaging.InMemory.sln
|-- README.md
`-- LICENSE
```

## Testing

```bash
dotnet test
```

## Benchmarks

```bash
dotnet run --project benchmarks/InMemory.Benchmarks/InMemory.Benchmarks.csproj --configuration Release -- --list flat
```

## Links

- Repository: https://github.com/AtyaLibraries/Messaging.InMemory
- NuGet: https://www.nuget.org/packages/Atya.Messaging.InMemory
- Samples: https://github.com/AtyaLibraries/Messaging.InMemory/tree/development/samples
- License: https://github.com/AtyaLibraries/Messaging.InMemory/blob/development/LICENSE

## License

Released under the MIT license. See [LICENSE](LICENSE).
