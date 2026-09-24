# QLThuGomRac_UNETI02_TI17A1HN

Ứng dụng Web quản lý thu gom rác tái chế và đăng ký lịch thu gom.

Project dùng đúng định dạng trong đề tài: `QLThuGomRac_UNETI02_TI17A1HN` (Nhóm 02, lớp TI17A1HN), xây dựng bằng **ASP.NET Core 10 MVC**, C# 14, Entity Framework Core 10 và SQL Server.

## Công nghệ

- .NET 10 / ASP.NET Core MVC
- C# 14
- Entity Framework Core 10 (`Code First` + `Migration`)
- SQL Server
- Razor View, Model Binding, Data Annotation, LINQ
- Session, đăng nhập và phân quyền
- HTML/CSS, JavaScript và Bootstrap

## Cấu trúc thư mục

```text
.
├── Controllers/                         # Controller theo từng module
│   ├── Account/                         # Đăng nhập, đăng xuất, phân quyền
│   ├── Admin/                            # Module 1, 2, 3 và 4 phía Admin
│   ├── NhanVien/                         # Module 3 phía Nhân viên
│   └── NguoiDan/                         # Module 2 phía Người dân
├── Models/
│   ├── Entities/                         # Entity và quan hệ CSDL
│   ├── Enums/                            # Loại tài khoản, trạng thái nghiệp vụ
│   └── ViewModels/                       # ViewModel cho từng chức năng
├── Data/
│   ├── Migrations/                       # EF Core Code First migrations
│   ├── Seed/                             # Dữ liệu mẫu
│   └── ApplicationDbContext.cs            # DbContext của SQL Server
├── Services/                             # Nơi tổ chức logic dùng chung khi cần
├── Views/
│   ├── Account/                          # Giao diện đăng nhập
│   ├── Admin/                            # Các màn hình quản trị
│   ├── NhanVien/                         # Các màn hình công việc
│   ├── NguoiDan/                         # Các màn hình người dân
│   ├── Home/                             # Trang chủ
│   └── Shared/                           # Layout, validation, error
├── wwwroot/                              # CSS, JavaScript và hình ảnh
├── Database/                             # Script/schema và dữ liệu mẫu
├── Docs/                                 # Tài liệu thiết kế và phân công
├── Tests/                                # Kiểm thử khi bổ sung test case
├── Properties/                           # Cấu hình chạy ứng dụng
├── appsettings.json                      # Chuỗi kết nối và cấu hình chung
├── Program.cs                            # Khởi tạo ASP.NET Core MVC
└── QLThuGomRac_UNETI02_TI17A1HN.sln       # Solution của nhóm
```

## Bốn module theo đề tài

1. **Tài khoản, đăng nhập, phân quyền và quản lý danh mục**: tài khoản, khu vực, loại rác, điểm tập kết.
2. **Quản lý người dân và đăng ký lịch thu gom**: hồ sơ người dân, yêu cầu, chi tiết yêu cầu, tra cứu lịch sử.
3. **Quản lý nhân viên, phân công và xử lý thu gom**: nhân viên, phân công, cập nhật trạng thái và kết quả.
4. **Theo dõi kết quả, Dashboard và thống kê**: theo dõi yêu cầu, kết quả, dashboard, thống kê LINQ.

## Entity tối thiểu

`TaiKhoan`, `KhuVuc`, `LoaiRac`, `DiemTapKet`, `NguoiDan`, `NhanVien`, `YeuCauThuGom`, `ChiTietYeuCau`, `PhanCongThuGom`, `KetQuaThuGom`.

## Chạy dự án

```bash
dotnet restore
dotnet build
dotnet run
```

Cấu hình chuỗi kết nối SQL Server trong `appsettings.json`. Sau khi cài `dotnet-ef` và hoàn thiện Entity:

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

Cấu trúc hiện tại là khung MVC ban đầu; các Controller, View, Entity, Validation và nghiệp vụ sẽ được bổ sung theo từng Module.
