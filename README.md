# AutoRental - Hệ thống thuê xe tự động

## Mô tả

AutoRental là một ứng dụng WPF được phát triển bằng C# và .NET 8, sử dụng Entity Framework Core để quản lý cơ sở dữ liệu SQL Server. Hệ thống cung cấp các chức năng cơ bản cho việc quản lý thuê xe.

## Tính năng đã phát triển

### 1. Xác thực người dùng

- **Đăng nhập**: Xác thực người dùng với username và password
- **Đăng ký**: Tạo tài khoản mới với validation đầy đủ
- **Đăng xuất**: Thoát khỏi hệ thống an toàn

### 2. Giao diện người dùng

- Giao diện đăng nhập/đăng ký hiện đại và thân thiện
- Dashboard chính với menu navigation
- Responsive design với Material Design

### 3. Bảo mật

- Mã hóa mật khẩu bằng SHA256
- Validation dữ liệu đầu vào
- Kiểm tra trùng lặp username/email

## Cấu trúc dự án

```
AutoRental/
├── AutoRental/                 # WPF Application
│   ├── LoginWindow.xaml       # Giao diện đăng nhập
│   ├── RegisterWindow.xaml    # Giao diện đăng ký
│   ├── MainWindow.xaml        # Giao diện chính
│   └── App.xaml              # Cấu hình ứng dụng
├── BusinessObject/            # Entity Models
│   ├── User.cs               # Model User
│   ├── Role.cs               # Model Role
│   └── AutoRentalPrnContext.cs # DbContext
├── Service/                   # Business Logic
│   ├── Interfaces/           # Service Interfaces
│   └── Implementations/      # Service Implementations
└── sample_data.sql           # Dữ liệu mẫu
```

## Cài đặt và chạy

### 1. Yêu cầu hệ thống

- .NET 8.0 SDK
- SQL Server (Express hoặc cao hơn)
- Visual Studio 2022 hoặc VS Code

### 2. Cài đặt database

1. Tạo database `AutoRental` trong SQL Server
2. Chạy script tạo bảng từ file `database_schema.sql`
3. Chạy script thêm dữ liệu mẫu từ file `sample_data.sql`

### 3. Cấu hình connection string

Cập nhật connection string trong file `BusinessObject/AutoRentalPrnContext.cs`:

```csharp
optionsBuilder.UseSqlServer("Server=YOUR_SERVER;Database=AutoRental;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=true;");
```

### 4. Build và chạy

1. Mở solution trong Visual Studio
2. Restore NuGet packages
3. Build solution
4. Chạy ứng dụng

## Tài khoản test

Sau khi chạy script `sample_data.sql`, bạn có thể sử dụng các tài khoản sau:

| Username | Password | Role  |
| -------- | -------- | ----- |
| admin    | 123456   | Admin |
| user1    | 123456   | User  |
| user2    | 123456   | User  |

## Tính năng đang phát triển

- Quản lý xe (CRUD operations)
- Quản lý đặt xe
- Quản lý người dùng
- Báo cáo và thống kê
- Quản lý thanh toán
- Upload hình ảnh xe

## Công nghệ sử dụng

- **Frontend**: WPF (Windows Presentation Foundation)
- **Backend**: C# .NET 8
- **Database**: SQL Server
- **ORM**: Entity Framework Core
- **Architecture**: 3-layer architecture (Presentation, Business, Data)

## Đóng góp

1. Fork dự án
2. Tạo feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to branch (`git push origin feature/AmazingFeature`)
5. Tạo Pull Request

## License

Dự án này được phát triển cho mục đích học tập và nghiên cứu.
