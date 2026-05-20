-- ============================================================================
-- PROJECT: Inventory Data Engine
-- COMPONENT: Stored Procedure - Inventory Transaction Ledger (Phase 1)
-- TARGET ENGINE: Microsoft SQL Server 2022
-- ============================================================================

USE InventoryEngine;
GO

IF OBJECT_ID('dbo.sp_Inventory_RegisterTransaction', 'P') IS NOT NULL
BEGIN
    DROP PROCEDURE dbo.sp_Inventory_RegisterTransaction;
END;
GO

CREATE PROCEDURE dbo.sp_Inventory_RegisterTransaction
    @ProductID INT,
    @TransactionTypeID TINYINT,
    @Quantity INT,
    @ReferenceDocument NVARCHAR(100) = NULL,
    @Notes NVARCHAR(255) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- Local variables for business logic validation
    DECLARE @CurrentStock INT;
    DECLARE @TransactionSign INT;

    -- 1. PRE-VALIDATION: Verify product existence
    IF NOT EXISTS (SELECT 1 FROM dbo.Products WHERE ProductID = @ProductID)
    BEGIN
        RAISERROR('The specified ProductID does not exist.', 16, 1);
        RETURN;
    END;

    -- 2. PRE-VALIDATION: Verify transaction type and capture its sign
    SELECT @TransactionSign = Sign 
    FROM dbo.TransactionTypes 
    WHERE TransactionTypeID = @TransactionTypeID;

    IF @TransactionSign IS NULL
    BEGIN
        RAISERROR('Invalid TransactionTypeID.', 16, 1);
        RETURN;
    END;

    -- 3. BUSINESS RULE: Prevent stock from dropping below zero (Stockout Defense)
    -- Capture the current stock level
    SELECT @CurrentStock = Quantity 
    FROM dbo.Stocks 
    WHERE ProductID = @ProductID;

    -- If sign is negative (-1) and quantity to reduce is greater than available stock
    IF @TransactionSign = -1 AND (@CurrentStock + (@Quantity * @TransactionSign)) < 0
    BEGIN
        DECLARE @ErrorMsg NVARCHAR(255);
        SET @ErrorMsg = CONCAT('Inexistent Stock. Attempted to reduce ', @Quantity, ' units, but only ', @CurrentStock, ' are available.');
        RAISERROR(@ErrorMsg, 16, 1);
        RETURN;
    END;

    -- 4. ATOMIC INSERTION BLOCK
    BEGIN TRANSACTION;

    BEGIN TRY
        -- Insert into Kardex (The Trigger 'trg_InventoryTransactions_AfterInsert' will handle the stock update)
        INSERT INTO dbo.InventoryTransactions (
            ProductID, TransactionTypeID, Quantity, ReferenceDocument, Notes
        )
        VALUES (
            @ProductID, @TransactionTypeID, @Quantity, @ReferenceDocument, @Notes
        );

        COMMIT TRANSACTION;
        PRINT 'Transaction registered successfully and stock synchronized.';

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();

        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH;
END;
GO