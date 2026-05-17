-- ============================================================================
-- PROJECT: Inventory Data Engine
-- COMPONENT: Transactional Ledger (Kardex - Phase 1)
-- TARGET ENGINE: Microsoft SQL Server 2022
-- ============================================================================

USE InventoryEngine;
GO

SET NOCOUNT ON;

-- ----------------------------------------------------------------------------
-- TABLE: InventoryTransactions (The Historical Kardex)
-- ----------------------------------------------------------------------------
IF OBJECT_ID('dbo.InventoryTransactions', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.InventoryTransactions (
        TransactionID BIGINT IDENTITY(1,1) NOT NULL,
        ProductID INT NOT NULL,
        TransactionTypeID TINYINT NOT NULL,
        Quantity INT NOT NULL,
        TransactionDate DATETIME2(3) NOT NULL CONSTRAINT DF_InventoryTransactions_Date DEFAULT (SYSUTCDATETIME()),
        ReferenceDocument NVARCHAR(100) NULL,
        Notes NVARCHAR(255) NULL,
        CONSTRAINT PK_InventoryTransactions PRIMARY KEY CLUSTERED (TransactionID),
        CONSTRAINT CHK_InventoryTransactions_Quantity CHECK (Quantity > 0),
        CONSTRAINT FK_InventoryTransactions_Products FOREIGN KEY (ProductID) REFERENCES dbo.Products (ProductID),
        CONSTRAINT FK_InventoryTransactions_TransactionTypes FOREIGN KEY (TransactionTypeID) REFERENCES dbo.TransactionTypes (TransactionTypeID)
    );

    CREATE NONCLUSTERED INDEX IX_InventoryTransactions_ProductID_Date 
    ON dbo.InventoryTransactions (ProductID, TransactionDate)
    INCLUDE (TransactionTypeID, Quantity);
END;
GO