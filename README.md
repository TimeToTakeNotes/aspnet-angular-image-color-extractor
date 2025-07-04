# ASP.NET Core + Angular Image Color Extractor

A full-stack image color extractor application that allows users to upload images, extract the centre pixel color (in hex), generate thumbnails, and view uploaded images.
The backend is built with **ASP.NET Core Web API**, and the frontend is built with **Angular (standalone components)**.

---

## **Features**

- Upload images → centre pixel color extracted
- Thumbnail generated
- Images path to wwwroot folder and data saved to SQL Server
- View image list + details in Angular app
- Secure user sign up/login
- Clean architecture: repository + service layers
- Environment variable used for DB connection

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

---

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
---

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

---

**Option 2 — Set directly in `appsettings.Development.json`**

```json
{
    "JwtSettings": {
        "Key": "YOUR_32_CHARACTER_SECRET_HERE"
    }
}
```
*(This will be picked up when your `ASPNETCORE_ENVIRONMENT` is set to `Development`)*

---

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

---

#### 2.6 Apply migrations + create database:

```bash
dotnet ef database update
```

*(Ensure SQL Server is running)*

---

#### 2.7 Run the backend:

```bash
dotnet run
```

The API will be available at: `http://localhost:5176`

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

## **Environment variable summary**

| Variable         | Purpose                       | Example                                                           |
| ---------------- | ----------------------------- | ----------------------------------------------------------------- |
| `SERVER_NAME`    | Your SQL Server name/instance | `ARNO_LAPTOP\SQLEXPRESS`                                          |
| `JWT_SECRET_KEY` | Secret used for JWT signing   | `3a335b0f0bbe70dd2623b2e00b825e4012efffcd8e6eb80e0cf16c91131f3e3` |

---

## **.gitignore / uploads**

* `wwwroot/uploads/` is used to save uploaded images and thumbnails at runtime.
* This folder is ignored by Git and **will not be pushed to GitHub**.
* Make sure the folder exists in `wwwroot/` before running the app (or it will be created on demand).

---

## **Example API endpoints**

| Method | URL                 | Description                                 |
| ------ | ------------------- | ------------------------------------------- |
| `POST` | `/api/image/upload` | Upload an image file                        |
| `GET`  | `/api/image/list`   | Get all uploaded images                     |
| `GET`  | `/api/image/{id}`   | Get details of a specific image             |
| `POST` | `/api/auth/register`| Register a new user                         |
| `POST` | `/api/auth/login`   | Authenticate user and return a JWT token    |
| `GET`  | `/api/auth/me`      | Validate token and return current user info |
