// Họ tên: Trần Thị Thảo
// Msv: 23103100025
// ND: Module 1 – Đăng nhập (§5.2)

using System.ComponentModel.DataAnnotations;

namespace QLThuGomRac_UNETI02_TI17A1HN.Models.ViewModels.Module1;

public class LoginViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập.")]
    [Display(Name = "Tên đăng nhập")]
    public string TenDangNhap { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
    [DataType(DataType.Password)]
    [Display(Name = "Mật khẩu")]
    public string MatKhau { get; set; } = string.Empty;
}
