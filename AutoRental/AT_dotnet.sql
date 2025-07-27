CREATE DATABASE AutoRental_dotnet;
GO

USE AutoRental_dotnet;
GO

CREATE TABLE Roles (
    RoleId INT NOT NULL IDENTITY(1,1),
    RoleName NVARCHAR(50) NOT NULL,
    CONSTRAINT PK_Roles PRIMARY KEY (RoleId)
);
GO

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

CREATE TABLE CarBrand (
    BrandId INT NOT NULL IDENTITY(1,1),
    BrandName NVARCHAR(100) NOT NULL UNIQUE,
    CONSTRAINT PK_CarBrand PRIMARY KEY (BrandId)
);
GO

-- Car: Thông tin xe
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

-- CarImages: Hình ảnh xe
CREATE TABLE CarImages (
    ImageId INT NOT NULL IDENTITY(1,1),
    CarId INT NOT NULL,
    ImageUrl NVARCHAR(255) NOT NULL,
    IsMain BIT NOT NULL DEFAULT 0,
    CONSTRAINT PK_CarImages PRIMARY KEY (ImageId),
    CONSTRAINT FK_CarImages_CarId FOREIGN KEY (CarId) REFERENCES Car(CarId) ON DELETE CASCADE
);
GO

-- Discount: Bảng giảm giá
CREATE TABLE Discount (
    DiscountId INT NOT NULL IDENTITY(1,1),
    DiscountName NVARCHAR(100) NOT NULL,
    DiscountValue DECIMAL(5,2) NOT NULL CHECK (DiscountValue >= 0 AND DiscountValue <= 100),
    CONSTRAINT PK_Discount PRIMARY KEY (DiscountId)
);
GO

-- Booking: Đặt xe
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

CREATE INDEX IX_Car_LicensePlate ON Car(LicensePlate);
CREATE INDEX IX_Users_Email ON Users(Email);
CREATE INDEX IX_Booking_BookingCode ON Booking(BookingCode);
CREATE INDEX IX_Booking_DiscountId ON Booking(DiscountId);
GO

SET IDENTITY_INSERT Roles ON;

INSERT INTO Roles (RoleId, RoleName) VALUES (1, N'User');
INSERT INTO Roles (RoleId, RoleName) VALUES (2, N'Admin');

INSERT INTO Users (
    Username,
    FullName,
    Email,
    Password,
    PhoneNumber,
    RoleId
)
VALUES (
    N'admin',
    N'Administrator',
    N'admin@autorental.com',
    N'admin123',        -- Mật khẩu nên hash nếu dùng thực tế
    N'0123456789',
    2                   -- RoleId của Admin
);

INSERT INTO Users (Username, FullName, Email, Password, PhoneNumber, RoleId)
VALUES 
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


SET IDENTITY_INSERT Roles OFF;