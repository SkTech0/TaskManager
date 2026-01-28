## Architecture

### Overview

The system is structured using clean architecture and a layered approach, separating concerns across domain, application, infrastructure, and presentation layers.

### Backend Projects

- `TaskManager.Domain`:
  - Contains core domain entities `User` and `TaskItem`.
  - Encapsulates business data structures without external dependencies.

- `TaskManager.Application`:
  - Contains DTOs for authentication and task operations.
  - Declares service and repository interfaces.
  - Implements application services `AuthService` and `TaskService`.
  - Applies business rules, validation, and orchestration of operations.

- `TaskManager.Infrastructure`:
  - Implements persistence using Entity Framework Core with PostgreSQL.
  - Defines `ApplicationDbContext` and entity configurations.
  - Implements repositories (`TaskRepository`, `UserRepository`) and `UnitOfWork`.
  - Implements `JwtTokenGenerator` and security settings.

- `TaskManager.API`:
  - ASP.NET Core Web API project exposing HTTP endpoints.
  - Configures dependency injection, authentication, authorization, logging, and Swagger.
  - Implements controllers `AuthController` and `TasksController`.
  - Hosts global exception handling middleware and seed data.

### Frontend Project

- **Angular 18.2 SPA** in the `frontend` folder:
  - **Standalone components architecture** (no NgModules)
  - Feature-based organization:
    - `features/auth/` - Login and registration components
    - `features/tasks/` - Task management components (list, create, edit, details, form)
  - Core services and utilities:
    - `core/services/` - AuthService, TaskService
    - `core/guards/` - AuthGuard for route protection
    - `core/interceptors/` - HTTP interceptor for JWT token injection
    - `core/components/` - Shared components (Navbar)
  - **Dependency Injection**: Uses constructor injection with proper provider configuration
  - **Routing**: Client-side routing with route guards
  - **Forms**: Reactive forms with FormBuilder
  - **HTTP Client**: Configured with interceptors for authentication
  - Responsive UI using inline component styles

### Cross-Cutting Concerns

- **Authentication and Authorization**:
  - Users authenticate via `/api/auth/login` and `/api/auth/register`.
  - JWT tokens are issued by the backend, stored on the client, and attached to API requests.
  - Protected endpoints require valid bearer tokens.

- **Validation**:
  - DataAnnotations on DTOs.
  - ASP.NET Core model validation in controllers.
  - Frontend reactive-form validators.

- **Error Handling**:
  - Global exception middleware returns consistent JSON error payloads.
  - Frontend surfaces error messages near forms and pages.

- **Logging**:
  - Serilog configured for structured console logging.
  - Request logging via Serilog middleware.
  - Application logs in services and controllers.

### Clean Architecture Principles

- Inner layers (`Domain`, `Application`) do not depend on outer layers.
- Application layer depends only on domain abstractions and DTOs.
- Infrastructure and API reference application and domain layers.
- Dependency inversion is applied through interfaces for repositories and services.

