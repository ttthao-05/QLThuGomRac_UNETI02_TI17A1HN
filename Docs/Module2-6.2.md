# Module 2 – §6.2 Quản lý yêu cầu thu gom

Entity `YeuCauThuGom` lưu thông tin đăng ký thu gom của người dân: người yêu cầu, khu vực, địa chỉ, số điện thoại liên hệ, ngày đăng ký, ngày mong muốn thu gom, khung giờ, ghi chú và trạng thái.

Các trạng thái được hỗ trợ trong `TrangThaiYeuCau`:

- Chờ xác nhận
- Đã xác nhận
- Đã phân công
- Đang thu gom
- Hoàn thành
- Từ chối
- Hủy

Người dân chỉ được tra cứu yêu cầu thuộc tài khoản của mình. Việc tạo yêu cầu được thực hiện qua `YeuCauThuGomController` và kiểm tra tài khoản, hồ sơ, khu vực hoạt động, lịch thu gom và dữ liệu chi tiết trước khi lưu.
