-- ============================================================================
-- PROJECT: Inventory Data Engine
-- COMPONENT: Inventory Synchronization Trigger (Phase 1)
-- TARGET ENGINE: Microsoft SQL Server 2022
-- ============================================================================

USE InventoryEngine;
GO

IF OBJECT_ID('dbo.trg_InventoryTransactions_AfterInsert', 'TR') IS NOT NULL
BEGIN
    DROP TRIGGER dbo.trg_InventoryTransactions_AfterInsert;
END;
GO

CREATE TRIGGER dbo.trg_InventoryTransactions_AfterInsert
ON dbo.InventoryTransactions
AFTER INSERT
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM inserted) RETURN;

    SET NOCOUNT ON;

    BEGIN TRY
        UPDATE s
        SET s.Quantity = s.Quantity + (i.Quantity * tt.Sign),
            s.LastUpdated = SYSUTCDATETIME()
        FROM dbo.Stocks s
        INNER JOIN inserted i ON s.ProductID = i.ProductID
        INNER JOIN dbo.TransactionTypes tt ON i.TransactionTypeID = tt.TransactionTypeID;

    END TRY
    BEGIN CATCH
        THROW;
    END CATCH;
END;
GO