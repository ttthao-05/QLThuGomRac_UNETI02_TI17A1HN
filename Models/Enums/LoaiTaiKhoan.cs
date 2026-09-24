// Họ và tên: Trần Thị Thảo
// Mã sinh viên: 23103100025
// Nội dung thực hiện: Module 1 – Quản lý tài khoản (§5.1 Quản lý tài khoản)

using System.ComponentModel.DataAnnotations;

namespace QLThuGomRac_UNETI02_TI17A1HN.Models.Enums;

/// <summary>Loại tài khoản của hệ thống (§5.1): Admin, Nhân viên, Người dân.</summary>
public enum LoaiTaiKhoan
{
    [Display(Name = "Quản trị viên (Admin)")]
    Admin = 1,

    [Display(Name = "Nhân viên")]
    NhanVien = 2,

    [Display(Name = "Người dân")]
    NguoiDan = 3
}
