-----CREATE AND USE DATABASE-----
CREATE DATABASE AMIProject;

GO

USE AMIProject;

GO

-----CREATE TABLES-----

CREATE TABLE Users (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    Password NVARCHAR(255) NOT NULL,
    DisplayName NVARCHAR(100),
    Email NVARCHAR(100),
    Phone NVARCHAR(20),
    LastLogin DATETIME NULL,
    Status NVARCHAR(20) DEFAULT 'Active' 
        CHECK (Status IN ('Active', 'Inactive'))
);

GO

CREATE TABLE OrgUnit (
    OrgUnitID INT IDENTITY(1,1) PRIMARY KEY,
    Zone NVARCHAR(100) NULL,
    Substation NVARCHAR(100) NULL,
    Feeder NVARCHAR(100) NULL,
    DTR NVARCHAR(100) NULL
);

GO

CREATE TABLE Tariff (
    TariffID INT IDENTITY(1,1) PRIMARY KEY,
    TariffName NVARCHAR(100) NOT NULL CHECK (TariffName IN ('Residential Tariff', 'Commercial Tariff', 'Factory Tariff')),
    EffectiveFrom DATE NOT NULL,
    EffectiveTo DATE NULL,
    BaseRate DECIMAL(10,2) NOT NULL,
    TaxRate DECIMAL(10,2) NOT NULL
);

GO

CREATE TABLE TariffSlab ( 
	SlabId INT IDENTITY(1,1) PRIMARY KEY, 
	TariffID INT NOT NULL REFERENCES Tariff(TariffID), 
	FromKwh DECIMAL(18,6) NOT NULL, 
	ToKwh DECIMAL(18,6) NOT NULL, 
	RatePerKwh DECIMAL(18,6) NOT NULL, 
	CONSTRAINT CK_TariffSlab_Range CHECK (FromKwh >= 0 AND ToKwh > FromKwh) 
); 

GO

CREATE TABLE Consumer (
    ConsumerID INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Address NVARCHAR(255),
    Phone NVARCHAR(20),
    Email NVARCHAR(100),
    Status VARCHAR(20) NOT NULL DEFAULT 'Active' CHECK (Status IN ('Active','Inactive')), 
    CreatedAt DATETIME2(3) NOT NULL DEFAULT SYSUTCDATETIME(), 
	CreatedBy NVARCHAR(100) NOT NULL DEFAULT 'admin', 
	UpdatedAt DATETIME2(3) NULL, 
	UpdatedBy NVARCHAR(100) NULL
);

GO

CREATE TABLE Meter (
    MeterSerialNo NVARCHAR(50) PRIMARY KEY,
    ConsumerID INT NOT NULL REFERENCES Consumer(ConsumerID),
    IPAddress NVARCHAR(50) NOT NULL,
    ICCID NVARCHAR(50) NOT NULL,
    IMSI NVARCHAR(50) NOT NULL,
    Manufacturer NVARCHAR(100) NOT NULL,
    Firmware NVARCHAR(20) NULL,
    Category NVARCHAR(50) NOT NULL CHECK (Category IN ('Residential Tariff', 'Commercial Tariff', 'Factory Tariff')),
	OrgUnitId INT NOT NULL REFERENCES OrgUnit(OrgUnitID),
    InstallDate DATETIME2(3) NOT NULL,
    Status VARCHAR(20) NOT NULL DEFAULT 'Active' 
           CHECK (Status IN ('Active','Inactive','Decommissioned')), 
);

GO

CREATE TABLE DailyConsumption (
    MeterID NVARCHAR(50) NOT NULL REFERENCES Meter(MeterSerialNo),
    ConsumptionDate DATE NOT NULL,
	ConsumptionkWh DECIMAL(10,2) NOT NULL
);

GO
CREATE TABLE MonthlyConsumption (
    MeterID NVARCHAR(50) NOT NULL REFERENCES Meter(MeterSerialNo),
    MonthStartDate DATE NOT NULL,
    ConsumptionkWh DECIMAL(10,2) NOT NULL,
    CONSTRAINT PK_MonthlyConsumption PRIMARY KEY (MeterID, MonthStartDate)
);

GO

CREATE TABLE Bill (
    BillID INT IDENTITY(1,1) PRIMARY KEY,
    MeterID NVARCHAR(50) NOT NULL REFERENCES Meter(MeterSerialNo),
    MonthStartDate DATE NOT NULL,
    MonthlyConsumptionkWh DECIMAL(10,2) NOT NULL,
    Category NVARCHAR(50) NOT NULL CHECK (Category IN ('Residential Tariff', 'Commercial Tariff', 'Factory Tariff')),
    BaseRate DECIMAL(10,2) NOT NULL,
    SlabRate DECIMAL(10,2) NOT NULL,
    TaxRate DECIMAL(10,2) NOT NULL,
    Amount DECIMAL(18,2) NOT NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT 'Pending' 
           CHECK (Status IN ('Paid','Pending')),
    GeneratedAt DATETIME2(3) NOT NULL DEFAULT SYSUTCDATETIME()
);

GO

CREATE TABLE ConsumerLogin (
    ConsumerLoginID INT IDENTITY(1,1) PRIMARY KEY,
    ConsumerID INT NOT NULL UNIQUE REFERENCES Consumer(ConsumerID) ON DELETE CASCADE,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    Password NVARCHAR(255) NOT NULL,
    LastLogin DATETIME2(3) NULL,
    IsVerified BIT DEFAULT 0,
    Status NVARCHAR(20) DEFAULT 'Active' 
        CHECK (Status IN ('Active', 'Inactive'))
);

GO

-----CREATE TRIGGERS-----

CREATE OR ALTER TRIGGER trg_AutoGenerateBill
ON MonthlyConsumption
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Meter NVARCHAR(50);
    DECLARE cur CURSOR LOCAL FAST_FORWARD FOR
        SELECT DISTINCT MeterID FROM inserted WHERE MeterID IS NOT NULL;
    OPEN cur;
    FETCH NEXT FROM cur INTO @Meter;
    WHILE @@FETCH_STATUS = 0
    BEGIN
	BEGIN TRY
        EXEC sp_GenerateMonthlyBills @MeterID = @Meter;
		END TRY
		BEGIN CATCH
		END CATCH;
        FETCH NEXT FROM cur INTO @Meter;
    END
    CLOSE cur;
    DEALLOCATE cur;
END;

GO

CREATE OR ALTER TRIGGER trg_Tariff_Changed
ON Tariff
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        DECLARE @AffectedTariffIDs TABLE (TariffID INT PRIMARY KEY);

        INSERT INTO @AffectedTariffIDs
        SELECT DISTINCT TariffID FROM inserted
        UNION
        SELECT DISTINCT TariffID FROM deleted;

        IF NOT EXISTS (SELECT 1 FROM @AffectedTariffIDs) RETURN;

        DECLARE @AffectedNames TABLE (TariffName NVARCHAR(200) PRIMARY KEY);

        INSERT INTO @AffectedNames
        SELECT DISTINCT TariffName
        FROM Tariff t
        INNER JOIN @AffectedTariffIDs x ON x.TariffID = t.TariffID;

        DECLARE @Meters TABLE (MeterID NVARCHAR(50) PRIMARY KEY);

        INSERT INTO @Meters
        SELECT DISTINCT MeterSerialNo
        FROM Meter m
        INNER JOIN @AffectedNames n ON m.Category = n.TariffName;

        DECLARE @m NVARCHAR(50);
        DECLARE cur CURSOR LOCAL FAST_FORWARD FOR SELECT MeterID FROM @Meters;

        OPEN cur;
        FETCH NEXT FROM cur INTO @m;

        WHILE @@FETCH_STATUS = 0
        BEGIN
            BEGIN TRY
                EXEC sp_GenerateMonthlyBills @MeterID = @m;
            END TRY BEGIN CATCH END CATCH;

            FETCH NEXT FROM cur INTO @m;
        END

        CLOSE cur;
        DEALLOCATE cur;
    END TRY
    BEGIN CATCH
    END CATCH
END;

GO

CREATE OR ALTER TRIGGER trg_TariffSlab_Changed
ON TariffSlab
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        DECLARE @AffectedTariffIDs TABLE (TariffID INT PRIMARY KEY);

        INSERT INTO @AffectedTariffIDs
        SELECT DISTINCT TariffID FROM inserted
        UNION
        SELECT DISTINCT TariffID FROM deleted;

        IF NOT EXISTS (SELECT 1 FROM @AffectedTariffIDs) RETURN;

        DECLARE @AffectedNames TABLE (TariffName NVARCHAR(200) PRIMARY KEY);

        INSERT INTO @AffectedNames
        SELECT DISTINCT TariffName
        FROM Tariff t
        INNER JOIN @AffectedTariffIDs x ON x.TariffID = t.TariffID;

        DECLARE @Meters TABLE (MeterID NVARCHAR(50) PRIMARY KEY);

        INSERT INTO @Meters
        SELECT DISTINCT MeterSerialNo
        FROM Meter m
        INNER JOIN @AffectedNames n ON m.Category = n.TariffName;

        DECLARE @m NVARCHAR(50);
        DECLARE cur CURSOR LOCAL FAST_FORWARD FOR SELECT MeterID FROM @Meters;

        OPEN cur;
        FETCH NEXT FROM cur INTO @m;

        WHILE @@FETCH_STATUS = 0
        BEGIN
            BEGIN TRY
                EXEC sp_GenerateMonthlyBills @MeterID = @m;
            END TRY BEGIN CATCH END CATCH;

            FETCH NEXT FROM cur INTO @m;
        END

        CLOSE cur;
        DEALLOCATE cur;
    END TRY
    BEGIN CATCH
    END CATCH
END;

GO

CREATE OR ALTER TRIGGER trg_UpdateMonthlyConsumption
ON DailyConsumption
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @ChangedMeters TABLE (MeterID NVARCHAR(50), ConsumptionMonth DATE);
    INSERT INTO @ChangedMeters (MeterID, ConsumptionMonth)
    SELECT DISTINCT MeterID,
           DATEFROMPARTS(YEAR(ConsumptionDate), MONTH(ConsumptionDate), 1)
    FROM inserted
    UNION
    SELECT DISTINCT MeterID,
           DATEFROMPARTS(YEAR(ConsumptionDate), MONTH(ConsumptionDate), 1)
    FROM deleted;
    MERGE MonthlyConsumption AS target
    USING (
        SELECT 
            MeterID,
            DATEFROMPARTS(YEAR(ConsumptionDate), MONTH(ConsumptionDate), 1) AS MonthStartDate,
            SUM(ConsumptionkWh) AS TotalConsumption
        FROM DailyConsumption
        WHERE EXISTS (
            SELECT 1 FROM @ChangedMeters c
            WHERE c.MeterID = DailyConsumption.MeterID
              AND DATEFROMPARTS(YEAR(DailyConsumption.ConsumptionDate), MONTH(DailyConsumption.ConsumptionDate), 1) = c.ConsumptionMonth
        )
        GROUP BY MeterID, DATEFROMPARTS(YEAR(ConsumptionDate), MONTH(ConsumptionDate), 1)
    ) AS src
    ON target.MeterID = src.MeterID AND target.MonthStartDate = src.MonthStartDate
    WHEN MATCHED THEN
        UPDATE SET target.ConsumptionkWh = src.TotalConsumption
    WHEN NOT MATCHED THEN
        INSERT (MeterID, MonthStartDate, ConsumptionkWh)
        VALUES (src.MeterID, src.MonthStartDate, src.TotalConsumption);
    DELETE mc
    FROM MonthlyConsumption mc
    WHERE NOT EXISTS (
        SELECT 1 
        FROM DailyConsumption d
        WHERE d.MeterID = mc.MeterID
          AND DATEFROMPARTS(YEAR(d.ConsumptionDate), MONTH(d.ConsumptionDate), 1) = mc.MonthStartDate
    );
END;

GO

CREATE OR ALTER TRIGGER trg_Update_ConsumerTimestamp
ON Consumer
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE c
    SET 
        UpdatedAt = SYSUTCDATETIME()
    FROM Consumer c
    INNER JOIN inserted i ON c.ConsumerID = i.ConsumerID;
END;

GO
-----CREATE PROCEDURES-----

CREATE OR ALTER PROCEDURE sp_GenerateMonthlyBills
    @MeterID NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Target TABLE (MeterID NVARCHAR(50), MonthStartDate DATE);
    IF @MeterID IS NOT NULL
    BEGIN
        INSERT INTO @Target (MeterID, MonthStartDate)
        SELECT DISTINCT MeterID, MonthStartDate
        FROM MonthlyConsumption
        WHERE MeterID = @MeterID;
    END
    ELSE
    BEGIN
        INSERT INTO @Target (MeterID, MonthStartDate)
        SELECT MeterID, MonthStartDate FROM MonthlyConsumption;
    END
    
    ;WITH BillData AS (
        SELECT
            mc.MeterID,
            mc.MonthStartDate,
            mc.ConsumptionkWh,
            m.Category,
            t.BaseRate,
            ts.RatePerKwh AS SlabRate,
            t.TaxRate,
            CAST(
                mc.ConsumptionkWh * (1 + t.BaseRate) * (1 + ts.RatePerKwh) * (1 + t.TaxRate)
                AS DECIMAL(18,2)
            ) AS Amount
        FROM MonthlyConsumption mc
        INNER JOIN Meter m ON mc.MeterID = m.MeterSerialNo
        INNER JOIN Tariff t ON m.Category = t.TariffName 
        INNER JOIN TariffSlab ts 
            ON ts.TariffID = t.TariffID 
            AND mc.ConsumptionkWh BETWEEN ts.FromKwh AND ts.ToKwh
        WHERE EXISTS (
            SELECT 1 FROM @Target t2
            WHERE t2.MeterID = mc.MeterID AND t2.MonthStartDate = mc.MonthStartDate
        )
    )
    MERGE Bill AS target
    USING BillData AS src
        ON target.MeterID = src.MeterID AND target.MonthStartDate = src.MonthStartDate
    WHEN MATCHED THEN
        UPDATE SET 
            target.MonthlyConsumptionkWh = src.ConsumptionkWh,
            target.BaseRate = src.BaseRate,
            target.SlabRate = src.SlabRate,
            target.TaxRate = src.TaxRate,
            target.Category = src.Category,
            target.Amount = src.Amount,
            target.Status = 'Pending',
            target.GeneratedAt = SYSUTCDATETIME()
    WHEN NOT MATCHED THEN
        INSERT (MeterID, MonthStartDate, MonthlyConsumptionkWh, Category, BaseRate, SlabRate, TaxRate, Amount, Status, GeneratedAt)
        VALUES (src.MeterID, src.MonthStartDate, src.ConsumptionkWh, src.Category, src.BaseRate, src.SlabRate, src.TaxRate, src.Amount, 'Pending', SYSUTCDATETIME());
END;

GO

-----INSERT DATA INTO TABLES-----

INSERT INTO Users (Username, Password, DisplayName, Email, Phone, LastLogin, Status)
VALUES
('admin', 'admin123', 'Administrator', 'admin@ami.com', '9999999999', '2025-10-30 09:00:00', 'Active'),
('shruti', 'shruti123', 'Shruti', 'shruti@example.com', '9876543210', NULL, 'Active'),
('nami', 'nami123', 'Namitha', 'nami@example.com', '9123456780', NULL, 'Inactive');

GO

INSERT INTO OrgUnit (Zone, Substation, Feeder, DTR)
VALUES
('North', 'Haryana', 'Hisar', 'DTR-1'),
('South', 'Karnataka', 'Bangalore', 'DTR-2'),
('North', 'Rajasthan', 'Jaipur', 'DTR-5'),
('South', 'Karnataka', 'Bangalore', 'DTR-4'),
('North', 'Rajasthan', 'Jaipur', 'DTR-3'),
('South', 'Karnataka', 'Bajpe', 'DTR-6');

GO

INSERT INTO Tariff (TariffName, EffectiveFrom, EffectiveTo, BaseRate, TaxRate)
VALUES
('Residential Tariff', '2024-01-01', NULL, 5.50, 0.18),
('Commercial Tariff',  '2024-01-01', NULL, 7.25, 0.18),
('Factory Tariff',     '2024-01-01', NULL, 6.00, 0.18);

GO

INSERT INTO TariffSlab (TariffID, FromKwh, ToKwh, RatePerKwh)
VALUES
(1, 0, 100, 4.00),
(1, 101, 300, 5.50),
(1, 301, 999999, 6.50),
(2, 0, 500, 7.00),
(2, 501, 999999, 8.00),
(3, 0, 1000, 5.00),
(3, 1001, 999999, 5.50);

GO

INSERT INTO Consumer (Name, Address, Phone, Email, TariffID, Status, CreatedBy)
VALUES
('Ravi Kumar', '123 North Street, City', '9000011111', 'ravi@example.com', 1, 'Active', 'admin'),
('Priya Sharma', '456 South Avenue, City', '9000022222', 'priya@example.com', 1, 'Active', 'admin'),
('TechCorp Pvt Ltd', '789 Industrial Area, City', '9000033333', 'corp@example.com', 2, 'Active', 'admin'),
('MegaFactory Ltd', 'Industrial Zone B, City', '9000044444', 'factory@example.com', 3, 'Active', 'admin');

GO

INSERT INTO Meter (MeterSerialNo, ConsumerID, IPAddress, ICCID, IMSI, Manufacturer, Firmware, Category, OrgUnitId, InstallDate, Status)
VALUES
('MTR1001', 1, '192.168.1.101', 'ICCID001', 'IMSI001', 'Siemens', '1.0.0', 'Residential Tariff', 1, '2025-09-22', 'Active'),
('MTR1002', 2, '192.168.1.102', 'ICCID002', 'IMSI002', 'Schneider', '1.1.0', 'Residential Tariff', 2, '2025-09-23', 'Active'),
('MTR2001', 3, '192.168.1.201', 'ICCID201', 'IMSI201', 'ABB', '2.0.0', 'Commercial Tariff', 4, '2025-09-25', 'Active'),
('MTR3001', 4, '192.168.1.301', 'ICCID301', 'IMSI301', 'Larsen', '3.0.0', 'Factory Tariff', 3, '2025-09-26', 'Active');

GO

INSERT INTO DailyConsumption (MeterID, ConsumptionDate, ConsumptionkWh)
VALUES
('MTR1001', '2025-11-01', 6.5),
('MTR1001', '2025-11-02', 5.8),
('MTR1001', '2025-11-03', 6.2),
('MTR1001', '2025-11-04', 5.4),
('MTR1002', '2025-11-01', 4.5),
('MTR1002', '2025-11-02', 4.8),
('MTR1002', '2025-11-03', 5.1),
('MTR2001', '2025-11-01', 15.2),
('MTR2001', '2025-11-02', 14.9),
('MTR2001', '2025-11-03', 16.0),
('MTR3001', '2025-11-01', 45.5),
('MTR3001', '2025-11-02', 46.3),
('MTR3001', '2025-11-03', 47.1),
('MTR3001', '2025-10-30', 32),
('MTR3001', '2025-11-20', 20.5),
('MTR2001', '2025-11-15', 12.5),
('MTR2001', '2025-10-16', 9.5);

GO

INSERT INTO ConsumerLogin (ConsumerID, Username, Password, IsVerified)
VALUES
(1, 'ravi_kumar', 'ravi@123', 1),
(2, 'priya_sharma', 'priya@123', 1),
(3, 'techcorp', 'corp@123', 0),
(4, 'megafactory', 'factory@123', 0);

GO

-----VIEW TABLES-----

SELECT * FROM Meter
SELECT * FROM Users
SELECT * FROM OrgUnit
SELECT * FROM Tariff 
SELECT * FROM TariffSlab
SELECT * FROM Consumer
SELECT * FROM DailyConsumption
SELECT * FROM MonthlyConsumption
SELECT * FROM Bill
SELECT * FROM ConsumerLogin

GO

-----DROP TABLES IF NEEDED-----

DROP TABLE Meter
DROP TABLE Users
DROP TABLE OrgUnit
DROP TABLE Tariff 
DROP TABLE TariffSlab
DROP TABLE Consumer
DROP TABLE DailyConsumption
DROP TABLE MonthlyConsumption
DROP TABLE Bill
DROP TABLE ConsumerLogin
