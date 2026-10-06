// Họ và tên: Nguyễn Tiến Đạt
// Mã sinh viên: 23103100053
// Nội dung thực hiện: Module 3 – Trạng thái nhân viên

using System.ComponentModel.DataAnnotations;

namespace QLThuGomRac_UNETI02_TI17A1HN.Models.Enums;
public enum TrangThaiNhanVien
{
    [Display(Name = "Đang hoạt động")]
    DangHoatDong = 0,

    [Display(Name = "Tạm nghỉ")]
    TamNghi = 1
}
