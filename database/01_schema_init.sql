-- ============================================================================
-- PROJECT: Inventory Data Engine
-- COMPONENT: Database Initialization and Core Schema (Phase 1)
-- TARGET ENGINE: Microsoft SQL Server 2022
-- ============================================================================

USE master;
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = N'InventoryEngine')
BEGIN
    ALTER DATABASE InventoryEngine SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE InventoryEngine;
END;
GO

CREATE DATABASE InventoryEngine;
GO

USE InventoryEngine;
GO

SET NOCOUNT ON;

-- ----------------------------------------------------------------------------
-- TABLE: Categories
-- ----------------------------------------------------------------------------
CREATE TABLE dbo.Categories (
    CategoryID INT IDENTITY(1,1) NOT NULL,
    CategoryName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(255) NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_Categories_IsActive DEFAULT (1),
    CreatedAt DATETIME2(3) NOT NULL CONSTRAINT DF_Categories_CreatedAt DEFAULT (SYSUTCDATETIME()),
    CONSTRAINT PK_Categories PRIMARY KEY CLUSTERED (CategoryID),
    CONSTRAINT UQ_Categories_Name UNIQUE (CategoryName)
);
GO

-- ----------------------------------------------------------------------------
-- TABLE: Products
-- ----------------------------------------------------------------------------
CREATE TABLE dbo.Products (
    ProductID INT IDENTITY(1,1) NOT NULL,
    SKU NVARCHAR(50) NOT NULL,
    ProductName NVARCHAR(150) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    CategoryID INT NOT NULL,
    Cost DECIMAL(18,4) NOT NULL CONSTRAINT CHK_Products_Cost CHECK (Cost >= 0),
    UnitPrice DECIMAL(18,4) NOT NULL CONSTRAINT CHK_Products_UnitPrice CHECK (UnitPrice >= 0),
    MinimumStock INT NOT NULL CONSTRAINT DF_Products_MinimumStock DEFAULT (0) CONSTRAINT CHK_Products_MinStock CHECK (MinimumStock >= 0),
    IsActive BIT NOT NULL CONSTRAINT DF_Products_IsActive DEFAULT (1),
    CreatedAt DATETIME2(3) NOT NULL CONSTRAINT DF_Products_CreatedAt DEFAULT (SYSUTCDATETIME()),
    CONSTRAINT PK_Products PRIMARY KEY CLUSTERED (ProductID),
    CONSTRAINT UQ_Products_SKU UNIQUE (SKU),
    CONSTRAINT FK_Products_Categories FOREIGN KEY (CategoryID) REFERENCES dbo.Categories (CategoryID)
);
GO

-- ----------------------------------------------------------------------------
-- TABLE: Stocks
-- ----------------------------------------------------------------------------
CREATE TABLE dbo.Stocks (
    StockID INT IDENTITY(1,1) NOT NULL,
    ProductID INT NOT NULL,
    Quantity INT NOT NULL CONSTRAINT DF_Stocks_Quantity DEFAULT (0),
    LastUpdated DATETIME2(3) NOT NULL CONSTRAINT DF_Stocks_LastUpdated DEFAULT (SYSUTCDATETIME()),
    CONSTRAINT PK_Stocks PRIMARY KEY CLUSTERED (StockID),
    CONSTRAINT UQ_Stocks_ProductID UNIQUE (ProductID),
    CONSTRAINT CHK_Stocks_Quantity CHECK (Quantity >= 0),
    CONSTRAINT FK_Stocks_Products FOREIGN KEY (ProductID) REFERENCES dbo.Products (ProductID) ON DELETE CASCADE
);
GO

-- ----------------------------------------------------------------------------
-- TABLE: TransactionTypes
-- ----------------------------------------------------------------------------
CREATE TABLE dbo.TransactionTypes (
    TransactionTypeID TINYINT IDENTITY(1,1) NOT NULL,
    TypeName NVARCHAR(50) NOT NULL,
    Sign INT NOT NULL CONSTRAINT CHK_TransactionTypes_Sign CHECK (Sign IN (1, -1)),
    CONSTRAINT PK_TransactionTypes PRIMARY KEY CLUSTERED (TransactionTypeID),
    CONSTRAINT UQ_TransactionTypes_Name UNIQUE (TypeName)
);
GO

-- ----------------------------------------------------------------------------
-- SEED DATA: TransactionTypes
-- ----------------------------------------------------------------------------
INSERT INTO dbo.TransactionTypes (TypeName, Sign) 
VALUES 
    ('Purchase', 1), 
    ('Sale', -1), 
    ('Adjustment_In', 1), 
    ('Adjustment_Out', -1);
GO