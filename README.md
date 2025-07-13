# ASP.NET Core + Angular Image Color Extractor

A full-stack image color extractor application that allows users to upload images, extract the center pixel color (in hex), generate thumbnails, and view uploaded images.

- **Backend**: ASP.NET Core Web API (.NET 7+)
- **Frontend**: Angular (standalone components)
- **Security**: JWT Auth via HttpOnly Cookies + CSRF Protection

---

## **Features**

- Upload image → extract center pixel color
- Thumbnail generation
- Images saved in per-user folders under `wwwroot/uploads/{userId}/`
- Secure user registration/login using JWT in HttpOnly cookies (access + refresh tokens)
- CSRF protection via custom middleware
- Token refresh + logout endpoints
- View your image list and details (per-user access)
- Image deletion removes both the DB entry and physical files (image + thumbnail)
- Repository + service layer separation
- Angular standalone components for clean architecture
- Environment-based config support (`appsettings.Development.json`, env vars)

---

## **Setup instructions**

### **1️. Clone the repository**

```bash
git clone https://github.com/TimeToTakeNotes/aspnet-angular-image-color-extractor.git
cd aspnet-angular-image-color-extractor
```

---

### 2️. ASP.NET Core backend setup

#### 2.1 Navigate to the backend folder:

```bash
cd ColorExtractorApi
```

#### 2.2 Restore dependencies:

```bash
dotnet restore
```

#### 2.3 Set up your database connection:

> The app expects an environment variable called `SERVER_NAME` or connection string to be set in `appsettings.json` or `appsettings.Development.json`
> You can set it locally like this (for development):

**Option 1 — Set via environment variable**

**Windows (PowerShell):**

```bash
$env:SERVER_NAME = "YOUR_SQL_SERVER_NAME"
```

**Linux/macOS:**

```bash
export SERVER_NAME=YOUR_SQL_SERVER_NAME
```

*(Replace `YOUR_SQL_SERVER_NAME` with your actual SQL Server instance, e.g. `ARNO_LAPTOP\SQLEXPRESS`)*

**Option 2 — Set directly in `appsettings.Development.json`**

```json
{
    "ConnectionStrings": {
        "DefaultConnection": "Server=(YOUR_SQL_SERVER_NAME);Database=ColorExtractorDb;Trusted_Connection=True;TrustServerCertificate=True;"
    }
}
```

#### 2.4 Set up your JWT secret

> The app requires a JWT secret key for generating and validating tokens. You can provide it via an environment variable or in your `appsettings.Development.json`.

**Option 1 — Set via environment variable**

**Windows (PowerShell):**

```powershell
$env:JWT_SECRET_KEY = "YOUR_32_CHARACTER_SECRET_HERE"
```

**Linux/macOS:**

```bash
export JWT_SECRET_KEY=YOUR_32_CHARACTER_SECRET_HERE
```
*(Choose a strong secret! Example: `3a335b0f0bbe70dd2623b2e00b825e4012efffcd8e6eb80e0cf16c91131f3e3`)*

**Option 2 — Set directly in `appsettings.Development.json`**

```json
{
    "JwtSettings": {
        "Key": "YOUR_32_CHARACTER_SECRET_HERE"
    }
}
```
*(This will be picked up when your `ASPNETCORE_ENVIRONMENT` is set to `Development`)*

#### 2.5 Set the ASP.NET Core environment to Development (optional but recommended)

To ensure the app loads your development settings (like `appsettings.Development.json`), set the environment variable `ASPNETCORE_ENVIRONMENT` to `Development`.

**Windows (PowerShell):**

```powershell
$env:ASPNETCORE_ENVIRONMENT = "Development"
```

**Linux/macOS:**

```bash
export ASPNETCORE_ENVIRONMENT=Development
```

Here's a concise section you can add under the **"ASP.NET Core backend setup"** section to guide users on installing the `dotnet-ef` tool before running migrations:

---

##### 2.5.1 (Optional) Install EF Core CLI if not installed

If you haven't installed the Entity Framework Core CLI (`dotnet ef`), run:

```bash
dotnet tool install --global dotnet-ef
```

Then confirm the installation:

```bash
dotnet ef --version
```

> If you're using .NET 9, make sure the EF Core CLI version matches your SDK version. See: [https://www.nuget.org/packages/dotnet-ef](https://www.nuget.org/packages/dotnet-ef)

#### 2.6 Apply migrations + create database:

```bash
dotnet ef database update
```

*(Ensure SQL Server is running)*

##### 2.6.1 If errors occur during migration

If the migration fails due to missing tables or conflicting changes, you can safely reset your migrations during development:

```bash
# Optional: Drop the existing database
dotnet ef database drop --force

# Delete all migration files (under Migrations/ or Data/Migrations/)
# Then recreate the initial migration
dotnet ef migrations add InitialCreate

# Apply the migration
dotnet ef database update
```

> ⚠️ Only do this in development — dropping the database will remove all data.

#### 2.7 Run the backend:

```bash
dotnet run
```

The API will be available at: `http://localhost:5176`
Images are saved per user under `wwwroot/uploads/{userId}/`

---

### 3️. Angular frontend setup

#### 3.1 Navigate to the frontend folder:

```bash
cd ../color-extractor-app
```

#### 3.2 Install dependencies:

```bash
npm install
```

#### 3.3 Run the Angular app:

```bash
ng serve --open
```

The frontend will be available at: `http://localhost:4200`

---

## Authentication & Security Notes:

- Login, registration, and token refresh use HttpOnly cookies.
- CSRF protection is handled by a custom .NET middleware (via X-CSRF-TOKEN header).
- Angular HttpClient must set { withCredentials: true } for secure cookie transfer.
- auth.guard.ts protects Angular routes.
- Auth state is tracked with a reactive BehaviorSubject in auth.service.ts.

---

## File Storage

- Uploaded images and thumbnails are stored in `wwwroot/uploads/{userId}/` with subfolder `thumbnails/`.
- On image deletion, the system removes:
  - The image file (e.g., uploads/5e2e23a9.png)
  - The thumbnail (e.g., uploads/thumbnails/5e2e23a9_thumb.jpg)
  - The corresponding DB record

---

## **Environment variable summary**

| Variable         | Purpose                       | Example                                                           |
| ---------------- | ----------------------------- | ----------------------------------------------------------------- |
| `SERVER_NAME`    | Your SQL Server name/instance | `ARNO_LAPTOP\SQLEXPRESS`                                          |
| `JWT_SECRET_KEY` | Secret used for JWT signing   | `3a335b0f0bbe70dd2623b2e00b825e4012efffcd8e6eb80e0cf16c91131f3e3` |

---

## **.gitignore / uploads**

- The `wwwroot/uploads/` folder is ignored in Git.
- Folder structure is auto-created per user on first upload.
- You only need to manually create the wwwroot directory if not present.

---

## **Example API endpoints**

| Method  | URL                        | Description                                 |
| ------  | -------------------------- | ------------------------------------------- |
| `POST`  | `/api/auth/register`       | Register new user                           |
| `POST`  | `/api/auth/login`          | Login and receive auth cookies              |
| `GET`   | `/api/auth/me`             | Validate and return current user            |
| `POST`  | `/api/auth/refresh`        | Refresh access token                        |
| `POST`  | `/api/auth/logout`         | Invalidate session                          |
| `POST`  | `/api/image/upload`        | Upload image and extract color              |
| `DELETE`| `/api/image/{id}`          | Delete image (DB + file system)             |
| `GET`   | `/api/image/my-images`     | Get current user's images (thumbnail list)  |
| `GET`   | `/api/image/{id}`          | Get details of a specific image             |
| `PUT`   | `/api/user/me`             | Update user name, surname, and email        |
| `DELETE`| `/api/user/me`             | Delete user account and data                |
| `POST`  | `/api/user/update-password`| Update user password                        |
