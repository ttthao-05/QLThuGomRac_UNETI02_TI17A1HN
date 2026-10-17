// Họ và tên: Trần Thị Thảo
// Mã sinh viên: 23103100025
// Nội dung thực hiện: Module 1 – Quản lý điểm tập kết

using System.ComponentModel.DataAnnotations;

namespace QLThuGomRac_UNETI02_TI17A1HN.Models.Enums;

public enum TrangThaiDiemTapKet
{
    [Display(Name = "Đang hoạt động")]
    DangHoatDong = 1,

    [Display(Name = "Ngừng hoạt động")]
    NgungHoatDong = 2
}
