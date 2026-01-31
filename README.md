# SilkRoute

SilkRoute is a .NET library designed for HTTP communication in microservice-based systems. Its purpose is to reduce repetitive client-side infrastructure code by shifting request construction and response interpretation into a dedicated, reusable component.

The library allows a microservice API to be described declaratively as an interface-based contract. Once such a contract is configured for use, SilkRoute can provide a dynamically generated proxy that implements the interface and can be consumed like a regular application service. Although the calling code interacts with an ordinary interface, each method invocation is translated into an HTTP request to the target service, and the received response is mapped back to the return type declared by the contract.

## Quick start

SilkRoute can be integrated into an existing solution or introduced as part of a new one. The steps below are intentionally generic, so the same workflow applies regardless of whether the project already exists or is being created from scratch.

### Installation

SilkRoute is distributed as a NuGet package and can be installed using any standard method (CLI, IDE UI, Package Manager Console).

### Define a microservice contract interface

A microservice API is described as a .NET interface that declares the available operations as methods. The interface must inherit from IMicroserviceClient. Each method should be annotated with ASP.NET Core attributes that describe the endpoint. This contract becomes the single source of truth for how outgoing requests should be composed.

### Obtain a proxy implementation of the contract via SilkRoute

SilkRoute generates a runtime proxy that implements the contract interface and translates method calls into HTTP requests. At the moment, the primary integration path is registration through an extension method AddMicroserviceClient into the application’s dependency injection container. This step is performed in the service that needs to call the remote microservice. After registration, the contract can be injected anywhere in the application where DI is available.
There is also a planned alternative initialization path that would allow creating a client without DI. This work is tracked as an open [issue](https://github.com/EnskWt/SilkRoute/issues/2) in the repository.

### Use the generated client in the application

Once an instance of the contract is available, its methods are invoked like regular service methods. Under the hood, SilkRoute builds the request according to the contract metadata, sends it through HttpClient, reads the response, and maps the result back to the declared return type.

### Additional notes

If server-side endpoints accept raw binary bodies such as byte[] or Stream, the default ASP.NET Core configuration may not bind these payloads in the intended way. SilkRoute provides BinaryBodyInputFormatter for these scenarios. Registering this formatter on the server side enables safe and predictable binding of binary request bodies when an endpoint contract explicitly expects them.

## Examples and Troubleshooting Guidance

SilkRoute is designed to mirror ASP.NET Core semantics and produce HTTP requests and return values as predictably as possible. However, the overall process is inherently broad: request composition depends on routes, binding rules, headers, body serialization, form-data handling, and response interpretation. Because of that complexity, some combinations of contract metadata and payload shapes may behave differently than a user expects.

For this reason, the repository includes a set of practical examples that demonstrate contract definitions and integration patterns that are known to work and are actively supported by the library. When uncertain about a specific scenario, these examples should be treated as the primary reference point for “supported usage” and as a baseline for troubleshooting.

If a real-world contract still produces unexpected behavior, it is recommended to open an issue. A minimal reproduction based on the examples (contract snippet, expected behavior, actual behavior, and request/response details) makes it significantly easier to confirm whether the behavior is a bug, a missing validation, or an unsupported scenario that should be documented or implemented.
