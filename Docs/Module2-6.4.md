# Module 2 – §6.4 Đăng ký lịch thu gom

Người dân phải đăng nhập và có hồ sơ đang hoạt động mới được tạo yêu cầu. Hệ thống kiểm tra tài khoản/người dân, khu vực đang hoạt động, loại rác đang được thu gom, ngày thu gom từ ngày mai, khung giờ hợp lệ, số lượng lớn hơn 0 và ít nhất một loại rác.

Địa chỉ, khu vực, ngày và khung giờ được chuẩn hóa thành khóa chống trùng. Một yêu cầu đang xử lý không thể đăng ký lại cùng lịch; yêu cầu đã từ chối hoặc hủy có thể đăng ký lại. Yêu cầu và các dòng chi tiết được lưu trong cùng giao dịch để dữ liệu luôn đầy đủ.
