# Module 2 – Người dân và đăng ký lịch thu gom

## Sử dụng

Chạy `dotnet run`. Ứng dụng tự áp dụng migration `Module2NguoiDanVaThuGom` và bổ sung hồ sơ cho tài khoản người dân hiện có. Tài khoản người dân tạo mới hoặc chuyển từ vai trò khác qua quản lý tài khoản cũng được tạo hồ sơ trong cùng lần lưu.

- Admin: menu **Quản lý người dân** (`/NguoiDan`), tìm theo họ tên/điện thoại/email, lọc trạng thái, xem chi tiết và khóa/mở hồ sơ.
- Người dân: **Thông tin cá nhân** (`/HoSo`), **Đăng ký thu gom** (`/YeuCauThuGom/Create`), **Yêu cầu của tôi** (`/YeuCauThuGom`).
- Dữ liệu mẫu hiện có: `admin`, `nd01`, `nd02`; mật khẩu `123456`. Chỉ dùng các tài khoản mẫu trong môi trường phát triển.
- Danh mục tối thiểu `LoaiRac` gồm giấy, nhựa, kim loại, thủy tinh; đơn vị kg. Cột `DangThuGom` quyết định có được đăng ký hay không. Giao diện quản trị loại rác và nghiệp vụ phân công/chuyển trạng thái yêu cầu thuộc module tiếp theo.

## Quy tắc nghiệp vụ

- Họ tên bắt buộc, tối đa 100 ký tự. Email tùy chọn nhưng phải đúng định dạng khi nhập. Số điện thoại dùng định dạng di động Việt Nam, bắt đầu bằng 0 hoặc +84. Ngày sinh tùy chọn, không vượt ngày hiện tại hoặc quá 120 năm trước. Giới tính tùy chọn.
- Chọn ngày từ **ngày mai**; khung giờ 08–10, 10–12, 13–15 hoặc 15–17. Quy tắc này được kiểm tra trên máy chủ.
- Ít nhất một loại rác, mỗi loại xuất hiện một lần; số lượng từ 0,001 đến 999.999.999,999, lưu với ba chữ số thập phân.
- Tài khoản và hồ sơ phải tồn tại, đang hoạt động và đúng vai trò; khu vực hoạt động; loại rác đang thu gom.
- Trùng không hợp lệ: cùng người dân, khu vực, địa chỉ (chuẩn hóa khoảng trắng và chữ hoa/thường), ngày, khung giờ với một yêu cầu chưa từ chối/hủy. Thay đổi loại rác không vượt qua quy tắc chống trùng. Yêu cầu từ chối/hủy cho phép đăng ký lại.
- Mã người dân lấy từ Session, trạng thái ban đầu và ngày đăng ký do máy chủ tạo. Người dân không thể sửa mã tài khoản, vai trò, trạng thái hay xem chi tiết/lịch sử người khác qua tham số URL/form.
- Lưu yêu cầu và toàn bộ chi tiết trong giao dịch Serializable; chỉ mục unique có điều kiện chống gửi trùng đồng thời. Khóa ngoại bảo vệ lịch sử; khu vực đã có yêu cầu không được xóa.
- Khóa hồ sơ ngăn đăng ký/cập nhật cá nhân; người dân vẫn xem được lịch sử. Khóa tài khoản hoặc đổi vai trò chặn truy cập Module 2 ngay với Session cũ.
- Có đủ bảy trạng thái: chờ xác nhận, đã xác nhận, đã phân công, đang thu gom, hoàn thành, từ chối, hủy. Nhóm đang xử lý gồm đã phân công/đang thu gom.

## Kiểm thử tích hợp

### LocalDB khi chạy F5

Trong môi trường Development trên Windows, `DevelopmentLocalDb` lấy địa chỉ named pipe hiện tại của instance từ `sqllocaldb info`. Nếu công cụ báo dừng nhưng SQL vẫn chạy (đã quan sát trên Visual Studio của máy phát triển), ứng dụng đọc địa chỉ từ nhật ký khởi động LocalDB và kiểm tra pipe còn tồn tại. Địa chỉ được tìm lại mỗi lần khởi động, không lưu cố định trong cấu hình. Instance chưa chạy sẽ được yêu cầu khởi động bằng `sqllocaldb start`. Tên CSDL, xác thực Windows và các thiết lập kết nối khác được giữ nguyên. Cấu hình SQL Server khác và môi trường Production không dùng bước này.

Nếu LocalDB thực sự không khởi động được, kiểm tra/cài SQL Server Express LocalDB và chạy `sqllocaldb start MSSQLLocalDB` trong terminal. Không xóa instance hay CSDL để xử lý lỗi kết nối.

### Chạy bộ kiểm thử

Yêu cầu .NET 10, SQL Server LocalDB, Python 3 và `sqlcmd`. Bài kiểm thử gửi HTTP thật, dùng cookie/anti-forgery thật và kiểm tra dữ liệu SQL. Chỉ chạy trên CSDL thử nghiệm riêng; bài kiểm thử có tạo yêu cầu và thay đổi dữ liệu mẫu trong CSDL này.

Terminal PowerShell thứ nhất:

```powershell
$env:ConnectionStrings__DefaultConnection='Server=(localdb)\MSSQLLocalDB;Database=QLThuGomRac_Module2_Test_20261001;Trusted_Connection=True;TrustServerCertificate=True'
dotnet run --no-launch-profile --urls http://127.0.0.1:5187
```

Terminal thứ hai:

```powershell
python Tests/module2_smoke.py
```

Kiểm tra: quyền truy cập, chống CSRF, xác thực hồ sơ, nhiều loại rác, ngày/số lượng/điện thoại sai, danh mục ngừng hoạt động, không lưu dữ liệu sai, chống giả mạo chủ sở hữu/trạng thái, lọc đủ bảy trạng thái, bảo vệ khu vực có lịch sử, khóa tài khoản/hồ sơ, đăng ký trùng và gửi đồng thời.
