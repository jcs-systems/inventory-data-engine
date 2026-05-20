-- ============================================================================
-- PROJECT: Inventory Data Engine
-- COMPONENT: Stored Procedure - Product Creation Ledger (Phase 1)
-- TARGET ENGINE: Microsoft SQL Server 2022
-- ============================================================================

USE InventoryEngine;
GO

IF OBJECT_ID('dbo.sp_Products_Create', 'P') IS NOT NULL
BEGIN
    DROP PROCEDURE dbo.sp_Products_Create;
END;
GO

CREATE PROCEDURE dbo.sp_Products_Create
    @SKU NVARCHAR(50),
    @ProductName NVARCHAR(150),
    @Description NVARCHAR(MAX) = NULL,
    @CategoryID INT,
    @Cost DECIMAL(18,4),
    @UnitPrice DECIMAL(18,4),
    @MinimumStock INT = 0
AS
BEGIN
    SET NOCOUNT ON;

    -- Local variable to capture the auto-generated ProductID
    DECLARE @NewProductID INT;

    -- 1. PRE-VALIDATION: Check if SKU already exists to prevent transaction overhead
    IF EXISTS (SELECT 1 FROM dbo.Products WHERE SKU = @SKU)
    BEGIN
        RAISERROR('The product SKU already exists in the system.', 16, 1);
        RETURN;
    END;

    -- 2. ATOMIC TRANSACTION BLOCK
    BEGIN TRANSACTION;

    BEGIN TRY
        -- Step A: Insert the master product record
        INSERT INTO dbo.Products (
            SKU, ProductName, Description, CategoryID, Cost, UnitPrice, MinimumStock
        )
        VALUES (
            @SKU, @ProductName, @Description, @CategoryID, @Cost, @UnitPrice, @MinimumStock
        );

        -- Capture the IDENTITY value generated in the scope of this connection
        SET @NewProductID = SCOPE_IDENTITY();

        -- Step B: Insert corresponding stock record initialization
        INSERT INTO dbo.Stocks (ProductID, Quantity)
        VALUES (@NewProductID, 0);

        -- If both inserts succeed without constraints violations, commit changes
        COMMIT TRANSACTION;

        -- Return the generated ID for backend consumption (.NET metadata mapping)
        SELECT @NewProductID AS GeneratedProductID;

    END TRY
    BEGIN CATCH
        -- Rollback any modification if an unexpected error occurs during execution
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        -- Bubble up the original error details to the calling application (C#)
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();

        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH;
END;
GO