## API Documentation

Base URL:

- **Local development**: `http://localhost:5000/api`
- **Docker**: `http://localhost:5001/api`

**Note**: When running with Docker, the API is exposed on port **5001** (mapped from container port 8080).

### Authentication

#### Register

- **Endpoint**: `POST /api/auth/register`
- **Request Body**:

```json
{
  "name": "John Doe",
  "email": "john@example.com",
  "password": "Secret123"
}
```

- **Response**: `200 OK`

```json
{
  "token": "<jwt_token>",
  "expiresAt": "2026-01-27T10:00:00Z",
  "userId": "uuid",
  "name": "John Doe",
  "email": "john@example.com"
}
```

#### Login

- **Endpoint**: `POST /api/auth/login`
- **Request Body**:

```json
{
  "email": "john@example.com",
  "password": "Secret123"
}
```

- **Response**: `200 OK` (same shape as register response)

### Tasks

All task endpoints require a valid `Authorization: Bearer <jwt_token>` header.

#### Create Task

- **Endpoint**: `POST /api/tasks`
- **Request Body**:

```json
{
  "title": "Prepare report",
  "description": "Prepare quarterly financial report",
  "dueDate": "2026-02-01T00:00:00Z",
  "status": "Pending",
  "remarks": "High priority"
}
```

- **Response**: `201 Created`
- **Body**: `TaskDto`

#### Get All Tasks

- **Endpoint**: `GET /api/tasks`
- **Response**: `200 OK`
- **Body**: `TaskDto[]`

#### Get Task By Id

- **Endpoint**: `GET /api/tasks/{id}`
- **Response**:
  - `200 OK` on success
  - `404 Not Found` if missing

#### Update Task

- **Endpoint**: `PUT /api/tasks/{id}`
- **Request Body**: Same shape as create request.
- **Response**:
  - `200 OK` with updated `TaskDto`
  - `404 Not Found` if missing

#### Delete Task

- **Endpoint**: `DELETE /api/tasks/{id}`
- **Response**:
  - `204 No Content` on success
  - `404 Not Found` if missing

#### Search Tasks

- **Endpoint**: `GET /api/tasks/search?q=term`
- **Query Parameters**:
  - `q` (required): Text to search within title, description, remarks, and status.
  - If `q` is empty or whitespace, returns all tasks.
- **Response**: `200 OK`

```json
{
  "items": [
    {
      "id": "uuid",
      "title": "Task title",
      "description": "Task description",
      "dueDate": "2026-02-01T00:00:00Z",
      "status": "Pending",
      "remarks": "Remarks",
      "createdOn": "2026-01-27T10:00:00Z",
      "updatedOn": "2026-01-27T10:00:00Z",
      "createdByUserId": "uuid",
      "updatedByUserId": "uuid",
      "createdByName": "User Name",
      "updatedByName": "User Name"
    }
  ],
  "totalCount": 1
}
```

**Note**: Status filtering is currently handled on the client side. The backend search endpoint only supports text search via the `q` parameter.

### Error Responses

Errors are returned as JSON with the following shape:

```json
{
  "status": 400,
  "error": "Validation or business error message"
}
```

For unauthorized requests, the status is `401`, for missing resources, the status is `404`, and for unexpected failures, the status is `500`.

