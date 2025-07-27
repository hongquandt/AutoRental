-- Create Database
CREATE DATABASE AutoRental_dotnet;
GO

USE AutoRental_dotnet;
GO

-- Create Roles Table
CREATE TABLE Roles (
    RoleId INT NOT NULL IDENTITY(1,1),
    RoleName NVARCHAR(50) NOT NULL,
    CONSTRAINT PK_Roles PRIMARY KEY (RoleId)
);
GO

-- Create Users Table
CREATE TABLE Users (
    UserId INT NOT NULL IDENTITY(1,1),
    Username NVARCHAR(100) NOT NULL UNIQUE,
    FullName NVARCHAR(256) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    Password NVARCHAR(255) NOT NULL,
    PhoneNumber NVARCHAR(10) NULL,
    RoleId INT NOT NULL,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT PK_Users PRIMARY KEY (UserId),
    CONSTRAINT FK_Users_Roles FOREIGN KEY (RoleId) REFERENCES Roles(RoleId) ON DELETE CASCADE
);
GO

-- Create CarBrand Table
CREATE TABLE CarBrand (
    BrandId INT NOT NULL IDENTITY(1,1),
    BrandName NVARCHAR(100) NOT NULL UNIQUE,
    CONSTRAINT PK_CarBrand PRIMARY KEY (BrandId)
);
GO

-- Create Car Table
CREATE TABLE Car (
    CarId INT NOT NULL IDENTITY(1,1),
    BrandId INT NOT NULL,
    CarModel NVARCHAR(50) NOT NULL,
    LicensePlate NVARCHAR(20) NOT NULL UNIQUE,
    Seats INT NOT NULL CHECK (Seats BETWEEN 1 AND 50),
    PricePerDay DECIMAL(10,2) NOT NULL CHECK (PricePerDay >= 0),
    Status VARCHAR(20) NOT NULL DEFAULT 'Available' CHECK (Status IN ('Available', 'Rented', 'Unavailable')),
    CreatedDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT PK_Car PRIMARY KEY (CarId),
    CONSTRAINT FK_Car_BrandId FOREIGN KEY (BrandId) REFERENCES CarBrand(BrandId)
);
GO

-- Create CarImages Table
CREATE TABLE CarImages (
    ImageId INT NOT NULL IDENTITY(1,1),
    CarId INT NOT NULL,
    ImageUrl NVARCHAR(255) NOT NULL,
    IsMain BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_CarImages PRIMARY KEY (ImageId),
    CONSTRAINT FK_CarImages_CarId FOREIGN KEY (CarId) REFERENCES Car(CarId) ON DELETE CASCADE
);
GO

-- Create Discount Table
CREATE TABLE Discount (
    DiscountId INT NOT NULL IDENTITY(1,1),
    DiscountName NVARCHAR(100) NOT NULL,
    DiscountValue DECIMAL(5,2) NOT NULL CHECK (DiscountValue >= 0 AND DiscountValue <= 100),
    CONSTRAINT PK_Discount PRIMARY KEY (DiscountId)
);
GO

-- Create Booking Table
CREATE TABLE Booking (
    BookingId INT NOT NULL IDENTITY(1,1),
    UserId INT NOT NULL,
    CarId INT NOT NULL,
    DiscountId INT NULL,
    PickupDateTime DATETIME2 NOT NULL,
    ReturnDateTime DATETIME2 NOT NULL,
    TotalAmount DECIMAL(10,2) NOT NULL CHECK (TotalAmount >= 0),
    Status NVARCHAR(20) NOT NULL DEFAULT 'Pending' CHECK (Status IN ('Pending', 'Confirmed', 'Cancelled', 'Completed')),
    CreatedDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    BookingCode NVARCHAR(20) NOT NULL UNIQUE,
    CONSTRAINT PK_Booking PRIMARY KEY (BookingId),
    CONSTRAINT FK_Booking_Users FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE,
    CONSTRAINT FK_Booking_Car FOREIGN KEY (CarId) REFERENCES Car(CarId) ON DELETE NO ACTION,
    CONSTRAINT FK_Booking_Discount FOREIGN KEY (DiscountId) REFERENCES Discount(DiscountId) ON DELETE SET NULL,
    CONSTRAINT CHK_Booking_Dates CHECK (ReturnDateTime > PickupDateTime)
);
GO

-- Create Payment Table
CREATE TABLE Payment (
    PaymentId INT NOT NULL IDENTITY(1,1),
    BookingId INT NOT NULL,
    Amount DECIMAL(10,2) NOT NULL CHECK (Amount >= 0),
    PaymentMethod NVARCHAR(50) NOT NULL,
    PaymentStatus NVARCHAR(20) NOT NULL DEFAULT 'Pending' CHECK (PaymentStatus IN ('Pending', 'Completed', 'Failed')),
    PaymentDate DATETIME2 NULL,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT PK_Payment PRIMARY KEY (PaymentId),
    CONSTRAINT FK_Payment_Booking FOREIGN KEY (BookingId) REFERENCES Booking(BookingId) ON DELETE CASCADE
);
GO

-- Create UserFeedback Table
CREATE TABLE UserFeedback (
    FeedbackId INT NOT NULL IDENTITY(1,1),
    UserId INT NOT NULL,
    CarId INT NULL,
    Rating INT NOT NULL CHECK (Rating >= 1 AND Rating <= 5),
    Content NVARCHAR(500) NULL,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT PK_UserFeedback PRIMARY KEY (FeedbackId),
    CONSTRAINT FK_UserFeedback_UserId FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE,
    CONSTRAINT FK_UserFeedback_CarId FOREIGN KEY (CarId) REFERENCES Car(CarId) ON DELETE SET NULL
);
GO

-- Create Indexes
CREATE INDEX IX_Car_LicensePlate ON Car(LicensePlate);
CREATE INDEX IX_Users_Email ON Users(Email);
CREATE INDEX IX_Booking_BookingCode ON Booking(BookingCode);
CREATE INDEX IX_Booking_DiscountId ON Booking(DiscountId);
GO

-- Insert into Roles
SET IDENTITY_INSERT Roles ON;
INSERT INTO Roles (RoleId, RoleName) VALUES 
(1, N'User'),
(2, N'Admin');
SET IDENTITY_INSERT Roles OFF;

-- Insert into Users
INSERT INTO Users (Username, FullName, Email, Password, PhoneNumber, RoleId) VALUES 
(N'admin', N'Administrator', N'admin@autorental.com', N'admin123', N'0123456789', 2),
(N'user1', N'Nguyễn Văn A', N'user1@example.com', N'123456', N'0901234561', 1),
(N'user2', N'Trần Thị B', N'user2@example.com', N'123456', N'0901234562', 1),
(N'user3', N'Lê Văn C', N'user3@example.com', N'123456', N'0901234563', 1),
(N'user4', N'Phạm Thị D', N'user4@example.com', N'123456', N'0901234564', 1),
(N'user5', N'Hoàng Văn E', N'user5@example.com', N'123456', N'0901234565', 1),
(N'user6', N'Đỗ Thị F', N'user6@example.com', N'123456', N'0901234566', 1),
(N'user7', N'Bùi Văn G', N'user7@example.com', N'123456', N'0901234567', 1),
(N'user8', N'Vũ Thị H', N'user8@example.com', N'123456', N'0901234568', 1),
(N'user9', N'Ngô Văn I', N'user9@example.com', N'123456', N'0901234569', 1),
(N'user10', N'Dương Thị J', N'user10@example.com', N'123456', N'0901234570', 1);
GO

-- Insert into CarBrand
INSERT INTO CarBrand (BrandName) VALUES
(N'Toyota'),
(N'Honda'),
(N'Hyundai'),
(N'Ford'),
(N'Kia'),
(N'Mazda'),
(N'Mitsubishi'),
(N'VinFast'),
(N'Chevrolet');
GO

-- Insert into Car (corrected to match table schema and use auto-incremented IDs)
INSERT INTO Car (BrandId, CarModel, LicensePlate, Seats, PricePerDay, Status, CreatedDate) VALUES
(1, N'Toyota Vios', '30A-12345', 5, 800.00, 'Available', GETDATE()),
(2, N'Honda City', '30A-23456', 5, 900.00, 'Available', GETDATE()),
(1, N'Toyota Innova', '30A-34567', 7, 1200.00, 'Available', GETDATE()),
(2, N'Honda CRV', '30A-45678', 7, 1500.00, 'Available', GETDATE()),
(1, N'Toyota Fortuner', '30A-56789', 7, 1000.00, 'Available', GETDATE()),
(3, N'Hyundai Accent', '30A-11223', 5, 1000.00, 'Available', GETDATE()),
(3, N'Hyundai SantaFe', '30A-22334', 7, 1800.00, 'Available', GETDATE()),
(4, N'Ford Ranger', '30A-33445', 5, 2000.00, 'Available', GETDATE()),
(4, N'Ford Everest', '30A-44556', 7, 1700.00, 'Available', GETDATE()),
(1, N'Toyota Camry', '30A-67890', 5, 1400.00, 'Available', GETDATE()),
(3, N'Hyundai Elantra', '30A-55667', 5, 1100.00, 'Available', GETDATE()),
(4, N'Ford EcoSport', '30A-66778', 5, 1200.00, 'Available', GETDATE()),
(5, N'Kia Morning', '30A-88881', 5, 700.00, 'Available', GETDATE()),
(5, N'Kia Seltos', '30A-88882', 5, 1200.00, 'Available', GETDATE()),
(5, N'Kia Sorento', '30A-88883', 7, 1500.00, 'Available', GETDATE()),
(6, N'Mazda 3', '30A-88884', 5, 900.00, 'Available', GETDATE()),
(6, N'Mazda CX-5', '30A-88885', 5, 1300.00, 'Available', GETDATE()),
(6, N'Mazda 6', '30A-88886', 5, 1100.00, 'Available', GETDATE()),
(7, N'Mitsubishi Xpander', '30A-88887', 7, 1000.00, 'Available', GETDATE()),
(7, N'Mitsubishi Outlander', '30A-88888', 5, 1200.00, 'Available', GETDATE()),
(7, N'Mitsubishi Attrage', '30A-88889', 5, 600.00, 'Available', GETDATE()),
(8, N'VinFast Fadil', '30A-88890', 5, 800.00, 'Available', GETDATE()),
(8, N'VinFast Lux A2.0', '30A-88891', 5, 1400.00, 'Available', GETDATE()),
(8, N'VinFast VF e34', '30A-88892', 5, 1600.00, 'Available', GETDATE()),
(9, N'Chevrolet Spark', '30A-88893', 5, 600.00, 'Available', GETDATE()),
(9, N'Chevrolet Colorado', '30A-88894', 5, 1300.00, 'Available', GETDATE()),
(9, N'Chevrolet Trailblazer', '30A-88895', 7, 1500.00, 'Available', GETDATE());
GO

-- Insert into CarImages (using auto-incremented CarId values)
INSERT INTO CarImages (CarId, ImageUrl, IsMain) VALUES
(1, '/assets/images/toyota_vios.jpg', 1),
(2, '/assets/images/honda_city.jpg', 1),
(3, '/assets/images/toyota_innova.jpg', 1),
(4, '/assets/images/honda_CRV.jpg', 1),
(5, '/assets/images/toyota_fortuner.jpg', 1),
(6, '/assets/images/hyundai_accent.jpg', 1),
(7, '/assets/images/hyundai_santaFe.jpg', 1),
(8, '/assets/images/ford_ranger.jpg', 1),
(9, '/assets/images/ford_everest.jpg', 1),
(10, '/assets/images/toyota_camry.jpg', 1),
(11, '/assets/images/hyundai_elantra.jpg', 1),
(12, '/assets/images/ford_ecosport.jpg', 1),
(13, '/assets/images/kia_morning.jpg', 1),
(14, '/assets/images/kia_seltos.jpg', 1),
(15, '/assets/images/kia_sorento.jpg', 1),
(16, '/assets/images/mazda3.jpg', 1),
(17, '/assets/images/mazda_cx5.jpg', 1),
(18, '/assets/images/mazda6.jpg', 1),
(19, '/assets/images/mitsubishi_xpander.jpg', 1),
(20, '/assets/images/mitsubishi_outlander.jpg', 1),
(21, '/assets/images/mitsubishi_attrage.jpg', 1),
(22, '/assets/images/vinfast_fadil.jpg', 1),
(23, '/assets/images/vinfast_luxa20.jpg', 1),
(24, '/assets/images/vinfast_vfe34.jpg', 1),
(25, '/assets/images/chevrolet_spark.jpg', 1),
(26, '/assets/images/chevrolet_colorado.jpg', 1),
(27, '/assets/images/chevrolet_trailblazer.jpg', 1);
GO
USE AutoRental_dotnet;
GO

UPDATE CarImages
SET ImageUrl = 
    CASE CarId
        WHEN 1 THEN '/images/toyota_vios.jpg'
        WHEN 2 THEN '/images/honda_city.jpg'
        WHEN 3 THEN '/images/toyota_innova.jpg'
        WHEN 4 THEN '/images/honda_CRV.jpg'
        WHEN 5 THEN '/images/toyota_fortuner.jpg'
        WHEN 6 THEN '/images/hyundai_accent.jpg'
        WHEN 7 THEN '/images/hyundai_santaFe.jpg'	
        WHEN 8 THEN '/images/ford_ranger.jpg'
        WHEN 9 THEN '/images/ford_everest.jpg'
        WHEN 10 THEN '/images/toyota_camry.jpg'
        WHEN 11 THEN '/images/hyundai_elantra.jpg'
        WHEN 12 THEN '/images/ford_ecosport.jpg'
        WHEN 13 THEN '/images/kia_morning.jpg'
        WHEN 14 THEN '/images/kia_seltos.jpg'
        WHEN 15 THEN '/images/kia_sorento.jpg'
        WHEN 16 THEN '/images/mazda3.jpg'
        WHEN 17 THEN '/images/mazda_cx5.jpg'
        WHEN 18 THEN '/images/mazda6.jpg'
        WHEN 19 THEN '/images/mitsubishi_xpander.jpg'
        WHEN 20 THEN '/images/mitsubishi_outlander.jpg'
        WHEN 21 THEN '/images/mitsubishi_attrage.jpg'
        WHEN 22 THEN '/images/vinfast_fadil.jpg'
        WHEN 23 THEN '/images/vinfast_luxa20.jpg'
        WHEN 24 THEN '/images/vinfast_vfe34.jpg'
        WHEN 25 THEN '/images/chevrolet_spark.jpg'
        WHEN 26 THEN '/images/chevrolet_colorado.jpg'
        WHEN 27 THEN '/images/chevrolet_trailblazer.jpg'
    END
WHERE CarId BETWEEN 1 AND 27;
GO