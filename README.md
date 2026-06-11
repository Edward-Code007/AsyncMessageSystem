# AsyncMessageSystem

> **Demo project** built to showcase backend development skills using .NET 10, asynchronous messaging, and modern software engineering practices.

## Overview

AsyncMessageSystem is a REST API that simulates an order processing pipeline. When a client submits an order, the system persists it and asynchronously publishes events through a message broker. Consumers on the other end update the order state as it moves through the pipeline — all without blocking the original request.

The goal is to demonstrate how to structure a real-world async system with clean separation of concerns, proper error handling, and infrastructure managed via containers.

## Architecture

```
Client
  │
  ▼
ASP.NET Core Minimal API  ──── EF Core ────▶ PostgreSQL
  │
  │  publishes event
  ▼
RabbitMQ (via MassTransit)
  │
  ├──▶ OrderSubmittedConsumer  ──── updates status
  └──▶ OrderProcessedConsumer ──── updates status + processedAt
```

The solution is split into three projects:

| Project | Description |
|---|---|
| `AsyncMessage.Src` | Web API — endpoints, services, EF Core, consumers |
| `AsyncMessageSystem.Contracts` | Shared library — messages, DTOs, models, mappers, Result pattern |
| `AsyncMessage.Test` | Unit tests |

## Tech Stack

| Layer | Technology |
|---|---|
| Runtime | .NET 10 |
| Web Framework | ASP.NET Core Minimal API |
| Messaging | MassTransit 8 + RabbitMQ |
| ORM | Entity Framework Core 10 |
| Database | PostgreSQL |
| Infrastructure | Docker Compose |
| Testing | xUnit v3, Moq, EF Core InMemory |
| CI | GitHub Actions |

## Key Concepts Demonstrated

- **Async messaging** — orders are accepted immediately (HTTP 202) and processed asynchronously via RabbitMQ consumers
- **MassTransit** — abstracts the broker, handles consumer registration and endpoint configuration
- **Result Pattern** — custom `Result<T>` type used throughout the service and repository layers instead of throwing exceptions for expected errors
- **Minimal API** — lightweight endpoint definition without controllers
- **EF Core Migrations** — schema versioning with seed data
- **Contracts project** — shared types between producers and consumers, keeping coupling explicit
- **Unit tests** — service layer tested with Moq and an in-memory database

## Endpoints

```
POST   /api/orders          Create a new order (returns 202 Accepted)
GET    /api/orders          List all orders
GET    /api/orders/{id}     Get a single order by GUID
```

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/) (for RabbitMQ and PostgreSQL)

### 1. Start infrastructure

```bash
docker compose up -d
```

This starts:
- RabbitMQ on ports `5672` (AMQP) and `15672` (management UI)
- PostgreSQL on port `5432`

### 2. Apply migrations

```bash
dotnet ef database update --project AsyncMessage.Src
```

### 3. Run the API

```bash
dotnet run --project AsyncMessage.Src
```

OpenAPI docs are available at `/openapi/v1.json` in development mode.

### 4. Run tests

```bash
dotnet test
```

## Project Structure

```
AsyncMessageSystem/
├── AsyncMessage.Src/
│   ├── Program.cs              # Minimal API setup, DI, MassTransit config
│   ├── Services/               # OrderService + IOrderService + IOrderRepository
│   ├── Data/                   # AppDbContext
│   └── Migrations/
├── AsyncMessageSystem.Contracts/
│   ├── Messages/               # OrderSubmitted, OrderProcessed (MassTransit contracts)
│   ├── Models/                 # OrderModel, OrderStatus enum
│   ├── DTOs/                   # OrderDto, CreateOrderRequest
│   ├── Mappers/                # OrderMapper extensions
│   └── Patterns/               # Result<T>, Error
├── AsyncMessage.Test/
│   └── Services/               # OrderService unit tests
├── docker-compose.yml
└── .github/workflows/          # CI pipeline
```

## CI / CD

GitHub Actions runs on `workflow_dispatch`. The pipeline builds the solution and executes the full test suite. Concurrency is configured to cancel in-progress runs for the same actor, avoiding redundant builds.

## Author

Edward — backend developer. This project is a portfolio demo. Feel free to explore, fork, or ask questions.
