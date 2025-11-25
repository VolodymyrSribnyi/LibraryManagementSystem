# 📚 Library Management System

## Overview
The Library Management System is a comprehensive web application designed to streamline library operations and enhance user experience. Built with modern web technologies and following Clean Architecture principles, the system provides robust functionality for managing books, authors, users, reservations, and notifications. The application features role-based access control, real-time notifications, and an intuitive user interface.

---

## Features

### Core Functionality

- **Book Management**:
  - Add, edit, delete, and search books with detailed information
  - Upload and display book cover images
  - Filter books by author, genre, publisher, publication year, rating, and availability
  - Rate books on a 5-star scale
  - Track book availability status in real-time

- **Author Management**:
  - Create, update, and delete author profiles
  - View detailed author information including biography and published books
  - Link books to authors with full relationship tracking

- **User Management**:
  - User registration and authentication with ASP.NET Core Identity
  - Role-based access control (Admin and User roles)
  - Profile management with password change functionality
  - Secure login with "Remember Me" option

- **Library Card System**:
  - Issue library cards to registered users
  - Track card validity and expiration dates
  - Automatic validation for reservation eligibility

- **Book Reservation System**:
  - Reserve available books for 14-day periods
  - Automatic due date calculation
  - View active and returned reservations
  - Overdue tracking with status indicators
  - Return books with automatic availability updates

- **Notification System**:
  - Real-time notifications for book availability
  - Reservation confirmations
  - Due date reminders
  - Library card expiry alerts
  - New book arrival notifications
  - System alerts and general updates

- **Subscription Management**:
  - Subscribe to book availability notifications
  - Manage subscriptions for unavailable books
  - Automatic notifications when books become available
  - Unsubscribe functionality

### Administrative Features

- **Admin Dashboard**:
  - Centralized management interface
  - View and manage all books, authors, and users
  - Monitor all reservations system-wide
  - Manage library cards

- **User Dashboard**:
  - Personal account management
  - View active and returned reservations
  - Manage subscriptions and notifications
  - Update profile information

### Key Highlights

- **Clean Architecture**: Separation of concerns with Domain, Application, Infrastructure, and Presentation layers
- **Result Pattern**: Consistent error handling without exceptions
- **Domain Events**: Event-driven architecture for book availability notifications
- **Repository Pattern**: Abstraction of data access logic
- **Service Layer**: Business logic encapsulation
- **AutoMapper**: Object-to-object mapping
- **Entity Framework Core**: ORM with SQL Server database
- **Responsive Design**: Mobile-friendly user interface

---

## Screenshots

### Home Page
<img width="1879" height="944" alt="image" src="https://github.com/user-attachments/assets/1a3c8bb4-92e3-4064-8492-fb331e448163" />


### Book Catalog
<img width="1895" height="946" alt="image" src="https://github.com/user-attachments/assets/fcd57e3f-053b-4a3e-b8d9-24316deeb563" />


### Book Details
<img width="1893" height="931" alt="image" src="https://github.com/user-attachments/assets/d7d91ab1-cde5-475f-ae05-b3f852709d8d" />

### Add/Edit Book
<img width="1896" height="950" alt="image" src="https://github.com/user-attachments/assets/c3c7e6af-b1fc-4f20-a96a-f41aa07323b2" />


### User Dashboard



### Active Reservations
<img width="1915" height="393" alt="image" src="https://github.com/user-attachments/assets/9a305241-c90d-4302-bfe6-c28166ab842b" />


### Admin Dashboard
<img width="1898" height="928" alt="image" src="https://github.com/user-attachments/assets/dcd815ac-9d52-43a4-946b-8d0bdee23d2a" />


### Notification Subscriptions
<img width="1915" height="341" alt="image" src="https://github.com/user-attachments/assets/d121108d-17f7-40e5-9845-53970e597f1f" />
<img width="1915" height="945" alt="image" src="https://github.com/user-attachments/assets/2b276570-39eb-4333-8d69-b280e42e2827" />

---

## Architecture

### Project Structure

```
LibraryManagementSystem/
├── Domain/                          # Domain layer (entities, enums, events, exceptions)
│   ├── Entities/                   # Domain entities
│   ├── Enums/                      # Domain enumerations
│   ├── Events/                     # Domain events
│   └── Exceptions/                 # Custom exceptions
├── Application/                     # Application layer (DTOs, services, filters)
│   ├── DTOs/                       # Data Transfer Objects
│   ├── Services/                   # Service interfaces
│   ├── Mappers/                    # AutoMapper profiles
│   ├── Filters/                    # Query filters
│   └── ErrorHandling/              # Result pattern implementation
├── Infrastructure/                  # Infrastructure layer (data access, implementations)
│   ├── Configurations/             # Entity configurations
│   ├── Repositories/               # Repository implementations
│   ├── Services/                   # Service implementations
│   └── Events/                     # Event publisher
├── Web/                            # Presentation layer (MVC)
│   ├── Controllers/                # MVC controllers
│   ├── Views/                      # Razor views
│   ├── Filters/                    # Custom authorization filters
│   ├── Middleware/                 # Custom middleware
│   └── wwwroot/                    # Static files (CSS, images)
└── InfrastructureTests/            # Unit tests
    └── Services/                   # Service tests
```

### Database Schema

**ApplicationUser**
- Id (PK, Guid)
- FirstName, Surname, MiddleName
- Age, Email, UserName
- LibraryCardId (FK)
- CreatedAt

**Author**
- Id (PK, Guid)
- FirstName, Surname, MiddleName
- Age, Description
- CreatedAt, IsDeleted

**Book**
- Id (PK, Guid)
- Title, Description
- AuthorId (FK)
- Genre, Publisher, PublishingYear
- IsAvailable, Rating
- PictureSource
- CreatedAt, IsDeleted

**Reservation**
- Id (PK, Guid)
- UserId (FK), BookId (FK)
- ReservedAt, EndsAt
- IsReturned

**Notification**
- Id (PK, Guid)
- UserId (FK), BookId (FK, nullable)
- Message, NotificationType
- IsRead, CreatedAt

**LibraryCard**
- Id (PK, Guid)
- UserId (FK)
- IsValid, ValidTo
- IsDeleted

**BookNotificationRequest**
- Id (PK, Guid)
- UserId (FK), BookId (FK)
- IsNotified

---

## Technologies Used

### Backend
- **Framework**: ASP.NET Core 8.0 MVC
- **ORM**: Entity Framework Core 9.0
- **Database**: Microsoft SQL Server
- **Identity**: ASP.NET Core Identity for authentication and authorization
- **Mapping**: AutoMapper 13.0

### Frontend
- **View Engine**: Razor Pages
- **CSS Framework**: Bootstrap 5
- **Icons**: Custom CSS styling

### Testing
- **Unit Testing**: xUnit 2.5.3
- **Mocking**: Moq 4.20.72
- **Assertions**: FluentAssertions 8.8.0

### Design Patterns & Principles
- Clean Architecture
- Repository Pattern
- Service Layer Pattern
- Result Pattern (for error handling)
- Domain Events
- Dependency Injection
- SOLID Principles

---

## Installation

### Prerequisites
- .NET 8.0 SDK or later
- SQL Server 2019 or later (or SQL Server Express)
- Visual Studio 2022 or Visual Studio Code
- Git

### Setup Steps

1. **Clone the repository**
```bash
git clone https://github.com/VolodymyrSribnyi/LibraryManagementSystem.git
cd LibraryManagementSystem
```

2. **Configure the database connection**
   
   Update `appsettings.json` in the Web project:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=LibraryDB;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

3. **Apply database migrations**
```bash
cd Infrastructure
dotnet ef database update --startup-project ../Web
```

4. **Restore NuGet packages**
```bash
dotnet restore
```

5. **Build the solution**
```bash
dotnet build
```

6. **Run the application**
```bash
cd Web
dotnet run
```

7. **Access the application**
   
   Open your browser and navigate to `https://localhost:5001` (or the port shown in the console)

### First-Time Setup

1. **Register the first user** - This user will automatically receive Admin role
2. **Log in with admin credentials**
3. **Add authors** before adding books
4. **Start adding books to the catalog**

---

## Usage

### For Users

**Browsing Books**
1. Navigate to "Book Catalog" from the main menu
2. Use filters to narrow down search (author, genre, year, rating, availability)
3. Click on a book card to view detailed information

**Reserving a Book**
1. Find an available book in the catalog
2. Click "Reserve this Book"
3. Confirm the reservation (14-day period)
4. View your active reservations in the User Dashboard

**Subscribing to Book Availability**
1. For unavailable books, click "Subscribe to Availability Notification"
2. Receive automatic notification when the book becomes available
3. Manage subscriptions in "My Subscriptions"

**Managing Your Account**
1. Access User Dashboard from the main menu
2. View active and returned reservations
3. Check notifications
4. Update profile or change password
5. Get a library card if you don't have one

### For Administrators

**Adding a New Book**
1. Navigate to Admin Dashboard → Add New Book
2. Fill in book details (title, author, genre, publisher, year, description)
3. Upload book cover image (optional, max 2MB)
4. Click "Add Book"

**Managing Authors**
1. Go to Admin Dashboard → Manage Authors
2. Add new authors with biography and details
3. Edit or delete existing authors
4. View all books by each author

**Monitoring Reservations**
1. Access "View All Reservations" from Admin Dashboard
2. See all active and returned reservations
3. Monitor overdue books
4. Track user activity

**Managing Users**
1. View all registered users
2. Monitor user activity
3. Manage library cards

---

## Domain Events

The system implements domain-driven design with the following event:

**BookBecameAvailableEvent**
- Triggered when a book is returned
- Automatically notifies all subscribed users
- Marks subscription as notified
- Removes the subscription after notification

---

## Error Handling

The application uses the Result pattern for consistent error handling:

```csharp
public class Result<T>
{
    public bool IsSuccess { get; }
    public bool IsFailure { get; }
    public T Value { get; }
    public Error Error { get; }
}
```

Error types include:
- `NullData`: Invalid or missing data
- `AuthorNotFound`, `BookNotFound`, `UserNotFound`
- `AuthorExists`, `BookExists`, `UserExists`
- `ReservationNotFound`, `ReservationExists`
- `LibraryCardExpired`, `LibraryCardNotFound`
- Custom validation errors

---

## Testing

### Running Unit Tests

```bash
cd InfrastructureTests
dotnet test
```

### Test Coverage

The solution includes comprehensive unit tests for:
- AuthorService
- BookService
- BookNotificationRequestService
- NotificationService
- ReservingBookService

Tests use Moq for mocking dependencies and FluentAssertions for readable assertions.

---

## Security Features

- **Authentication**: ASP.NET Core Identity with cookie-based authentication
- **Authorization**: Role-based access control (Admin, User)
- **Password Security**: Hashed passwords with Identity's secure hashing
- **Custom Authorization Filter**: `CustomAuthorize` attribute for fine-grained access control
- **HTTPS**: Enforced secure connections
- **Anti-forgery Tokens**: CSRF protection on forms
