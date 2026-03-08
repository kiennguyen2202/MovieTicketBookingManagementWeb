# Movie Ticket Booking Management System

## Overview
A full-stack, enterprise-ready web application developed using ASP.NET Core 9.0 MVC. This system provides a comprehensive movie ticket booking platform, offering end-users a seamless experience for browsing movies, selecting seats in real-time, and purchasing tickets alongside concession items. Additionally, it features a secure, role-based administrative dashboard for managing cinema locations, showtimes, user accounts, and financial transactions.

The project demonstrates proficiency in modern .NET web development, full-stack architecture design, and the implementation of scalable, production-ready systems.

## Architecture & Technology Stack
- **Framework:** .NET 9.0 (ASP.NET Core MVC)
- **Language:** C#
- **Database:** Microsoft SQL Server
- **ORM:** Entity Framework Core 9.0 (Code-First approach)
- **Identity & Security:** ASP.NET Core Identity for Authentication and Role-based Authorization
- **Frontend Layer:** HTML5, CSS3, JavaScript, Razor Views, Bootstrap
- **State Management:** Distributed Memory Cache, Session Management
- **Design Patterns:** Model-View-Controller (MVC), Repository/Service Pattern, Dependency Injection (DI)

## Key Features

### Customer Portal
- **Movie Catalog:** Browse current and upcoming movies with detailed information, integrated trailers, and user reviews.
- **Dynamic Seat Booking:** Interactive, real-time seat selection interface mapped to specific screening rooms and showtimes.
- **Concession Integration:** Add on F&B items (popcorn, drinks) directly to the booking cart.
- **Checkout & Payment:** Secure processing of payment information and transaction tracking.
- **User Dashboard:** Profile management, booking history tracking, and interactive movie review capabilities.

### Administrative Dashboard
- **System Analytics:** Overview of cumulative sales, ticket volumes, and system performance metrics.
- **Cinema & Infrastructure Management:** Multi-location management (e.g., CGV, Lotte, Galaxy) handling respective screening rooms, integrated with Google Maps APIs.
- **Content Management System (CMS):** CRUD operations for movies, genres, and scheduling of complex showtimes.
- **Transaction Handling:** Monitor, process, and manage customer orders and payment statuses.
- **Access Control:** Strictly enforced route protection and functionality isolation for administrative staff.

## Project Structure & Implementation Details
- **Service Layer:** Business validation and complex logic are extracted into dedicated, scalable services (e.g., `PaymentInfoService`, `CinemaService`, `TransactionCodeService`) to keep controllers lean and testable.
- **Data Seeding:** Automated database initialization utilizing Entity Framework Core, provisioning default administrative roles, permissions, and an extensive schema of regional cinema data.
- **Relational Schema:** Optimized, normalized database design handling intricate entity relationships across Movies, Showtimes, Rooms, Seats, Tickets, and Transactions.

## Getting Started

### Prerequisites
- .NET 9.0 SDK
- Microsoft SQL Server
- Visual Studio 2022 / Visual Studio Code

### Local Environment Setup
1. **Clone the repository:**
   ```bash
   git clone https://github.com/kiennguyen2202/MovieTicketBookingManagementWeb.git
   cd MovieTicketBookingManagementWeb
   ```

2. **Configure Database Connection:**
   Update the `DefaultConnection` string in `appsettings.json` / `appsettings.Development.json` to point to your local SQL Server instance.
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=MovieTicketDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
   }
   ```

3. **Database Migration & Initialization:**
   Apply Entity Framework migrations to generate the database schema.
   ```bash
   dotnet ef database update
   ```

4. **Launch the Application:**
   ```bash
   dotnet run
   ```


*Note: This repository serves as a portfolio piece showcasing backend architecture, database design, and end-to-end full-stack development capabilities using the .NET ecosystem.*
