// Họ và tên: Phạm Văn Hào
// Mã sinh viên: 23103100041
// Nội dung thực hiện: Module 2 – Trạng thái Module 2 (§6.1–§6.2)

using System.ComponentModel.DataAnnotations;

namespace QLThuGomRac_UNETI02_TI17A1HN.Models.Enums;

public enum TrangThaiNguoiDan
{
    [Display(Name = "Đang hoạt động")] DangHoatDong = 1,
    [Display(Name = "Đã khóa")] DaKhoa = 2
}

public enum GioiTinh
{
    [Display(Name = "Nam")] Nam = 1,
    [Display(Name = "Nữ")] Nu = 2,
    [Display(Name = "Khác")] Khac = 3
}

public enum TrangThaiYeuCau
{
    [Display(Name = "Chờ xác nhận")] ChoXacNhan = 1,
    [Display(Name = "Đã xác nhận")] DaXacNhan = 2,
    [Display(Name = "Đã phân công")] DaPhanCong = 3,
    [Display(Name = "Đang thu gom")] DangThuGom = 4,
    [Display(Name = "Hoàn thành")] HoanThanh = 5,
    [Display(Name = "Từ chối")] TuChoi = 6,
    [Display(Name = "Hủy")] Huy = 7
}
