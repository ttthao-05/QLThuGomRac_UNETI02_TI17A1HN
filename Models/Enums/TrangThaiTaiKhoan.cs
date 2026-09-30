// Họ và tên: Trần Thị Thảo
// Mã sinh viên: 23103100025
// Nội dung thực hiện: Module 1 – Quản lý tài khoản (§5.1 Quản lý tài khoản)

using System.ComponentModel.DataAnnotations;

namespace QLThuGomRac_UNETI02_TI17A1HN.Models.Enums;

/// <summary>Trạng thái tài khoản: Đang hoạt động hoặc Đã khóa.</summary>
public enum TrangThaiTaiKhoan
{
    [Display(Name = "Đang hoạt động")]
    DangHoatDong = 1,

    [Display(Name = "Đã khóa")]
    DaKhoa = 2
}
