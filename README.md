<div align="center" style="margin-bottom:20px">
  <img src="assets/logo.png" alt="booking-microservices" />
    <div align="center">
           <a href="https://github.com/meysamhadeli/booking-microservices/actions/workflows/ci.yml"><img alt="ci-status" src="https://github.com/meysamhadeli/booking-microservices/actions/workflows/ci.yml/badge.svg?branch=main&style=flat-square"/></a>
                 <a href="https://github.com/meysamhadeli/booking-microservices/blob/main/LICENSE"><img alt="build-status"          src="https://img.shields.io/github/license/meysamhadeli/booking-microservices?color=%234275f5&style=flat-square"/></a>
    </div>
</div>

> 🚀 **A practical and imaginary microservices for implementing an infrastructure for up and running distributed system with the latest technology and architecture like Vertical Slice Architecture, Event Sourcing, CQRS, DDD, gRpc, MongoDB, RabbitMq, Masstransit in .Net 9.**

> 💡 **This project is not business-oriented and most of my focus was in the thechnical part for implement a distributed system with a sample project. In this project I implemented some concept in microservices like Messaging, Tracing, Event Driven Architecture, Vertical Slice Architecture, Event Sourcing, CQRS, DDD and gRpc.**


<div>
  <a href="https://gitpod.io/#https://github.com/meysamhadeli/booking-microservices"><img alt="Open in Gitpod" src="https://gitpod.io/button/open-in-gitpod.svg"/></a>
</div>
<div>
  <a href='https://codespaces.new/meysamhadeli/booking-microservices?quickstart=1'><img alt='Open in GitHub Codespaces' src='https://github.com/codespaces/badge.svg'></a>
</div>

# Table of Contents

- [The Goals of This Project](#the-goals-of-this-project)
- [Technologies - Libraries](#technologies---libraries)
- [Key Features](#key-features)
- [When to Use](#when-to-use)
- [Challenges](#challenges)
- [The Domain and Bounded Context - Service Boundary](#the-domain-and-bounded-context---service-boundary)
- [Structure of Project](#structure-of-project)
- [Development Setup](#development-setup)
    - [Dotnet Tools Packages](#dotnet-tools-packages)
    - [Husky](#husky)
    - [Upgrade Nuget Packages](#upgrade-nuget-packages)
- [How to Run](#how-to-run)
  - [Config Certificate](#config-certificate)
  - [Docker Compose](#docker-compose)
  - [Kubernetes](#kubernetes)
  - [Build](#build)
  - [Run](#run)
  - [Test](#test)
- [Documentation Apis](#documentation-apis)
- [Support](#support)
- [Contribution](#contribution)


## The Goals of This Project

- :sparkle: Using `Vertical Slice Architecture` for `architecture` level.
- :sparkle: Using `Domain Driven Design (DDD)` to implement all `business logic`.
- :sparkle: Using `Rabbitmq` on top of `Masstransit` for `Event Driven Architecture`.
- :sparkle: Using `gRPC` for `internal communication`.
- :sparkle: Using `CQRS` implementation with `MediatR` library.
- :sparkle: Using `Postgres` for `write side` database.
- :sparkle: Using `MongoDB` for `read side` database.
- :sparkle: Using `Event Store` for `write side` of Booking Microservice/Module to store all `historical change` of aggregate.
- :sparkle: Using `Inbox Pattern` for ensuring message idempotency for receiver and `Exactly once Delivery`.
- :sparkle: Using `Outbox Pattern` for ensuring no message is lost and there is at `At Least One Delivery`.
- :sparkle: Using `Unit Testing` for testing small units and mocking our dependencies with `Nsubstitute`.
- :sparkle: Using `End-To-End Testing` and `Integration Testing` for testing `features` with all dependencies using `testcontainers`.
- :sparkle: Using `Fluent Validation` and a `Validation Pipeline Behaviour` on top of `MediatR`.
- :sparkle: Using `Minimal API` for all endpoints.
- :sparkle: Using `AspNetCore OpenApi` for `generating` built-in support `OpenAPI documentation` in ASP.NET Core.
- :sparkle: Using `Health Check` for `reporting` the `health` of app infrastructure components.
- :sparkle: Using `Docker-Compose` and `Kubernetes` for our deployment mechanism.
- :sparkle: Using `Kibana` on top of `Serilog` for `logging`.
- :sparkle: Using `OpenTelemetry` for distributed tracing on top of `Jaeger`.
- :sparkle: Using `OpenTelemetry` for monitoring on top of `Prometheus` and `Grafana`.
- :sparkle: Using `IdentityServer` for authentication and authorization base on `OpenID-Connect` and `OAuth2`.
- :sparkle: Using `Yarp` as a microservices `gateway`.
- :sparkle: Using `Kubernetes` to achieve efficient `scaling` and ensure `high availability` for each of our microservices.
- :sparkle: Using `Nginx Ingress Controller` for `load balancing` between our microservices top of `Kubernetes`.
- :sparkle: Using `cert-manager` to Configure `TLS` in `kubernetes cluster`.


## Technologies - Libraries

- ✔️ **[`.NET 9`](https://github.com/dotnet/aspnetcore)** - .NET Framework and .NET Core, including ASP.NET and ASP.NET Core.
- ✔️ **[`MVC Versioning API`](https://github.com/microsoft/aspnet-api-versioning)** - Set of libraries which add service API versioning to ASP.NET Web API, OData with ASP.NET Web API, and ASP.NET Core.
- ✔️ **[`EF Core`](https://github.com/dotnet/efcore)** - Modern object-database mapper for .NET. It supports LINQ queries, change tracking, updates, and schema migrations.
- ✔️ **[`AspNetCore OpenApi`](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/openapi/aspnetcore-openapi?view=aspnetcore-9.0&tabs=visual-studio#configure-openapi-document-generation)** - Provides built-in support for OpenAPI document generation in ASP.NET Core.
- ✔️ **[`Masstransit`](https://github.com/MassTransit/MassTransit)** - Distributed Application Framework for .NET.
- ✔️ **[`MediatR`](https://github.com/jbogard/MediatR)** - Simple, unambitious mediator implementation in .NET.
- ✔️ **[`FluentValidation`](https://github.com/FluentValidation/FluentValidation)** - Popular .NET validation library for building strongly-typed validation rules.
- ✔️ **[`Scalar`](https://github.com/scalar/scalar/tree/main/packages/scalar.aspnetcore)** - Scalar provides an easy way to render beautiful API references based on OpenAPI/Swagger documents.
- ✔️ **[`Swagger UI`](https://github.com/domaindrivendev/Swashbuckle.AspNetCore)** - Swagger tools for documenting API's built on ASP.NET Core.
- ✔️ **[`Serilog`](https://github.com/serilog/serilog)** - Simple .NET logging with fully-structured events
- ✔️ **[`Polly`](https://github.com/App-vNext/Polly)** - Polly is a .NET resilience and transient-fault-handling library that allows developers to express policies such as Retry, Circuit Breaker, Timeout, Bulkhead Isolation, and Fallback in a fluent and thread-safe manner.
- ✔️ **[`Scrutor`](https://github.com/khellang/Scrutor)** - Assembly scanning and decoration extensions for Microsoft.Extensions.DependencyInjection
- ✔️ **[`Opentelemetry-dotnet`](https://github.com/open-telemetry/opentelemetry-dotnet)** - The OpenTelemetry .NET Client
- ✔️ **[`DuendeSoftware IdentityServer`](https://github.com/DuendeSoftware/IdentityServer)** - The most flexible and standards-compliant OpenID Connect and OAuth 2.x framework for ASP.NET Core.
- ✔️ **[`EasyCaching`](https://github.com/dotnetcore/EasyCaching)** - Open source caching library that contains basic usages and some advanced usages of caching which can help us to handle caching more easier.
- ✔️ **[`Mapster`](https://github.com/MapsterMapper/Mapster)** - Convention-based object-object mapper in .NET.
- ✔️ **[`Hellang.Middleware.ProblemDetails`](https://github.com/khellang/Middleware/tree/master/src/ProblemDetails)** - A middleware for handling exception in .Net Core.
- ✔️ **[`NewId`](https://github.com/phatboyg/NewId)** - NewId can be used as an embedded unique ID generator that produces 128 bit (16 bytes) sequential IDs.
- ✔️ **[`Yarp`](https://github.com/microsoft/reverse-proxy)** - Reverse proxy toolkit for building fast proxy servers in .NET.
- ✔️ **[`Tye`](https://github.com/dotnet/tye)** - Developer tool that makes developing, testing, and deploying microservices and distributed applications easier.
- ✔️ **[`gRPC-dotnet`](https://github.com/grpc/grpc-dotnet)** - gRPC functionality for .NET.
- ✔️ **[`EventStore`](https://github.com/EventStore/EventStore)** - The open-source, functional database with Complex Event Processing.
- ✔️ **[`MongoDB.Driver`](https://github.com/mongodb/mongo-csharp-driver)** - .NET Driver for MongoDB.
- ✔️ **[`xUnit.net`](https://github.com/xunit/xunit)** - A free, open source, community-focused unit testing tool for the .NET Framework.
- ✔️ **[`Respawn`](https://github.com/jbogard/Respawn)** - Respawn is a small utility to help in resetting test databases to a clean state.
- ✔️ **[`Testcontainers`](https://github.com/testcontainers/testcontainers-dotnet)** - Testcontainers for .NET is a library to support tests with throwaway instances of Docker containers.
- ✔️ **[`K6`](https://github.com/grafana/k6)** - Modern load testing for developers and testers in the DevOps era.


## Key Features
1. **Independent Services**: Each service is a separate project with its own database and deployment pipeline, enabling independent development and deployment.
2. **Decentralized Communication**: Services communicate via APIs (REST, gRPC) or message brokers (RabbitMQ, Kafka), ensuring loose coupling and resilience.
3. **Scalability**: Services can be scaled independently based on demand, allowing efficient resource utilization.
4. **Fault Tolerance**: Failures are isolated, preventing cascading failures and ensuring high availability.
5. **Technology Agnostic**: Services can use different technologies, frameworks, or databases, providing flexibility.


## When to Use
1. **Large and Complex Projects**: Ideal for applications with complex business logic that can be broken into smaller, manageable services.
2. **High Scalability Needs**: Suitable for applications requiring independent scaling of components.
3. **Fault Tolerance and High Availability**: Perfect for systems where failure isolation and uptime are critical.
4. **Distributed Teams**: Enables teams to work independently on different services.
5. **Frequent Updates**: Supports continuous deployment and A/B testing for individual services.
6. **Technology Diversity**: Allows the use of different technologies for different services.


## Challenges
- Increased complexity in management, DevOps overhead, data consistency, latency, and higher costs.


## The Domain And Bounded Context - Service Boundary

- `Identity Service`: The Identity Service is a bounded context for the authentication and authorization of users using [Identity Server](https://github.com/DuendeSoftware/IdentityServer). This service is responsible for creating new users and their corresponding roles and permissions using [.Net Core Identity](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity) and Jwt authentication and authorization.

- `Flight Service`: The Flight Service is a bounded context `CRUD` service to handle flight related operations.

- `Passenger Service`: The Passenger Service is a bounded context for managing passenger information, tracking activities and subscribing to get notification for out of stock products.

- `Booking Service`: The Booking Service is a bounded context for managing all operation related to booking ticket.

![](./assets/booking-microservices.png)


## Structure of Project

In this project, I used [vertical slice architecture](https://jimmybogard.com/vertical-slice-architecture/) at the architectural level and [feature folder structure](http://www.kamilgrzybek.com/design/feature-folders/) to structure my files.

I treat each request as a distinct use case or slice, encapsulating and grouping all concerns from front-end to back.
When adding or changing a feature in an application in n-tire architecture, we are typically touching many "layers" in an application. We are changing the user interface, adding fields to models, modifying validation, and so on. Instead of coupling across a layer, we couple vertically along a slice. We `minimize coupling` `between slices`, and `maximize coupling` `in a slice`.

With this approach, each of our vertical slices can decide for itself how to best fulfill the request. New features only add code, we're not changing shared code and worrying about side effects.

<div align="center">
  <img src="./assets/vertical-slice-architecture.png" />
</div>

Instead of grouping related action methods in one controller, as found in traditional ASP.net controllers, I used the [REPR pattern](https://deviq.com/design-patterns/repr-design-pattern). Each action gets its own small endpoint, consisting of a route, the action, and an `IMediator` instance (see [MediatR](https://github.com/jbogard/MediatR)). The request is passed to the `IMediator` instance, routed through a [`Mediatr pipeline`](https://lostechies.com/jimmybogard/2014/09/09/tackling-cross-cutting-concerns-with-a-mediator-pipeline/) where custom [middleware](https://github.com/jbogard/MediatR/wiki/Behaviors) can log, validate and intercept requests. The request is then handled by a request specific `IRequestHandler` which performs business logic before returning the result.

The use of the [mediator pattern](https://dotnetcoretutorials.com/2019/04/30/the-mediator-pattern-in-net-core-part-1-whats-a-mediator/) in my controllers creates clean and [thin controllers](https://codeopinion.com/thin-controllers-cqrs-mediatr/). By separating action logic into individual handlers we support the [Single Responsibility Principle](https://en.wikipedia.org/wiki/Single_responsibility_principle) and [Don't Repeat Yourself principles](https://en.wikipedia.org/wiki/Don%27t_repeat_yourself), this is because traditional controllers tend to become bloated with large action methods and several injected `Services` only being used by a few methods.

I used CQRS to decompose my features into small parts that makes our application:

- Maximize performance, scalability and simplicity.
- Easy to maintain and add features to. Changes only affect one command or query, avoiding breaking changes or creating side effects.
- It gives us better separation of concerns and cross-cutting concern (with help of mediatr behavior pipelines), instead of bloated service classes doing many things.

Using the CQRS pattern, we cut each business functionality into vertical slices, for each of these slices we group classes (see [technical folders structure](http://www.kamilgrzybek.com/design/feature-folders)) specific to that feature together (command, handlers, infrastructure, repository, controllers, etc). In our CQRS pattern each command/query handler is a separate slice. This is where you can reduce coupling between layers. Each handler can be a separated code unit, even copy/pasted. Thanks to that, we can tune down the specific method to not follow general conventions (e.g. use custom SQL query or even different storage). In a traditional layered architecture, when we change the core generic mechanism in one layer, it can impact all methods.


## Development Setup

### Dotnet Tools Packages
For installing our requirement packages with .NET cli tools, we need to install `dotnet tool manifest`.
```bash
dotnet new tool-manifest
```
And after that we can restore our dotnet tools packages with .NET cli tools from `.config` folder and `dotnet-tools.json` file.
```
dotnet tool restore
```

### Husky
Here we use `husky` to handel some pre commit rules and we used `conventional commits` rules and `formatting` as pre commit rules, here in [package.json](.././package.json). of course, we can add more rules for pre commit in future. (find more about husky in the [documentation](https://typicode.github.io/husky/get-started.html))
We need to install `husky` package for `manage` `pre commits hooks` and also I add two packages `@commitlint/cli` and `@commitlint/config-conventional` for handling conventional commits rules in [package.json](.././package.json).
Run the command bellow in the root of project to install all npm dependencies related to husky:

```bash
npm install
```

> Note: In the root of project we have `.husky` folder and it has `commit-msg` file for handling conventional commits rules with provide user friendly message and `pre-commit` file that we can run our `scripts` as a `pre-commit` hooks. that here we call `format` script from [package.json](./package.json) for formatting purpose.

### Upgrade Nuget Packages
For upgrading our nuget packages to last version, we use the great package [dotnet-outdated](https://github.com/dotnet-outdated/dotnet-outdated).
Run the command below in the root of project to upgrade all of packages to last version:
```bash
dotnet outdated -u
```

## How to Run

> ### Config Certificate
Run the following commands to [Config SSL](https://docs.microsoft.com/en-us/aspnet/core/security/docker-compose-https?view=aspnetcore-6.0) in your system:

#### Windows using Linux containers
```bash
dotnet dev-certs https -ep %USERPROFILE%\.aspnet\https\aspnetapp.pfx -p password
dotnet dev-certs https --trust
```
***Note:** for running this command in `powershell` use `$env:USERPROFILE` instead of `%USERPROFILE%`*

#### macOS or Linux
```bash
dotnet dev-certs https -ep ${HOME}/.aspnet/https/aspnetapp.pfx -p $CREDENTIAL_PLACEHOLDER$
dotnet dev-certs https --trust
```
> ### Docker Compose


To run this app in `Docker`, use the [docker-compose.yaml](./deployments/docker-compose/docker-compose.yaml) and execute the below command at the `root` of the application:

```bash
docker-compose -f ./deployments/docker-compose/docker-compose.yaml up -d
```

> ### Kubernetes
To `configure TLS` in the `Kubernetes cluster`, we need to install `cert-manager` based on the [docs](https://cert-manager.io/docs/installation) and run the following commands to apply TLS in our application. Here, we use [Let's Encrypt](https://letsencrypt.org/) to encrypt our certificate.

```bash
kubectl apply -f ./deployments/kubernetes/booking-cert-manager.yml
```

To apply all necessary `deployments`, `pods`, `services`, `ingress`, and `config maps`, please run the following command:

```bash
kubectl apply -f ./deployments/kubernetes/booking-microservices.yml
```

> ### Build
To `build` all microservices, run this command in the `root` of the project:
```bash
dotnet build
```

> ### Run
To `run` each microservice, run this command in the root of the `Api` folder of each microservice where the `csproj` file is located:
```bash
dotnet run
```

> ### Test

To `test` all microservices, run this command in the `root` of the project:
```bash
dotnet test
```

> ### Documentation Apis

Each microservice provides `API documentation` and navigate to `/swagger` for `Swagger OpenAPI` or `/scalar/v1` for `Scalar OpenAPI` to visit list of endpoints.

As part of API testing, I created the [booking.rest](./booking.rest) file which can be run with the [REST Client](https://github.com/Huachao/vscode-restclient) `VSCode plugin`.

# Support

If you like my work, feel free to:

- ⭐ this repository. And we will be happy together :)

Thanks a bunch for supporting me!

## Contribution

Thanks to all [contributors](https://github.com/meysamhadeli/booking-microservices/graphs/contributors), you're awesome and this wouldn't be possible without you! The goal is to build a categorized, community-driven collection of very well-known resources.

Please follow this [contribution guideline](./CONTRIBUTION.md) to submit a pull request or create the issue.

## Project References & Credits

- [https://github.com/jbogard/ContosoUniversityDotNetCore-Pages](https://github.com/jbogard/ContosoUniversityDotNetCore-Pages)
- [https://github.com/kgrzybek/modular-monolith-with-ddd](https://github.com/kgrzybek/modular-monolith-with-ddd)
- [https://github.com/oskardudycz/EventSourcing.NetCore](https://github.com/oskardudycz/EventSourcing.NetCore)
- [https://github.com/thangchung/clean-architecture-dotnet](https://github.com/thangchung/clean-architecture-dotnet)
- [https://github.com/pdevito3/MessageBusTestingInMemHarness](https://github.com/pdevito3/MessageBusTestingInMemHarness)

## License
This project is made available under the MIT license. See [LICENSE](https://github.com/meysamhadeli/booking-microservices/blob/main/LICENSE) for details.
