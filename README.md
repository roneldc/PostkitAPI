# Postkit.API

Postkit is a clean and extensible ASP.NET Core Web API for user authentication, post and comment management using Identity and JWT. Follows best practices with layered architecture, role-based access, and Swagger docs.

## Features

- JWT Authentication with ASP.NET Core Identity
- Role-based Authorization (Admin, User)
- Register / Login / Change Password
- Get current user (`/me`)
- Manage Posts and Comments (CRUD)
- Clean separation: Controller → Service → Repository
- DTOs for request/response mapping
- Swagger UI with bearer auth support
- Built-in logging with `ILogger`

## 📁 Folder Structure

```
Postkit.API/
├── Constants/
├── Controllers/
├── Data/
├── DTOs/
├── Helpers/
├── Interfaces/
├── Mappers/
├── Middleware/
├── Models/
├── Queries/
├── Repositories/
├── Services/
├── Program.cs
```

## 🚀 Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- Visual Studio or VS Code

### Clone & Run

```bash
git clone https://github.com/roneldc/Postkit.API.git
cd Postkit.API
dotnet run
```

Open Swagger: http://localhost:5129/swagger

## Authentication

All authenticated endpoints require a JWT token via:

```
Authorization: Bearer {token}
```

You can obtain the token by calling:

```
POST /api/auth/login
```

## 🎯 API Endpoints

#### Auth (Public)

| Method | Endpoint             | Description       |
| ------ | -------------------- | ----------------- |
| POST   | `/api/auth/register` | Register new user |
| POST   | `/api/auth/login`    | Login and get JWT |

#### Account (Requires Auth)

| Method | Endpoint                        | Description          |
| ------ | ------------------------------- | -------------------- |
| GET    | `/api/accounts/me`              | Get current user     |
| POST   | `/api/accounts/change-password` | Change user password |

#### Admin-only

| Method | Endpoint                                                                                | Description                                         |
| ------ | --------------------------------------------------------------------------------------- | --------------------------------------------------- |
| GET    | `/api/accounts/users?Username={username}&Email={email}&Page={page}&PageSize={pageSize}` | List all users with optional filters and pagination |
| POST   | `/api/accounts/assign-role`                                                             | Assign a role to a user                             |

#### Posts

| Method | Endpoint                                                                     | Description                                                               |
| ------ | ---------------------------------------------------------------------------- | ------------------------------------------------------------------------- |
| GET    | `/api/posts?Title={title}&Content={content}&Page={page}&PageSize={pageSize}` | Get all posts (filter by Title, Content; paginate with Page and PageSize) |
| GET    | `/api/posts/{id}`                                                            | Get single post                                                           |
| POST   | `/api/posts`                                                                 | Create new post                                                           |
| PUT    | `/api/posts/{id}`                                                            | Update existing post                                                      |
| DELETE | `/api/posts/{id}`                                                            | Delete own post                                                           |

#### Comments

| Method | Endpoint                 | Description           |
| ------ | ------------------------ | --------------------- |
| GET    | `/api/{postId}/comments` | Get comments per post |
| POST   | `/api/{postId}/comments` | Add comment to post   |
| DELETE | `/api/comments/{id}`     | Delete own comment    |

## Architecture

- **Controllers**: Handle HTTP requests only.
- **Services**: Handle business logic.
- **Repositories**: Abstract data access.
- **DTOs**: Separate models for request/response.
- **ICurrentUserService**: Access current user from token.
- **ILogger**: Structured logging support.

## Sample Users (Seeded in Dev)

Admin

```
{
  "username": "admin"
  "email": "admin@email.com",
  "password": "Admin123"
}
```

User

```
{
  "username": "user"
  "email": "user@email.com",
  "password": "User@123"
}
```

## Tech Stack

- ASP.NET Core 8 Web API
- Entity Framework Core
- ASP.NET Core Identity
- JWT Authentication
- Swagger / Swashbuckle
- SQL Server

## License

MIT © John Ronel Dela Cruz — free to use, modify, and distribute.

## 🔄 Possible Enhancements

These are features and improvements that can be added to enhance the quality, reliability, and maintainability of the project:

### Final Cleanup (Before Production)

- [ ] Ensure all error messages are clear, consistent, and helpful.
- [ ] Validate edge cases and invalid inputs.

### Functional Enhancements

- [ ] Sorting support for /api/posts (e.g., by date, title).
- [ ] Comment Query filters (e.g., latest, by user, search keyword).
- [ ] Implement reply-to-comment feature

### Testing

- [✅] Add unit tests for services, repositories, and controllers.
- [✅] Setup integration tests for key API flows.

### Documentation

- [ ] Complete XML comments for all DTOs and service methods.
- [ ] Use Swagger annotations for richer API docs.
- [ ] Add a guide for setup, usage, and deployment.

### API Management

- [ ] Add API Versioning support (e.g., /api/v1/account/me).
- [ ] Add rate limiting, throttling, or request logging (for future scale).

### Deployment

- [ ] Add a Dockerfile for containerization.
- [ ] Create a docker-compose.yml for local DB + API setup.
- [ ] Add environment-based config loading (appsettings.{env}.json).

### Logging

- [ ] Integrate Serilog for structured and centralized logging.
- [ ] Support logging to files, console, and cloud sinks (e.g., Seq, Application Insights).

## ⭐ Credits ⭐

Built and maintained by John Ronel Dela Cruz.  
If you have questions, ideas, or feedback, feel free to open an issue or reach out via GitHub.
