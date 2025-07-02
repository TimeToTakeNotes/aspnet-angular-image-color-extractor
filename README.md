# ASP.NET Core + Angular Image Color Extractor

A full-stack image color extractor application that allows users to upload images, extract the centre pixel color (in hex), generate thumbnails, and view uploaded images.
The backend is built with **ASP.NET Core Web API**, and the frontend is built with **Angular (standalone components)**.

---

## **Features**

- Upload images → centre pixel color extracted
- Thumbnail generated
- Images path to wwwroot folder and data saved to SQL Server
- View image list + details in Angular app
- Clean architecture: repository + service layers
- Environment variable used for DB connection

---

## **Setup instructions**

### 1️. Clone the repository

```bash
git clone https://github.com/TimeToTakeNotes/aspnet-angular-image-color-extractor.git
cd aspnet-angular-image-color-extractor
```

---

### 2️. ASP.NET Core backend setup

#### Navigate to the backend folder:

```bash
cd ColorExtractorApi
```

#### Restore dependencies:

```bash
dotnet restore
```

#### Set up your database connection:

> The app expects an environment variable called `SERVER_NAME`
> You can set it locally like this (for development):

**Windows (PowerShell):**

```bash
$env:SERVER_NAME = "YOUR_SQL_SERVER_NAME"
```

**Linux/macOS:**

```bash
export SERVER_NAME=YOUR_SQL_SERVER_NAME
```

*(Replace `YOUR_SQL_SERVER_NAME` with your actual SQL Server instance, e.g. `ARNO_LAPTOP\SQLEXPRESS`)*

#### Apply migrations + create database:

```bash
dotnet ef database update
```

*(Ensure SQL Server is running)*

#### Run the backend:

```bash
dotnet run
```

The API will be available at: `http://localhost:5176`

---

### 3️. Angular frontend setup

#### Navigate to the frontend folder:

```bash
cd ../color-extractor-app
```

#### Install dependencies:

```bash
npm install
```

#### Run the Angular app:

```bash
ng serve --open
```

The frontend will be available at: `http://localhost:4200`

---

## **Environment variable summary**

| Variable      | Purpose                       | Example                  |
| ------------- | ----------------------------- | ------------------------ |
| `SERVER_NAME` | Your SQL Server name/instance | `ARNO_LAPTOP\SQLEXPRESS` |

---

## **.gitignore / uploads**

* `wwwroot/uploads/` is used to save uploaded images and thumbnails at runtime.
* This folder is ignored by Git and **will not be pushed to GitHub**.
* Make sure the folder exists in `wwwroot/` before running the app (or it will be created on demand).

---

## **Example API endpoints**

| Method | URL                 | Description                     |
| ------ | ------------------- | ------------------------------- |
| `POST` | `/api/image/upload` | Upload an image file            |
| `GET`  | `/api/image/list`   | Get all uploaded images         |
| `GET`  | `/api/image/{id}`   | Get details of a specific image |
