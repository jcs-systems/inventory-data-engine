# Inventory Data Engine

A high-performance, asynchronous backend data engine engineered for complex inventory management, Kardex transaction logging, and real-time stock alert automation. Built with .NET 8 Web API, Dapper, SQL Server stored procedures, and Brevo SMTP integration.

---

## Architecture & Tech Stack

* Core Framework: .NET 8.0 Web API (C#)
* Data Access Layer: Dapper (Micro-ORM) for high-speed mapping & ADO.NET execution
* Database Engine: Microsoft SQL Server 2022 (Stored Procedures & Triggers)
* Integration & Services: Brevo SMTP API / Relay for automated email alerts
* Security & Configuration: .NET User Secrets for isolated environment credentials
* Testing: Integrated HTTP Client scripts (api-tests.http)

---

## Key Features

1. Transactional Kardex Engine: Atomic registration of inventory movements (Inbound/Outbound) with strict document tracing and audit compliance.
2. Automated Minimum Stock Alerts: Real-time stock evaluation post-transaction. Triggers asynchronous email notifications via Brevo SMTP when inventory drops below defined thresholds.
3. Advanced Reporting Endpoints: Flexible Kardex history extraction supporting date-range filtering, product-level queries, and stock level summaries.
4. Production-Grade Error Handling: Centralized GlobalExceptionHandlerMiddleware for standardized JSON error payloads (HTTP 500) and structured server logging.
5. Secure Configuration Pipeline: Enforced separation of public configuration (appsettings.json) and sensitive keys via .NET User Secrets store.

---

## Getting Started

### Prerequisites
* .NET 8.0 SDK
* SQL Server 2022 (Local or Docker container)
* SMTP Credentials (e.g., Brevo)

### 1. Database Setup
Execute the DDL and Stored Procedures initialization script in your SQL Server instance:
database/00_script_create_tables_sp.sql

### 2. Configure Local User Secrets
Store sensitive parameters securely in your local secret store:

cd src/Inventory.WebApi

dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:SqlConnectionString" "Server=YOUR_SERVER;Database=YOUR_DB;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=True;"
dotnet user-secrets set "SmtpSettings:Username" "YOUR_SMTP_USERNAME"
dotnet user-secrets set "SmtpSettings:Password" "YOUR_SMTP_PASSWORD"
dotnet user-secrets set "SmtpSettings:SenderEmail" "YOUR_VERIFIED_SENDER_EMAIL"

### 3. Run the Application
dotnet run --project src/Inventory.WebApi/Inventory.WebApi.csproj

The API will start listening on http://localhost:5113.

---

## API Endpoints Summary

- POST /api/products : Registers a new product in the catalog
- POST /api/transactions : Logs Kardex movement & triggers stock alerts if low
- GET /api/stocks : Retrieves overall inventory levels
- GET /api/stocks/{productId} : Retrieves current stock for a specific product
- GET /api/transactions/report : Queries Kardex history (supports productId, startDate, endDate)

---
Status: Active Development - Exception Handling & Documentation Finalized.