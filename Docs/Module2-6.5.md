# Module 2 – §6.5 Tra cứu yêu cầu thu gom

Người dân có thể xem toàn bộ lịch sử yêu cầu của chính mình và lọc theo các nhóm: chờ xác nhận, đã xác nhận, đang xử lý, hoàn thành, từ chối hoặc hủy.

Tại `/YeuCauThuGom/Index`, có thể tìm mã yêu cầu (có hoặc không có dấu #), địa chỉ, điện thoại và kết hợp lọc khu vực, trạng thái, khoảng ngày thu gom. Bộ chọn trạng thái cũng có “Không thực hiện được” để theo dõi nhánh xử lý Module 3. Danh sách sắp xếp theo ngày đăng ký mới nhất hoặc ngày thu gom tăng dần, phân trang 10 dòng và giữ các điều kiện khi chuyển trang. Khoảng ngày sai được thông báo trên View.

Các action tra cứu luôn lọc theo mã tài khoản đang đăng nhập. Vì vậy người dân không thể xem chi tiết hoặc lịch sử yêu cầu thuộc tài khoản khác bằng cách thay đổi mã trên URL.
