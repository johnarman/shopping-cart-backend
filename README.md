This file provides clear guidance for users on how to run your backend application, access the Swagger API documentation, and use the project.

# Shopping Cart Backend

This is the backend API for the shopping cart application, built using .NET Core. The backend handles user authentication, product management, and shopping cart operations.

## Features

- User authentication with JWT
- Product listing and management
- Shopping cart functionality (add, update, remove items)
- Automatic cart cleanup service for abandoned carts
- Database seeding with demo data (including a default admin user)

## Getting Started

### Prerequisites

- [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)

### Set up the Environment

1. **Clone the repository**:
   ```bash
   git clone https://github.com/johnarman/shopping-cart-backend.git
   ```

2. **Install Dependencies**:
   Navigate to the project directory and run:
   ```bash
   dotnet restore
   ```

3. **Run Database Migrations**:
   Apply migrations to create the database and seed it with data:
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update

   or if you are executing commands through package manager console

   Add-Migration InitialCreate -Project ShoppingCart.Infrastructure -StartupProject ShoppingCart.API
   Update-Database -Project ShoppingCart.Infrastructure -StartupProject ShoppingCart.API
   ```

4. **Run the Application**:
   To start the backend application, use the command:
   ```bash
   dotnet run
   ```

### Default Admin User

The application will create a default admin user for login upon seeding:

- **Username**: `admin`
- **Password**: `admin123`

   OR

- **Username**: `testuser`
- **Password**: `test123`


### Swagger API Documentation

Once the application is running, you can access the Swagger UI for detailed API documentation and testing:

- [Swagger UI][(http://localhost:7109/swagger/index.html)]

Swagger provides interactive documentation for the available API endpoints.

### Testing

The project includes unit tests for key services. To run the tests, use the following command in the project directory:

```bash
dotnet test
```

### Project Structure

```plaintext
ShoppingCart.API/                    -> Main API project (contains controllers and startup configuration)
ShoppingCart.Infrastructure/         -> Data layer, including EF Core models, migrations, and repositories
ShoppingCart.Common/                 -> Shared utilities like JWT helpers, DTOs, etc.
ShoppingCart.Test/                   -> Unit tests for application services
ShoppingCart.Service/                -> Business logic related to shopping cart and products
ShoppingCart.Entity/                 -> Domain or Main data model
```
