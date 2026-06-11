-- ====================================================================
-- KỊCH BẢN TẠO CƠ SỞ DỮ LIỆU QUẢN LÝ KHO (WMS)
-- ====================================================================
-- Mục đích: Khởi tạo toàn bộ cấu trúc DB, Trigger, Procedure và dữ liệu mẫu
-- ====================================================================

-- ====================================================================
-- PHẦN 1. KHỞI TẠO DATABASE (CHẠY 1 LẦN DUY NHẤT LÚC BẮT ĐẦU DỰ ÁN)
-- ====================================================================
USE master;
GO

-- Xóa database cũ nếu đã tồn tại để làm sạch môi trường
IF EXISTS (SELECT name FROM sys.databases WHERE name = N'QuanLyKhoDB')
BEGIN
    ALTER DATABASE QuanLyKhoDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE QuanLyKhoDB;
END
GO

-- Tạo mới database
CREATE DATABASE QuanLyKhoDB;
GO

USE QuanLyKhoDB;
GO

-- =================================================================
-- PHẦN 2. TẠO CẤU TRÚC BẢNG (TABLES) - DANH MỤC NỀN TẢNG (MASTER DATA)
-- Chú ý: Các bảng độc lập tạo trước, bảng có khóa ngoại (Foreign Key) tạo sau
-- =================================================================

-- 2.1. Bảng Người dùng hệ thống
CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Username VARCHAR(50) NOT NULL UNIQUE,
    PasswordHash VARCHAR(255) NOT NULL,
    FullName NVARCHAR(100) NOT NULL,
    Email VARCHAR(100) NOT NULL UNIQUE,
    Role NVARCHAR(20) NOT NULL CONSTRAINT CK_Users_Role CHECK (Role IN (N'quản lý', N'nhân viên')),
    IsActive BIT NOT NULL DEFAULT 1
);

-- 2.2. Bảng Nhà cung cấp
CREATE TABLE Suppliers (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(150) NOT NULL,
    Phone VARCHAR(20) NULL,
    Email VARCHAR(100) NULL,
    Address NVARCHAR(250) NULL,
    IsActive BIT NOT NULL DEFAULT 1
);

-- 2.3. Bảng Khách hàng / Đại lý
CREATE TABLE Customers (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(150) NOT NULL,
    Phone VARCHAR(20) NULL,
    Email VARCHAR(100) NULL,
    Address NVARCHAR(250) NULL,
    IsActive BIT NOT NULL DEFAULT 1
);

-- 2.4. Bảng Danh sách Kho hàng
CREATE TABLE Warehouses (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(150) NOT NULL,
    Address NVARCHAR(250) NULL,
    IsActive BIT NOT NULL DEFAULT 1
);

-- 2.5. Bảng Danh mục Sản phẩm
CREATE TABLE Products (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    SKU VARCHAR(50) NOT NULL UNIQUE,
    Barcode VARCHAR(50) NULL UNIQUE,
    Name NVARCHAR(200) NOT NULL,
    Category NVARCHAR(100) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1
);

-- 2.6. Bảng Lưu vết Hệ thống (Audit Logs)
-- Dùng để lưu lại lịch sử thay đổi trạng thái của các phiếu xuất/nhập/chuyển
CREATE TABLE AuditLogs (
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    Action VARCHAR(50) NOT NULL,
    TableName VARCHAR(100) NOT NULL,
    RecordId INT NOT NULL,
    OldData NVARCHAR(MAX) NULL,
    NewData NVARCHAR(MAX) NULL,
    Timestamp DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);

-- =================================================================
-- PHẦN 3. BẢNG TRÁI TIM HỆ THỐNG: TỒN KHO & BIẾN ĐỘNG (TRANSACTIONS)
-- =================================================================

-- 3.1. Bảng Tồn kho hiện tại (Inventory)
CREATE TABLE Inventory (
    WarehouseId INT NOT NULL,
    ProductId INT NOT NULL,
    Quantity INT NOT NULL DEFAULT 0,
    MinQuantity INT NOT NULL DEFAULT 10,   -- Mức tồn kho tối thiểu cảnh báo
    MaxQuantity INT NOT NULL DEFAULT 1000, -- Mức tồn kho tối đa cho phép
    UpdatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    
    PRIMARY KEY (WarehouseId, ProductId),
    FOREIGN KEY (WarehouseId) REFERENCES Warehouses(Id),
    FOREIGN KEY (ProductId) REFERENCES Products(Id),
    CONSTRAINT CK_Inventory_Quantity CHECK (Quantity >= 0),
    CONSTRAINT CK_Inventory_MinMax CHECK (MaxQuantity >= MinQuantity)
);

-- 3.2. Bảng Sổ cái kho (Lịch sử biến động từng mặt hàng)
CREATE TABLE InventoryTransactions (
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,
    WarehouseId INT NOT NULL,
    ProductId INT NOT NULL,
    TransactionType VARCHAR(20) NOT NULL, -- INBOUND, OUTBOUND, TRANSFER_IN, TRANSFER_OUT
    ReferenceCode VARCHAR(50) NOT NULL,   -- Mã phiếu tham chiếu
    QuantityChanged INT NOT NULL,         -- Số lượng thay đổi (dương hoặc âm)
    BeforeQuantity INT NOT NULL,          -- Tồn đầu
    AfterQuantity INT NOT NULL,           -- Tồn cuối
    CreatedBy INT NOT NULL,
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
    
    FOREIGN KEY (WarehouseId) REFERENCES Warehouses(Id),
    FOREIGN KEY (ProductId) REFERENCES Products(Id),
    FOREIGN KEY (CreatedBy) REFERENCES Users(Id),
    CONSTRAINT CK_InvTransactions_Type CHECK (TransactionType IN ('INBOUND', 'OUTBOUND', 'TRANSFER_OUT', 'TRANSFER_IN')),
    CONSTRAINT CK_InvTransactions_Qty CHECK (QuantityChanged > 0)
);

-- =================================================================
-- PHẦN 4. BẢNG NGHIỆP VỤ (ORDERS - NHẬP / XUẤT / CHUYỂN KHO)
-- =================================================================

-- 4.1. Nghiệp vụ Nhập Kho (Inbound)
CREATE TABLE InboundOrders (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    InboundCode VARCHAR(50) NOT NULL UNIQUE,
    WarehouseId INT NOT NULL,
    SupplierId INT NOT NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT N'chờ nhập',
    CreatedBy INT NOT NULL,
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
    ProcessedBy INT NULL,
    ProcessedDate DATETIME NULL,
    Note NVARCHAR(250) NULL,
    
    FOREIGN KEY (WarehouseId) REFERENCES Warehouses(Id),
    FOREIGN KEY (SupplierId) REFERENCES Suppliers(Id),
    FOREIGN KEY (CreatedBy) REFERENCES Users(Id),
    FOREIGN KEY (ProcessedBy) REFERENCES Users(Id),
    CONSTRAINT CK_InboundOrders_Status CHECK (Status IN (N'chờ nhập', N'đã nhập', N'đã hủy'))
);

CREATE TABLE InboundOrderDetails (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    InboundOrderId INT NOT NULL,
    ProductId INT NOT NULL,
    Quantity INT NOT NULL,
    Price DECIMAL(18,2) NOT NULL DEFAULT 0,
    
    FOREIGN KEY (InboundOrderId) REFERENCES InboundOrders(Id),
    FOREIGN KEY (ProductId) REFERENCES Products(Id),
    CONSTRAINT CK_InboundOrderDetails_Quantity CHECK (Quantity > 0)
);

-- 4.2. Nghiệp vụ Xuất Kho (Outbound)
CREATE TABLE OutboundOrders (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    OutboundCode VARCHAR(50) NOT NULL UNIQUE,
    WarehouseId INT NOT NULL,
    CustomerId INT NOT NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT N'chờ xuất',
    CreatedBy INT NOT NULL,
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
    ProcessedBy INT NULL,
    ProcessedDate DATETIME NULL,
    Note NVARCHAR(250) NULL,
    
    FOREIGN KEY (WarehouseId) REFERENCES Warehouses(Id),
    FOREIGN KEY (CustomerId) REFERENCES Customers(Id),
    FOREIGN KEY (CreatedBy) REFERENCES Users(Id),
    FOREIGN KEY (ProcessedBy) REFERENCES Users(Id),
    CONSTRAINT CK_OutboundOrders_Status CHECK (Status IN (N'chờ xuất', N'đã xuất', N'đã hủy'))
);

CREATE TABLE OutboundOrderDetails (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    OutboundOrderId INT NOT NULL,
    ProductId INT NOT NULL,
    Quantity INT NOT NULL,
    Price DECIMAL(18,2) NOT NULL DEFAULT 0,
    
    FOREIGN KEY (OutboundOrderId) REFERENCES OutboundOrders(Id),
    FOREIGN KEY (ProductId) REFERENCES Products(Id),
    CONSTRAINT CK_OutboundOrderDetails_Quantity CHECK (Quantity > 0)
);

-- 4.3. Nghiệp vụ Chuyển Kho Nội Bộ (Transfer)
CREATE TABLE TransferOrders (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    TransferCode VARCHAR(50) NOT NULL UNIQUE,
    FromWarehouseId INT NOT NULL,
    ToWarehouseId INT NOT NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT N'chờ duyệt',
    CreatedBy INT NOT NULL,
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
    ProcessedBy INT NULL, 
    ProcessedDate DATETIME NULL,
    
    FOREIGN KEY (FromWarehouseId) REFERENCES Warehouses(Id),
    FOREIGN KEY (ToWarehouseId) REFERENCES Warehouses(Id),
    FOREIGN KEY (CreatedBy) REFERENCES Users(Id),
    FOREIGN KEY (ProcessedBy) REFERENCES Users(Id),
    
    CONSTRAINT CK_TransferOrders_Status CHECK (Status IN (N'chờ duyệt', N'đang chuyển', N'đã nhận')),
    CONSTRAINT CK_TransferOrders_Warehouses CHECK (FromWarehouseId <> ToWarehouseId)
);

CREATE TABLE TransferOrderDetails (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    TransferOrderId INT NOT NULL,
    ProductId INT NOT NULL,
    RequestedQty INT NOT NULL,  -- Số lượng yêu cầu xuất đi
    ActualQty INT NULL,         -- Số lượng thực tế kho đích nhận được
    
    FOREIGN KEY (TransferOrderId) REFERENCES TransferOrders(Id),
    FOREIGN KEY (ProductId) REFERENCES Products(Id),
    CONSTRAINT CK_TransferOrderDetails_ReqQty CHECK (RequestedQty > 0),
    CONSTRAINT CK_TransferOrderDetails_ActQty CHECK (ActualQty >= 0)
);

-- 4.4. Bảng quản lý Hao hụt khi chuyển kho
CREATE TABLE StockLosses (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    TransferOrderId INT NOT NULL,
    ProductId INT NOT NULL,
    LossQty INT NOT NULL,
    Reason NVARCHAR(250) NULL,
    ResolvedStatus BIT NOT NULL DEFAULT 0, -- 0: Chưa xử lý, 1: Đã xử lý đền bù/trừ kho
    
    FOREIGN KEY (TransferOrderId) REFERENCES TransferOrders(Id),
    FOREIGN KEY (ProductId) REFERENCES Products(Id),
    CONSTRAINT CK_StockLosses_Qty CHECK (LossQty > 0)
);
GO

-- =================================================================
-- PHẦN 5. LẬP TRÌNH CƠ SỞ DỮ LIỆU (FUNCTIONS & VIEWS)
-- =================================================================

-- 5.1. Hàm đánh giá trạng thái tồn kho (An toàn / Cảnh báo)
CREATE FUNCTION dbo.fn_GetStockAlertStatus (@Quantity INT, @MinQuantity INT, @MaxQuantity INT)
RETURNS NVARCHAR(50)
AS
BEGIN
    DECLARE @AlertStatus NVARCHAR(50) = N'An toàn';
    IF @Quantity = 0 SET @AlertStatus = N'Hết hàng';
    ELSE IF @Quantity < @MinQuantity SET @AlertStatus = N'Dưới hạn mức';
    ELSE IF @Quantity > @MaxQuantity SET @AlertStatus = N'Vượt hạn mức';
    RETURN @AlertStatus;
END;
GO

-- 5.2. View báo cáo tổng quan tồn kho (Dashboard)
CREATE VIEW dbo.v_InventoryDashboardAlert
AS
SELECT 
    w.Id AS WarehouseId, w.Name AS WarehouseName, p.Id AS ProductId, p.SKU, p.Name AS ProductName, p.Category,
    i.Quantity AS CurrentStock, i.MinQuantity, i.MaxQuantity,
    dbo.fn_GetStockAlertStatus(i.Quantity, i.MinQuantity, i.MaxQuantity) AS StockStatusText
FROM Inventory i
INNER JOIN Warehouses w ON i.WarehouseId = w.Id
INNER JOIN Products p ON i.ProductId = p.Id;
GO

-- 5.3. View Báo cáo biến động kho hằng ngày
CREATE VIEW dbo.v_DailyInventoryLedgerReport
AS
SELECT 
    CONVERT(DATE, CreatedDate) AS TransactionDate, WarehouseId, ProductId, TransactionType,
    ReferenceCode, QuantityChanged, BeforeQuantity, AfterQuantity, CreatedBy
FROM InventoryTransactions;
GO

-- =================================================================
-- PHẦN 6. LẬP TRÌNH CƠ SỞ DỮ LIỆU (STORED PROCEDURES)
-- =================================================================

-- 6.1. Thủ tục xác nhận NHẬP KHO (Xử lý Transactions an toàn)
CREATE PROCEDURE dbo.sp_ConfirmInboundOrder
    @InboundOrderId INT, @ProcessedBy INT, @IsSuccess BIT OUTPUT, @Message NVARCHAR(250) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @CurrentStatus NVARCHAR(20), @WarehouseId INT, @InboundCode VARCHAR(50);
    
    -- Lấy thông tin phiếu nhập
    SELECT @CurrentStatus = Status, @WarehouseId = WarehouseId, @InboundCode = InboundCode 
    FROM InboundOrders WHERE Id = @InboundOrderId;
    
    -- Validate trạng thái
    IF @CurrentStatus IS NULL OR @CurrentStatus <> N'chờ nhập'
    BEGIN
        SET @IsSuccess = 0; SET @Message = N'Thất bại: Phiếu không hợp lệ hoặc đã xử lý!'; RETURN;
    END

    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Bước 1: Khởi tạo dòng tồn kho = 0 cho mặt hàng mới chưa từng có trong kho
        INSERT INTO Inventory (WarehouseId, ProductId, Quantity, MinQuantity, MaxQuantity, UpdatedAt)
        SELECT DISTINCT @WarehouseId, det.ProductId, 0, 10, 1000, GETDATE()
        FROM InboundOrderDetails det WHERE det.InboundOrderId = @InboundOrderId
        AND NOT EXISTS (SELECT 1 FROM Inventory WHERE WarehouseId = @WarehouseId AND ProductId = det.ProductId);

        DECLARE @InboundLogs TABLE (ProductId INT, BeforeQty INT, AfterQty INT);

        -- Bước 2: Cộng số lượng vào bảng Inventory và xuất ra biến tạm @InboundLogs để ghi sổ
        UPDATE inv SET inv.Quantity = inv.Quantity + src.TotalQty, inv.UpdatedAt = GETDATE()
        OUTPUT inserted.ProductId, deleted.Quantity, inserted.Quantity INTO @InboundLogs(ProductId, BeforeQty, AfterQty)
        FROM Inventory inv
        INNER JOIN (SELECT ProductId, SUM(Quantity) AS TotalQty FROM InboundOrderDetails WHERE InboundOrderId = @InboundOrderId GROUP BY ProductId) src ON inv.ProductId = src.ProductId
        WHERE inv.WarehouseId = @WarehouseId;

        -- Bước 3: Ghi Sổ cái (Lịch sử biến động)
        INSERT INTO InventoryTransactions (WarehouseId, ProductId, TransactionType, ReferenceCode, QuantityChanged, BeforeQuantity, AfterQuantity, CreatedBy, CreatedDate)
        SELECT @WarehouseId, ProductId, 'INBOUND', @InboundCode, (AfterQty - BeforeQty), BeforeQty, AfterQty, @ProcessedBy, GETDATE() FROM @InboundLogs;

        -- Bước 4: Chuyển trạng thái phiếu nhập
        UPDATE InboundOrders SET Status = N'đã nhập', ProcessedBy = @ProcessedBy, ProcessedDate = GETDATE() WHERE Id = @InboundOrderId;

        COMMIT TRANSACTION;
        SET @IsSuccess = 1; SET @Message = N'Nhập kho thành công!';
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        SET @IsSuccess = 0; SET @Message = ERROR_MESSAGE();
    END CATCH
END;
GO

-- 6.2. Thủ tục xác nhận XUẤT KHO (Xử lý Transactions an toàn)
CREATE PROCEDURE dbo.sp_ConfirmOutboundOrder
    @OutboundOrderId INT, @ProcessedBy INT, @IsSuccess BIT OUTPUT, @Message NVARCHAR(250) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @CurrentStatus NVARCHAR(20), @WarehouseId INT, @OutboundCode VARCHAR(50);
    
    SELECT @CurrentStatus = Status, @WarehouseId = WarehouseId, @OutboundCode = OutboundCode 
    FROM OutboundOrders WHERE Id = @OutboundOrderId;
    
    IF @CurrentStatus IS NULL OR @CurrentStatus <> N'chờ xuất'
    BEGIN
        SET @IsSuccess = 0; SET @Message = N'Thất bại: Phiếu không hợp lệ hoặc đã xử lý!'; RETURN;
    END

    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Bước 1: Kiểm tra chống xuất Âm kho (Khối lượng tồn không đủ)
        IF EXISTS (
            SELECT 1 FROM (SELECT ProductId, SUM(Quantity) AS ReqQty FROM OutboundOrderDetails WHERE OutboundOrderId = @OutboundOrderId GROUP BY ProductId) det
            LEFT JOIN Inventory inv ON inv.ProductId = det.ProductId AND inv.WarehouseId = @WarehouseId
            WHERE det.ReqQty > ISNULL(inv.Quantity, 0)
        )
        BEGIN
            SET @IsSuccess = 0; SET @Message = N'Thất bại: Tồn kho hiện tại không đủ đáp ứng số lượng xuất!';
            ROLLBACK TRANSACTION; RETURN;
        END

        DECLARE @OutboundLogs TABLE (ProductId INT, BeforeQty INT, AfterQty INT);

        -- Bước 2: Trừ số lượng tồn kho
        UPDATE inv SET inv.Quantity = inv.Quantity - src.TotalQty, inv.UpdatedAt = GETDATE()
        OUTPUT inserted.ProductId, deleted.Quantity, inserted.Quantity INTO @OutboundLogs(ProductId, BeforeQty, AfterQty)
        FROM Inventory inv
        INNER JOIN (SELECT ProductId, SUM(Quantity) AS TotalQty FROM OutboundOrderDetails WHERE OutboundOrderId = @OutboundOrderId GROUP BY ProductId) src ON inv.ProductId = src.ProductId
        WHERE inv.WarehouseId = @WarehouseId;

        -- Bước 3: Ghi Sổ cái (Lịch sử biến động)
        INSERT INTO InventoryTransactions (WarehouseId, ProductId, TransactionType, ReferenceCode, QuantityChanged, BeforeQuantity, AfterQuantity, CreatedBy, CreatedDate)
        SELECT @WarehouseId, ProductId, 'OUTBOUND', @OutboundCode, (BeforeQty - AfterQty), BeforeQty, AfterQty, @ProcessedBy, GETDATE() FROM @OutboundLogs;

        -- Bước 4: Chuyển trạng thái phiếu xuất
        UPDATE OutboundOrders SET Status = N'đã xuất', ProcessedBy = @ProcessedBy, ProcessedDate = GETDATE() WHERE Id = @OutboundOrderId;

        COMMIT TRANSACTION;
        SET @IsSuccess = 1; SET @Message = N'Xuất kho thành công!';
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        SET @IsSuccess = 0; SET @Message = ERROR_MESSAGE();
    END CATCH
END;
GO

-- =================================================================
-- PHẦN 7. TRIGGERS (TỰ ĐỘNG HÓA VÀ GHI LOGS)
-- =================================================================

-- 7.1. Trigger nghiệp vụ luân chuyển kho & Ghi nhận hao hụt
CREATE TRIGGER dbo.trg_OnTransferStatusChange
ON TransferOrders
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT UPDATE(Status) RETURN;

    DECLARE @OrderId INT, @OldStatus NVARCHAR(20), @NewStatus NVARCHAR(20);
    DECLARE @FromWh INT, @ToWh INT, @Code VARCHAR(50), @User INT;
    
    SELECT @OrderId = i.Id, @OldStatus = d.Status, @NewStatus = i.Status, @FromWh = i.FromWarehouseId, @ToWh = i.ToWarehouseId, @Code = i.TransferCode, @User = i.ProcessedBy
    FROM inserted i INNER JOIN deleted d ON i.Id = d.Id;

    -- Chặn thay đổi nếu đơn đã chốt hoặc hủy
    IF (@OldStatus = N'đã nhận' OR @OldStatus = N'đã hủy')
    BEGIN
        ;THROW 51000, N'Lỗi: Phiếu đã đóng, không thể sửa trạng thái!', 1; RETURN;
    END

    -- GIAI ĐOẠN 1: KHO NGUỒN XUẤT ĐI (Trạng thái -> 'đang chuyển')
    IF (@OldStatus = N'chờ duyệt' AND @NewStatus = N'đang chuyển')
    BEGIN
        IF @User IS NULL BEGIN ;THROW 51003, N'Lỗi: Thiếu thông tin người duyệt!', 1; RETURN; END

        -- Kiểm tra âm kho
        IF EXISTS (
            SELECT 1 FROM (SELECT ProductId, SUM(RequestedQty) AS ReqQty FROM TransferOrderDetails WHERE TransferOrderId = @OrderId GROUP BY ProductId) det
            LEFT JOIN Inventory inv ON inv.ProductId = det.ProductId AND inv.WarehouseId = @FromWh
            WHERE det.ReqQty > ISNULL(inv.Quantity, 0)
        )
        BEGIN
            ;THROW 51001, N'Lỗi: Kho xuất không đủ hàng!', 1; RETURN;
        END

        DECLARE @OutLogs TABLE (ProductId INT, BeforeQty INT, AfterQty INT);

        UPDATE inv SET inv.Quantity = inv.Quantity - src.Qty, inv.UpdatedAt = GETDATE()
        OUTPUT inserted.ProductId, deleted.Quantity, inserted.Quantity INTO @OutLogs(ProductId, BeforeQty, AfterQty)
        FROM Inventory inv
        INNER JOIN (SELECT ProductId, SUM(RequestedQty) AS Qty FROM TransferOrderDetails WHERE TransferOrderId = @OrderId GROUP BY ProductId) src ON inv.ProductId = src.ProductId
        WHERE inv.WarehouseId = @FromWh;

        INSERT INTO InventoryTransactions (WarehouseId, ProductId, TransactionType, ReferenceCode, QuantityChanged, BeforeQuantity, AfterQuantity, CreatedBy, CreatedDate)
        SELECT @FromWh, ProductId, 'TRANSFER_OUT', @Code, (BeforeQty - AfterQty), BeforeQty, AfterQty, @User, GETDATE() FROM @OutLogs;
    END

    -- GIAI ĐOẠN 2: KHO ĐÍCH NHẬN HÀNG VÀ XỬ LÝ HAO HỤT (Trạng thái -> 'đã nhận')
    IF (@OldStatus = N'đang chuyển' AND @NewStatus = N'đã nhận')
    BEGIN
        IF @User IS NULL BEGIN ;THROW 51004, N'Lỗi: Thiếu thông tin người nhận!', 1; RETURN; END

        -- Bắt buộc phải có ActualQty (Nhập từ giao diện phần mềm)
        IF EXISTS (SELECT 1 FROM TransferOrderDetails WHERE TransferOrderId = @OrderId AND ActualQty IS NULL)
        BEGIN
            ;THROW 51005, N'Lỗi: Vui lòng nhập Số lượng Thực nhận (ActualQty) trước khi chốt đơn!', 1; RETURN;
        END

        -- Khởi tạo mặt hàng nếu kho đích chưa có
        INSERT INTO Inventory (WarehouseId, ProductId, Quantity, MinQuantity, MaxQuantity, UpdatedAt)
        SELECT DISTINCT @ToWh, det.ProductId, 0, 10, 1000, GETDATE()
        FROM TransferOrderDetails det WHERE det.TransferOrderId = @OrderId
        AND NOT EXISTS (SELECT 1 FROM Inventory WHERE WarehouseId = @ToWh AND ProductId = det.ProductId);

        DECLARE @InLogs TABLE (ProductId INT, BeforeQty INT, AfterQty INT);

        -- Cộng kho đích theo số lượng THỰC NHẬN (ActualQty)
        UPDATE inv SET inv.Quantity = inv.Quantity + src.Qty, inv.UpdatedAt = GETDATE()
        OUTPUT inserted.ProductId, deleted.Quantity, inserted.Quantity INTO @InLogs(ProductId, BeforeQty, AfterQty)
        FROM Inventory inv
        INNER JOIN (SELECT ProductId, SUM(ActualQty) AS Qty FROM TransferOrderDetails WHERE TransferOrderId = @OrderId GROUP BY ProductId) src ON inv.ProductId = src.ProductId
        WHERE inv.WarehouseId = @ToWh;

        INSERT INTO InventoryTransactions (WarehouseId, ProductId, TransactionType, ReferenceCode, QuantityChanged, BeforeQuantity, AfterQuantity, CreatedBy, CreatedDate)
        SELECT @ToWh, ProductId, 'TRANSFER_IN', @Code, (AfterQty - BeforeQty), BeforeQty, AfterQty, @User, GETDATE() FROM @InLogs;

        -- Tự động ghi nhận hao hụt vào bảng StockLosses nếu Xuất > Thực nhận
        INSERT INTO StockLosses (TransferOrderId, ProductId, LossQty, Reason, ResolvedStatus)
        SELECT @OrderId, ProductId, (RequestedQty - ActualQty), N'Hệ thống tự động ghi nhận hao hụt do nhận thiếu hàng', 0
        FROM TransferOrderDetails
        WHERE TransferOrderId = @OrderId AND RequestedQty > ActualQty;
    END
END;
GO

-- 7.2. Trigger Audit Lịch sử Phiếu Nhập
CREATE TRIGGER dbo.trg_AuditInboundOrders
ON InboundOrders
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF UPDATE(Status)
    BEGIN
        INSERT INTO AuditLogs (UserId, Action, TableName, RecordId, OldData, NewData, Timestamp)
        SELECT 
            ISNULL(i.ProcessedBy, i.CreatedBy), 'UPDATE_STATUS', 'InboundOrders', i.Id,
            N'Trạng thái cũ: ' + d.Status, N'Trạng thái mới: ' + i.Status, GETDATE()
        FROM inserted i INNER JOIN deleted d ON i.Id = d.Id;
    END
END;
GO

-- 7.3. Trigger Audit Lịch sử Phiếu Xuất
CREATE TRIGGER dbo.trg_AuditOutboundOrders
ON OutboundOrders
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF UPDATE(Status)
    BEGIN
        INSERT INTO AuditLogs (UserId, Action, TableName, RecordId, OldData, NewData, Timestamp)
        SELECT 
            ISNULL(i.ProcessedBy, i.CreatedBy), 'UPDATE_STATUS', 'OutboundOrders', i.Id,
            N'Trạng thái cũ: ' + d.Status, N'Trạng thái mới: ' + i.Status, GETDATE()
        FROM inserted i INNER JOIN deleted d ON i.Id = d.Id;
    END
END;
GO

-- 7.4. Trigger Audit Lịch sử Chuyển Kho
CREATE TRIGGER dbo.trg_AuditTransferOrders
ON TransferOrders
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF UPDATE(Status)
    BEGIN
        INSERT INTO AuditLogs (UserId, Action, TableName, RecordId, OldData, NewData, Timestamp)
        SELECT 
            ISNULL(i.ProcessedBy, i.CreatedBy), 'UPDATE_STATUS', 'TransferOrders', i.Id,
            N'Trạng thái cũ: ' + d.Status, N'Trạng thái mới: ' + i.Status, GETDATE()
        FROM inserted i INNER JOIN deleted d ON i.Id = d.Id;
    END
END;
GO

-- =================================================================
-- PHẦN 8. TỐI ƯU HIỆU NĂNG TÌM KIẾM (INDEXES)
-- =================================================================
-- Tăng tốc độ tìm kiếm khi quét Barcode hoặc gõ mã SKU
CREATE NONCLUSTERED INDEX IX_Products_SKU ON Products(SKU);
CREATE NONCLUSTERED INDEX IX_Products_Barcode ON Products(Barcode);

-- Tăng tốc độ lọc danh sách đơn hàng trên giao diện quản trị theo trạng thái
CREATE NONCLUSTERED INDEX IX_InboundOrders_Status ON InboundOrders(Status);
CREATE NONCLUSTERED INDEX IX_OutboundOrders_Status ON OutboundOrders(Status);
CREATE NONCLUSTERED INDEX IX_TransferOrders_Status ON TransferOrders(Status);
GO

-- =================================================================
-- PHẦN 9. SEED DATA (NẠP DỮ LIỆU MẪU ĐỂ CHẠY THỬ HỆ THỐNG)
-- =================================================================

-- 9.1. Dữ liệu Tài khoản (Mật khẩu là: 123456 đã băm SHA256)
INSERT INTO Users (Username, PasswordHash, FullName, Email, Role, IsActive)
VALUES 
('admin', '8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', N'Trần Giám Đốc', 'admin@wms.com', N'quản lý', 1),
('nhanvien1', '8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92', N'Nguyễn Thủ Kho', 'thukho@wms.com', N'nhân viên', 1);
GO

-- 9.2. Dữ liệu Kho & Sản phẩm
INSERT INTO Warehouses (Name, Address, IsActive)
VALUES 
(N'Kho Tổng Miền Bắc (Hà Nội)', N'Cụm CN Từ Liêm, Hà Nội', 1),
(N'Kho Trung Chuyển (Đà Nẵng)', N'KCN Hòa Khánh, Đà Nẵng', 1),
(N'Kho Miền Nam (TP.HCM)', N'Khu Công Nghệ Cao, Q9', 1);
GO

INSERT INTO Products (SKU, Barcode, Name, Category, IsActive)
VALUES 
('IP15-256', '893001', N'iPhone 15 Pro Max 256GB Titan', N'Điện thoại', 1),
('SS-S24U', '893002', N'Samsung Galaxy S24 Ultra 512GB', N'Điện thoại', 1),
('MAC-M3', '893003', N'MacBook Pro 14 M3 2023', N'Máy tính', 1),
('AP-PRO2', '893004', N'AirPods Pro Gen 2 Type-C', N'Phụ kiện', 1);
GO

-- 9.3. Dữ liệu Đối tác (Khách hàng & Nhà cung cấp)
INSERT INTO Suppliers (Name, Phone, Email, Address, IsActive)
VALUES 
(N'Công Ty TNHH Apple Việt Nam Logistics', '02838241234', 'supply@apple.com.vn', N'Tòa nhà trung tâm Melinh Point, Quận 1, TP.HCM', 1),
(N'Tổng Kho Linh Kiện Samsung Electronics', '02435559876', 'contact@samsunglogistics.com', N'Khu công nghiệp Yên Phong, Tỉnh Bắc Ninh', 1),
(N'Nhà Phân Phối Thiết Bị Số Digiwold', '19001234', 'info@digiwold.com.vn', N'Đường Cộng Hòa, Quận Tân Bình, TP.HCM', 1);
GO

INSERT INTO Customers (Name, Phone, Email, Address, IsActive)
VALUES 
(N'Hệ Thống Bán Lẻ Thế Giới Di Động', '18001060', 'contact@thegioididong.com', N'Khu công nghệ cao, Quận 9, TP.HCM', 1),
(N'Công Ty Cổ Phần Bán Lẻ Kỹ Thuật Số FPT Shop', '18006601', 'fptshop@fpt.com.vn', N'Đường Trần Hưng Đạo, Quận 5, TP.HCM', 1),
(N'Chuỗi Cửa Hàng Hoàng Hà Mobile', '19002091', 'online@hoanghamobile.com', N'Đường Lê Duẩn, Quận Đống Đa, Hà Nội', 1);
GO

-- 9.4. Nạp Dữ liệu Tồn kho mẫu để Kích hoạt Dashboard View (Đủ 4 tình huống Alert)
INSERT INTO Inventory (WarehouseId, ProductId, Quantity, MinQuantity, MaxQuantity, UpdatedAt)
VALUES 
-- 🟢 Trạng thái: An toàn (Nằm trong khoảng 10 - 100)
(1, 1, 50, 10, 100, GETDATE()), 

-- 🟡 Trạng thái: Dưới hạn mức (Tồn 5, nhưng tối thiểu phải là 10)
(1, 2, 5, 10, 100, GETDATE()),  

-- 🔴 Trạng thái: Hết hàng (Tồn = 0)
(2, 3, 0, 5, 50, GETDATE()),    

-- 🔵 Trạng thái: Vượt hạn mức (Tồn 150, nhưng kho chỉ quy định chứa tối đa 100)
(3, 4, 150, 20, 100, GETDATE());
GO