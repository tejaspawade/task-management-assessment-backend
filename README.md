# TaskBoard API

A lightweight Jira-style task board backend built with **ASP.NET Core 8 Web API**. Tasks are organized into columns (e.g. *To Do*, *In Progress*, *Done*), can be favorited, and are stored in-memory for the lifetime of the process.

<img width="1830" height="916" alt="Task Board Assessment" src="https://github.com/user-attachments/assets/a05c6e49-701c-4149-bc9b-47111d43595c" />


## Project Structure

```
backend/
├── TaskBoard.Api/            # Web API project
│   ├── Controllers/          # ColumnsController, TasksController
│   ├── Models/                # BoardColumn, TaskItem, request/response DTOs
│   ├── Services/              # IBoardService / BoardService (in-memory store)
│   └── Program.cs             # App startup & configuration
└── TaskBoard.Api.Tests/       # NUnit test project for BoardService
```

## Requirements

- [.NET 8 SDK](https://dotnet.microsoft.com/download)

## Getting Started

```bash
# Restore dependencies
dotnet restore

# Run the API
dotnet run --project TaskBoard.Api
```

By default, Swagger UI is available in the Development environment at `/swagger` for exploring the API.

The API is configured with CORS to allow requests from a React frontend running at `http://localhost:3000`.

## Running Tests

```bash
dotnet test
```
<img width="746" height="58" alt="image" src="https://github.com/user-attachments/assets/a2c6e3b2-10c5-4a14-97bb-6b87065e2af5" />


## API Endpoints

### Columns

| Method | Route            | Description            |
|--------|------------------|-------------------------|
| GET    | `/api/columns`   | List all columns        |
| POST   | `/api/columns`   | Create a new column     |

### Tasks

| Method | Route                        | Description                          |
|--------|------------------------------|---------------------------------------|
| GET    | `/api/tasks?columnId={id}`   | List tasks for a column               |
| GET    | `/api/tasks/{id}`            | Get a single task by id               |
| POST   | `/api/tasks`                 | Create a new task                     |
| PUT    | `/api/tasks/{id}`            | Update a task                         |
| DELETE | `/api/tasks/{id}`            | Delete a task                         |
| PATCH  | `/api/tasks/{id}/move`       | Move a task to a different column     |
| PATCH  | `/api/tasks/{id}/favorite`   | Toggle a task's favorite status       |

## Data Model

- **BoardColumn** — `Id`, `Name`, `Order`
- **TaskItem** — `Id`, `Name`, `Description`, `Deadline`, `ColumnId`, `IsFavorite`, `ImageUrl`, `CreatedAt`

Tasks within a column are sorted alphabetically by name, with favorited tasks pinned to the top.

## Notes

- Data is stored **in-memory** (`ConcurrentDictionary`) and is **not persisted** across application restarts.
- On startup, the board is seeded with three default columns: *To Do*, *In Progress*, and *Done*.
