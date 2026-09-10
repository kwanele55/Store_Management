# StoreManagement

A simple **ASP.NET Core MVC** web application for managing a store's categories, products, customers, and orders — built as a CRUD (Create, Read, Update, Delete) assignment using **Entity Framework Core** and **SQL Server**.

## Features

- Full CRUD operations (Create, Read, Update, Delete) for:
  - **Categories**
  - **Products**
  - **Customers**
  - **Orders**
- Relational data model with foreign keys and navigation properties (e.g. a Product belongs to a Category, an Order belongs to a Customer and contains Order Items)
- Server-side rendered views using Razor (`.cshtml`)
- Bootstrap-based responsive UI

## Tech Stack

- **Framework:** ASP.NET Core MVC (.NET 8.0)
- **ORM:** Entity Framework Core 8.0 (`Microsoft.EntityFrameworkCore.SqlServer`)
- **Database:** SQL Server (SQL Server Express by default)
- **Frontend:** Razor Views, Bootstrap, jQuery, jQuery Validation

## Project Structure

```
StoreManagement/
├── Controllers/
│   ├── CategoriesController.cs
│   ├── ProductsController.cs
│   ├── CustomersController.cs
│   ├── OrdersController.cs
│   └── HomeController.cs
├── Models/
│   ├── Category.cs
│   ├── Product.cs
│   ├── Customer.cs
│   ├── Order.cs
│   ├── OrderItem.cs
│   └── StoreManagementDbContext.cs
├── Views/
│   ├── Categories/
│   ├── Products/
│   ├── Customers/
│   ├── Orders/
│   └── Shared/
├── wwwroot/
├── appsettings.json
└── Program.cs
```

## Data Model

| Entity        | Key Fields                                                       | Relationships                          |
|---------------|--------------------------------------------------------------------|-----------------------------------------|
| **Category**  | CategoryId, CategoryName, Description, IsActive, CreatedDate       | Has many Products                       |
| **Product**   | ProductId, ProductName, Price, StockQty                            | Belongs to Category, has many OrderItems |
| **Customer**  | CustomerId, FirstName, LastName, Email, RegisteredDate              | Has many Orders                         |
| **Order**     | OrderId, OrderDate, TotalAmount, Status                            | Belongs to Customer, has many OrderItems |
| **OrderItem** | OrderItemId, Quantity, UnitPrice, LineTotal                        | Belongs to Order and Product            |

## Getting Started

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download)
- SQL Server or SQL Server Express (LocalDB also works)
- Visual Studio 2022 (recommended) or VS Code

### Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/kwanele55/Store_Management.git
   cd Store_Management
   ```

2. **Configure the database connection**

   Update the connection string in `appsettings.json` if needed:
   ```json
   "ConnectionStrings": {
     "StoreManagementDbContext": "Server=.\\SQLEXPRESS;Database=StoreManagementDB;Trusted_Connection=True;TrustServerCertificate=True;"
   }
   ```

3. **Apply database migrations** (if using EF Core migrations)
   ```bash
   dotnet ef database update
   ```

4. **Run the application**
   ```bash
   dotnet run
   ```

   Or press `F5` / `Ctrl+F5` in Visual Studio.

5. Open your browser to the URL shown in the console (e.g. `https://localhost:5001`).

## Usage

Navigate between the **Categories**, **Products**, **Customers**, and **Orders** sections from the app to view records in a list, and use the **Create**, **Edit**, **Details**, and **Delete** links on each to manage entries.

## Author

**Kwanele** — [kwanele55](https://github.com/kwanele55)
