## Database Design

### Overview

The database uses PostgreSQL and is managed via Entity Framework Core migrations. The schema supports users and tasks with audit and search capabilities.

### Tables

#### `users`

- `id` (uuid, primary key)
- `name` (varchar(100), not null)
- `email` (varchar(200), not null, unique)
- `password_hash` (varchar(512), not null)
- `created_on` (timestamp, not null)

#### `tasks`

- `id` (uuid, primary key)
- `title` (varchar(200), not null)
- `description` (varchar(2000), nullable)
- `due_date` (timestamp, nullable)
- `status` (varchar(50), not null)
- `remarks` (varchar(1000), nullable)
- `created_on` (timestamp, not null)
- `updated_on` (timestamp, not null)
- `created_by_user_id` (uuid, not null, foreign key to `users.id`)
- `updated_by_user_id` (uuid, not null, foreign key to `users.id`)

### Relationships

- `tasks.created_by_user_id` → `users.id` (many tasks to one user).
- `tasks.updated_by_user_id` → `users.id` (many tasks to one user).

Cascade deletes are restricted for audit integrity; tasks remain even if users are removed.

### Indexes

- `idx_tasks_title` on `tasks(title)`
- `idx_tasks_due_date` on `tasks(due_date)`
- `idx_tasks_status` on `tasks(status)`
- `idx_tasks_created_by_user_id` on `tasks(created_by_user_id)`
- Unique index on `users(email)`

These indexes support fast lookups for common filters and search scenarios.

### Seed Data

On first run, a default administrator user is created along with one sample task:

- Email: `admin@taskmanager.local`
- Password: `Admin@123` (hashed with configured salt)

This allows immediate login and system validation.

