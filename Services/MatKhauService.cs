// Họ và tên: Trần Thị Thảo
// Mã sinh viên: 23103100025
// ND: Module 1 - Băm và xác minh mật khẩu

using Microsoft.AspNetCore.Identity;
using QLThuGomRac_UNETI02_TI17A1HN.Models.Entities;

namespace QLThuGomRac_UNETI02_TI17A1HN.Services;

public class MatKhauService
{
    private readonly PasswordHasher<TaiKhoan> _passwordHasher = new();

    // Băm mật khẩu trước khi lưu vào Database
    public string BamMatKhau(TaiKhoan taiKhoan, string matKhau)
    {
        return _passwordHasher.HashPassword(taiKhoan, matKhau);
    }

    // Xác minh mật khẩu người dùng nhập
    public bool KiemTraMatKhau(
        TaiKhoan taiKhoan,
        string matKhauNhap)
    {
        var ketQua = _passwordHasher.VerifyHashedPassword(
            taiKhoan,
            taiKhoan.MatKhau,
            matKhauNhap);

        return ketQua != PasswordVerificationResult.Failed;
    }
}
