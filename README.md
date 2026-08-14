# CodeSync 🎓

**CodeSync** is a full-stack e-learning platform built with **ASP.NET Core MVC 8**.
It provides a complete course-learning experience with user authentication, course enrollment, separate User/Admin panels, dynamic course management, contact & feedback systems, and common web security practices.

> Built as a practical MVC project with real-world application features rather than a simple CRUD application.

---

## ✨ Features

### 👤 User

* User registration & login
* Session-based authentication
* Cookie-based authentication
* Role-based authorization
* User dashboard
* Browse dynamically managed courses
* Course enrollment
* View enrolled courses
* Authenticated contact system
* Track submitted contact requests
* Submit feedback with rating & description
* View submitted feedback
* Privacy Policy & Terms and Conditions

### 🛠️ Admin

* Separate Admin Panel
* Course management
* User management
* Contact management
* Feedback management
* Dedicated admin navigation and layout

### 🔐 Security

The application implements several common web security practices:

* CSRF protection
* XSS protection/mitigation
* `HttpOnly` cookies
* `Secure` cookies
* `SameSite` cookie configuration
* Session-based authentication
* Role-based authorization
* Server-side validation
* Authentication-protected actions
* Private file uploads
* MIME type detection
* Secure image processing

### 🖼️ File & Image Handling

Uploaded files are validated before processing.

* MIME type detection
* Private upload handling
* Image validation
* PNG conversion
* Image processing with **SixLabors.ImageSharp**

---

## 🧩 Application Structure

CodeSync uses separate Razor layouts for different areas of the application:

```text
Views/
├── Shared/
│   └── _Layout.cshtml
│
├── User/
│   └── _UserLayout.cshtml
│
└── Admin/
    └── _AdminLayout.cshtml
```

* `_Layout.cshtml` → Public website
* `_UserLayout.cshtml` → Authenticated User area
* `_AdminLayout.cshtml` → Admin area

---

## 🗄️ Database

CodeSync uses **Microsoft SQL Server** with **Entity Framework Core**.

### Database Schema

![CodeSync Database Schema](docs/database-schema.png)

### Main Tables

| Table        | Purpose                        |
| ------------ | ------------------------------ |
| `Admin`      | Administrator accounts         |
| `User`       | User accounts and profile data |
| `Course`     | Course information             |
| `UserCourse` | User-course enrollment records |
| `Contact`    | User contact requests          |
| `Feedback`   | User feedback                  |

### 🔗 Relationships

```text
User
 │
 ├── 1 : N ──► Contact
 │
 ├── 1 : N ──► Feedback
 │
 └── 1 : N ──► UserCourse ◄── N : 1 ── Course
```

`UserCourse` works as the junction table between `User` and `Course`, allowing a user to enroll in multiple courses while each course can have multiple enrolled users.

The database uses multiple foreign keys:

```text
Contact.UserId      → User.UserId
Feedback.UserId     → User.UserId
UserCourse.UserId   → User.UserId
UserCourse.CourseId → Course.CourseId
```

The SQL schema is included in:

```text
codesync.sql
```

---

## 💳 Payment Integration

**Razorpay integration is planned** for course purchases.

The intended flow is:

```text
User
  ↓
Select Course
  ↓
Razorpay Payment
  ↓
Payment Verification
  ↓
Webhook
  ↓
Course Enrollment
```

Server-side webhook verification will be used instead of relying only on the client-side payment response.

---

## 🛠️ Tech Stack

### Backend

* C#
* ASP.NET Core MVC 8
* Entity Framework Core
* Razor Views

### Frontend

* HTML5
* CSS3
* JavaScript
* Bootstrap 5

### Database

* Microsoft SQL Server

### Security & File Processing

* Session & Cookie Authentication
* Role-based Authorization
* CSRF Protection
* XSS Mitigation
* HttpOnly / Secure / SameSite Cookies
* MIME Type Detection
* SixLabors.ImageSharp

### Tools

* Visual Studio
* SQL Server Management Studio
* Git
* GitHub

---

## 🚀 Getting Started

### Prerequisites

* .NET 8 SDK
* Microsoft SQL Server
* SQL Server Management Studio
* Visual Studio

### Clone the repository

```bash
git clone <repository-url>
cd CodeSync
```

### Database Setup

1. Create a SQL Server database named `CodeSync`.
2. Execute `codesync.sql`.
3. Update the connection string in `appsettings.json`.

Example:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=CodeSync;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### Run the project

```bash
dotnet restore
dotnet build
dotnet run
```

---

## 📌 Project Status

**Active Development 🚧**

### Implemented

* ASP.NET Core MVC 8 application
* User/Admin authentication
* Session & cookie-based authentication
* Role-based authorization
* User & Admin panels
* Dynamic course management
* Course enrollment
* Contact & contact history
* Feedback & feedback history
* Private file uploads
* Image processing
* Common web security protections
* Privacy Policy
* Terms & Conditions

### Planned

* Razorpay payment integration
* Razorpay webhook verification
* Automated enrollment after verified payment
* Further improvements to course management and learning features

---

## 🎯 Purpose

CodeSync was built to practice and demonstrate how a real-world **ASP.NET Core MVC application** can be structured beyond basic CRUD operations.

The project focuses on authentication, authorization, sessions, cookies, relational database design, dynamic content, file handling, user-specific data, admin workflows, and application security.

---

## 👨‍💻 Author

**Alok Maurya**

Full Stack Developer
C# • ASP.NET Core • React • Next.js • SQL Server

---

⭐ If you find the project interesting, consider giving it a star.
