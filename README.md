# Project: Base Architecture for WebAPI, gRPC, EntityFramework, and RabbitMQ

This project establishes a foundational solution for a system that uses **gRPC** for service-to-service communication, **EntityFramework** for database interaction, and **RabbitMQ** for asynchronous messaging. The solution is designed to be modular and extensible.

## Project Structure
The solution consists of the following projects:

1. **WebAPI**:
   - Acts as the primary interface of the system.
   - Communicates with the service via gRPC to perform operations.

2. **Service**:
   - Implements business logic and handles database communication using EntityFramework.
   - Also sends and receives messages through RabbitMQ.

3. **Class Library for Proto Files**:
   - Contains the contracts (.proto files) for gRPC communication.
   - Generates the required client and server classes.

4. **Class Library for EntityFramework**:
   - Contains the data models and EntityFramework context.
   - Manages migrations and database interactions.

5. **Class Library for RabbitMQ**:
   - Abstracts interaction with RabbitMQ.
   - Provides functionality for sending and receiving messages.

---

## Prerequisites

### Software
- **.NET SDK 6.0 or later**
- **RabbitMQ** (local or remote instance)
- **SQL Server** (or another EntityFramework-compatible provider)
- **Docker** (optional, for containerizing the solution)

### Dependencies
- **WebAPI**:
  - Microsoft.AspNetCore.Grpc
  - Swashbuckle.AspNetCore (for Swagger documentation, optional)

- **Service**:
  - Microsoft.EntityFrameworkCore
  - Microsoft.EntityFrameworkCore.SqlServer (or your database provider)
  - RabbitMQ.Client

- **Proto Class Library**:
  - Grpc.Tools (for generating classes from .proto files)

- **EntityFramework Class Library**:
  - Microsoft.EntityFrameworkCore
  - Microsoft.EntityFrameworkCore.Tools

- **RabbitMQ Class Library**:
  - RabbitMQ.Client

---

## Configuration

### 1. RabbitMQ Configuration
1. Ensure a RabbitMQ instance is running.
2. Configure the credentials and connection details in the `appsettings.json` file of the relevant project:

```json
"RabbitMQ": {
  "HostName": "localhost",
  "UserName": "guest",
  "Password": "guest",
  "ExchangeName": "my_exchange",
  "QueueName": "my_queue"
}
```

### 2. Database Configuration
1. Configure the connection string in the `appsettings.json` file of the service project:

```json
"ConnectionStrings": {
  "TaskManagerDatabase": "Server=localhost;Database=TaskManagerDb;Trusted_Connection=True;"
}
```

2. Apply migrations:

```bash
dotnet ef migrations add InitialCreate --project TaskManager.Data --startup-project TaskManager.Service
dotnet ef database update --project TaskManager.Data --startup-project TaskManager.Service
```

### 3. gRPC Configuration
Define your contracts in the class library using a `.proto` file. For example:

```proto
syntax = "proto3";

service TaskManagerGRPCService {
  rpc CreateTask (TaskCreateMessage) returns (TaskCreatedMessage);
}

message TaskCreateMessage {
  string name = 1;
  string description = 2;
  string status = 3;
}

message TaskCreatedMessage {
  int32 id = 1;
}
```

Compile the generated classes using the **Grpc.Tools** package.

---

## Execution

1. Start the entire solution from Visual Studio:

2. Access the API via gRPC using a client like **Postman** or gRPC-specific tools.

## Execution with Docker Compose
If you want to use containers, you need to install the latest version of Docker Desktop. Once installed, these are the steps to start the application on the repository folders:

1. docker-compose build
2. docker-compose up

---

## Workflow

1. **WebAPI**:
   - Receives user requests.
   - Calls the gRPC service to perform operations (e.g., creating a task).

2. **Service**:
   - Processes business logic.
   - Interacts with the database via EntityFramework.
   - Publishes events to RabbitMQ to notify other systems.

3. **RabbitMQ**:
   - Facilitates asynchronous communication between systems.

4. **Proto Library**:
   - Defines communication contracts between WebAPI and the Service.

5. **EntityFramework**:
   - Handles database access and data persistence.

---

