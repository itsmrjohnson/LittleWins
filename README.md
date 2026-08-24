# Little Wins

Little Wins is a family gamification platform designed to encourage positive behaviours through small actions, progress, achievements, and rewards.

The initial use case is household chores, but the platform is designed around the broader concept of **activities** so it can later support homework, reading, exercise, routines, personal goals, and custom activities.

## Core Philosophy

> Small actions → Progress → Achievements → Rewards

## Current Status

The project is currently in early development.

### Milestone 1 — First Little Win

The first vertical slice is:

1. A parent creates a family.
2. A parent adds a child.
3. A parent creates an activity.
4. The activity is assigned to the child.
5. The child completes the activity.
6. The parent approves the completion.
7. The system awards and records the points.

The initial implementation uses SQL Server and Entity Framework Core.

---

## Architecture

The solution follows a clean layered architecture:

```text
LittleWins/
├── src/
│   ├── LittleWins.Api/
│   ├── LittleWins.Application/
│   ├── LittleWins.Domain/
│   └── LittleWins.Infrastructure/
│
└── tests/
    ├── LittleWins.UnitTests/
    └── LittleWins.IntegrationTests/
```

### Domain

Contains the core business concepts and rules.

The Domain layer has no dependency on ASP.NET Core, Entity Framework Core, or a specific database provider.

### Application

Contains application use cases and orchestrates business workflows.

Examples include:

- `CreateFamily`
- `AddMember`
- `CreateActivity`
- `CompleteActivity`
- `ApproveCompletion`

### Infrastructure

Contains persistence and external infrastructure concerns.

The initial implementation uses:

- Entity Framework Core
- SQL Server

Database-specific concerns remain isolated in this layer so that PostgreSQL can be introduced later.

### API

Contains HTTP/API concerns.

Controllers or endpoints should remain thin, with business logic implemented in the Application and Domain layers.

---

## Initial Domain

The first version contains:

- Families
- Members
- Activities
- Activity completions
- Approval
- Points

### Activities

Activities are the fundamental unit of the system rather than chores.

An activity can represent:

- A chore
- Homework
- Reading
- Exercise
- A routine
- A personal goal
- Another custom activity

This allows the platform to expand beyond household chores without changing the fundamental domain model.

---

## API

The initial API will expose endpoints for:

```http
POST /api/families

GET /api/families/{familyId}

POST /api/families/{familyId}/members

GET /api/families/{familyId}/members

POST /api/families/{familyId}/activities

GET /api/families/{familyId}/activities

POST /api/activities/{activityId}/complete

POST /api/completions/{completionId}/approve
```

The API should expose application use cases without embedding business rules inside the HTTP layer.

---

## Business Rules

The initial implementation includes the following rules:

1. A child can only complete an activity assigned to them.
2. A completion belongs to the relevant activity, member, and family.
3. Activities that require approval do not immediately award points.
4. Points are awarded when the completion is approved.
5. A completion cannot award points more than once.
6. A completion belongs to the same family as its activity and member.

These rules should be covered by automated tests.

---

## Technology

### Initial

- C#
- .NET
- ASP.NET Core
- Entity Framework Core
- SQL Server
- Swagger / OpenAPI
- xUnit
- FluentAssertions

### Planned

The project will evolve incrementally and may introduce:

- Authentication and authorization
- Recurring activities
- Streaks
- Levels
- Rewards
- Achievements
- Challenges
- Domain events
- Background processing
- Redis
- SignalR
- Notifications
- Testcontainers
- Docker
- GitHub Actions
- Azure
- PostgreSQL
- Logging and monitoring
- Health checks
- API versioning

These technologies will only be introduced when they solve a meaningful engineering problem.

The project deliberately avoids introducing technology purely for the sake of demonstrating it.

---

## Testing

The project focuses on testing **business behaviour** rather than simply testing HTTP status codes.

### Initial Tests

Tests should cover scenarios such as:

- Activities requiring approval do not immediately award points.
- Approving a completion awards points.
- Approving the same completion twice cannot award points twice.
- A child cannot complete another child's activity.
- Completions belong to the correct activity, member, and family.
- Points are awarded exactly once.

### Unit Tests

Unit tests focus on:

- Domain behaviour
- Business rules
- Application use cases

### Integration Tests

Integration tests verify the complete application flow against the database.

The initial integration flow is:

```text
Create Family
    ↓
Add Child
    ↓
Create Activity
    ↓
Assign Activity
    ↓
Complete Activity
    ↓
Approve Completion
    ↓
Award Points
    ↓
Persist Result
```

---

## Development

### Build the Solution

```bash
dotnet build
```

### Run the Tests

```bash
dotnet test
```

### Run the API

```bash
dotnet run --project src/LittleWins.Api
```

Swagger/OpenAPI is enabled during development.

---

## Project Goals

Little Wins is primarily a portfolio project demonstrating practical **C#/.NET backend engineering**.

The focus is on meaningful engineering decisions around:

- Domain modelling
- Business rules
- REST API design
- Dependency injection
- Entity Framework Core
- Relational database design
- Testing
- Integration testing
- Transactions
- Concurrency
- Background processing
- Caching
- Real-time communication
- Observability
- Docker
- CI/CD
- Cloud deployment
- Database portability

The project deliberately evolves incrementally rather than introducing technologies purely for the sake of demonstrating them.

---

## Engineering Principles

### Keep the Domain Independent

The Domain should not depend on:

- ASP.NET Core
- Entity Framework Core
- SQL Server
- PostgreSQL
- Infrastructure-specific implementations

The Domain should remain focused on business concepts, behaviour, and invariants.

### Keep the API Thin

HTTP endpoints should:

1. Validate and translate HTTP input where appropriate.
2. Invoke Application use cases.
3. Return appropriate HTTP responses.

They should not contain core business rules.

### Prefer Behaviour Over Anemic Models

Domain objects should own behaviour and enforce meaningful business invariants where appropriate.

### Test Business Rules

Tests should demonstrate what the system guarantees rather than simply testing implementation details.

### Introduce Complexity Deliberately

Additional infrastructure should be introduced only when the application has a real requirement for it.

For example:

- Redis should solve a genuine caching or distributed-state problem.
- SignalR should solve a real-time communication requirement.
- Background processing should be introduced when work genuinely needs to happen asynchronously.
- Domain events should be introduced when decoupled reactions to domain changes become valuable.
- PostgreSQL should be introduced when database portability or deployment requirements justify it.

### Optimise for Learning and Maintainability

The project should demonstrate production-quality engineering practices while remaining understandable and appropriately sized for a portfolio project.

---

## Future Direction

As Little Wins evolves, the platform can grow from a simple activity and points system into a broader family engagement platform.

Potential future capabilities include:

```text
Activities
    ↓
Completions
    ↓
Points
    ↓
Progress
    ↓
Achievements
    ↓
Levels
    ↓
Rewards
    ↓
Challenges
    ↓
Streaks
```

The underlying architecture should allow these capabilities to be introduced incrementally without prematurely introducing unnecessary complexity.

---

## Guiding Principle

> Build the simplest system that correctly solves the current problem, then evolve it when a real requirement justifies the next level of complexity.
