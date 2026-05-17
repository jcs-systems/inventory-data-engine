# Inventory Data Engine

A high-performance, asynchronous backend data engine engineered for complex inventory management and logistics tracking. This system prioritizes strict transactional integrity, detailed audit logging, and efficient data processing.

## Architecture Overview

The system utilizes a distributed local architecture designed for isolated data management:
* **Development Workstation:** Client applications built on .NET 8.0 running on Windows.
* **Database Infrastructure:** Microsoft SQL Server 2022 deployed inside a Linux Docker container on a dedicated Ubuntu server node (192.168.100.19).
* **Communication Layer:** Secure TCP/IP connectivity via SQL Server Authentication over standard port 1433.

## Tech Stack

* **Backend Core:** .NET 8.0 (C#)
* **Database Engine:** Microsoft SQL Server 2022 (Developer Edition)
* **Infrastructure:** Docker / Ubuntu Linux
* **Data Access:** T-SQL / Stored Procedures / Triggers

## Core Engineering Focus

1. **Strict Audit Trail:** Automated historical logging using database triggers to enforce compliance and data lineage.
2. **Performance-Driven Logic:** Encapsulation of business rules directly within the database layer using advanced T-SQL and Stored Procedures to eliminate network overhead.
3. **Asynchronous Execution:** Optimized .NET asynchronous data streams for handling concurrent inventory operations.

---
*Status: Active Development - Infrastructure Verified.*