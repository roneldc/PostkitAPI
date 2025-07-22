# PostkitAPI

Postkit is a clean and extensible ASP.NET Core Web API for user authentication, post and comment management using Identity and JWT. Follows best practices with layered architecture, role-based access, and Swagger docs.

## Features

- JWT Authentication with ASP.NET Core Identity
- Role-based Authorization (Admin, User)
- Register / Login / Change Password
- Get current user (`/me`)
- Manage Posts and Comments (CRUD)
- React to posts (Upvote, Like, etc.)
- Real-time notifications using SignalR
- Cloudinary Integration (Free tier – image upload and hosting)
- Mailjet Integration (Free tier – transactional email for registration, password reset, etc.)
- Clean separation: Controller → Service → Repository
- DTOs for request/response mapping
- Swagger UI with bearer auth support

## 📁 Folder Structure

```
Postkit/
├── Postkit.API/                     # ASP.NET Core Web API (entry point)
│   ├── Controllers/                 # Controllers for API endpoints
│   ├── Middlewares/                 # Global exception handler
│   ├── Program.cs
│   ├── appsettings.json

├── Postkit.Infrastructure/         # Infrastructure: EF Core DbContext, migrations
│   ├── Postkit.Data/               # Centralized EF Core context
│   ├── Migrations/                 # EF Core migrations

├── Postkit.Posts/                  # Posts module
│   ├── Interfaces/                 # IPostRepository, IPostService
│   ├── Services/                   # PostService.cs
│   ├── Repositories/               # PostRepository.cs
│   ├── Mappers/                    # ToDto, ToModel extensions
│   └── Queries/                    # LINQ queries / filters

├── Postkit.Comments/              # Comments module
│   ├── Interfaces/
│   ├── Services/
│   ├── Repositories/
│   ├── Mappers/
│   └── Queries/

├── Postkit.Reactions/             # Reactions module
│   ├── Interfaces/
│   ├── Services/
│   ├── Repositories/
│   ├── Mappers/
│   └── Queries/

├── Postkit.Notifications/         # Notifications module (including SignalR)
│   ├── Interfaces/
│   ├── Services/
│   ├── Repositories/
│   ├── Hubs/                      # SignalR Hub (secured)
│   └── Mappers/

├── Postkit.Shared/                # Shared layer for DTOs, interfaces
│   ├── Dtos/                      # Request/Response models
│   ├── Models/                    # ApplicationUser, base entities
│   ├── Constants/                 # Role names, claim types, etc.
│   ├── Helpers/                   # Utility classes, Standardized API response wrapper

├── Postkit.Tests/                 # xUnit test project
│   ├── Posts/                     # PostServiceTests, PostControllerTests
│   ├── Comments/
│   ├── Mocks/                     # Mock services/repositories
│   └── Utilities/                 # Shared test helpers

├── .gitignore
├── README.md
└── Postkit.sln
```

## 🚀 Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- Visual Studio or VS Code

### Clone & Run

```bash
git clone https://github.com/roneldc/PostkitAPI.git
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

## Architecture

- **Modular Structure** - Feature-based projects (Postkit.Posts, Postkit.Comments, Postkit.Reactions, Postkit.Notifications) for scalability and separation of concerns.
- **Controllers (in Postkit.API)**: Handle HTTP requests and delegate to services. One controller per module (e.g., PostsController, CommentsController).
- **Services (per module)**: Encapsulate business logic. Inject repositories and utility services like ICurrentUserService and ILogger<T>.
- **Repositories (per module)**: Abstract and encapsulate data access using PostkitDbContext.
- **DTOs (in Postkit.Shared)**: Used for request and response mapping across all modules to decouple API contracts from data models.
- **Mappers (per module)**: Static extension methods to convert between entities and DTOs (ToDto(), ToModel()).
- **PostkitDbContext (in Postkit.Infrastructure)**: Centralized EF Core context registered in Postkit.API, shared across modules via DI.
- **ICurrentUserService (in Postkit.Shared)**: Provides access to the currently authenticated user ID from the JWT token.
- **ILogger**: Structured, per-class logging support via ASP.NET Core's built-in logging (Serilog-compatible).
- **SignalR Hub (in Postkit.Notifications)**: Handles real-time notification delivery to connected clients (NotificationHub.cs).
- **Cloudinary Service (in Postkit.Posts)**: Provides free-tier image upload and retrieval support, abstracted via ICloudinaryService for loose coupling.
- **Mailjet Service (in Postkit.Identity)**: Sends transactional emails (e.g., registration, email confirmation, password reset) using Mailjet's free-tier API and templated messages.

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
- SignalR
- Cloudinary
- MailJet

## License

MIT © John Ronel Dela Cruz — free to use, modify, and distribute.

## ⭐ Credits ⭐

Built and maintained by John Ronel Dela Cruz.  
If you have questions, ideas, or feedback, feel free to open an issue or reach out via GitHub.
