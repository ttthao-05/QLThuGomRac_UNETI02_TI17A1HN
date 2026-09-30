// Họ và tên: Trần Thị Thảo
// Mã sinh viên: 23103100025
// Nội dung thực hiện: Module 1 – Quản lý tài khoản (§5.1 Quản lý tài khoản)

using Microsoft.EntityFrameworkCore;
using QLThuGomRac_UNETI02_TI17A1HN.Models.Entities;
using QLThuGomRac_UNETI02_TI17A1HN.Models.Enums;

namespace QLThuGomRac_UNETI02_TI17A1HN.Data.Seed;

/// <summary>
/// Dữ liệu mẫu cho bảng TaiKhoan theo §15:
/// 02 tài khoản Admin, 05 tài khoản Nhân viên, 10 tài khoản Người dân.
/// Các Module khác sẽ bổ sung dữ liệu mẫu của riêng mình.
/// </summary>
public static class SeedData
{
    public static void EnsureSeeded(ApplicationDbContext context)
    {
        if (context.TaiKhoans.Any())
        {
            return;
        }

        var danhSach = new List<TaiKhoan>
        {
            // 02 tài khoản Admin (§15)
            new() { TenDangNhap = "admin", MatKhau = "123456", HoTen = "Quản trị viên", SoDienThoai = "0901000001", LoaiTaiKhoan = LoaiTaiKhoan.Admin, TrangThai = TrangThaiTaiKhoan.DangHoatDong },
            new() { TenDangNhap = "admin2", MatKhau = "123456", HoTen = "Quản trị viên phụ", SoDienThoai = "0901000002", LoaiTaiKhoan = LoaiTaiKhoan.Admin, TrangThai = TrangThaiTaiKhoan.DangHoatDong },

            // 05 tài khoản Nhân viên, tương ứng 05 nhân viên của §15
            new() { TenDangNhap = "nv01", MatKhau = "123456", HoTen = "Nhân viên 01", SoDienThoai = "0902000001", LoaiTaiKhoan = LoaiTaiKhoan.NhanVien, TrangThai = TrangThaiTaiKhoan.DangHoatDong },
            new() { TenDangNhap = "nv02", MatKhau = "123456", HoTen = "Nhân viên 02", SoDienThoai = "0902000002", LoaiTaiKhoan = LoaiTaiKhoan.NhanVien, TrangThai = TrangThaiTaiKhoan.DangHoatDong },
            new() { TenDangNhap = "nv03", MatKhau = "123456", HoTen = "Nhân viên 03", SoDienThoai = "0902000003", LoaiTaiKhoan = LoaiTaiKhoan.NhanVien, TrangThai = TrangThaiTaiKhoan.DangHoatDong },
            new() { TenDangNhap = "nv04", MatKhau = "123456", HoTen = "Nhân viên 04", SoDienThoai = "0902000004", LoaiTaiKhoan = LoaiTaiKhoan.NhanVien, TrangThai = TrangThaiTaiKhoan.DangHoatDong },
            new() { TenDangNhap = "nv05", MatKhau = "123456", HoTen = "Nhân viên 05", SoDienThoai = "0902000005", LoaiTaiKhoan = LoaiTaiKhoan.NhanVien, TrangThai = TrangThaiTaiKhoan.DangHoatDong },

            // 10 tài khoản Người dân, tương ứng 10 người dân của §15
            new() { TenDangNhap = "nd01", MatKhau = "123456", HoTen = "Người dân 01", SoDienThoai = "0903000001", LoaiTaiKhoan = LoaiTaiKhoan.NguoiDan, TrangThai = TrangThaiTaiKhoan.DangHoatDong },
            new() { TenDangNhap = "nd02", MatKhau = "123456", HoTen = "Người dân 02", SoDienThoai = "0903000002", LoaiTaiKhoan = LoaiTaiKhoan.NguoiDan, TrangThai = TrangThaiTaiKhoan.DangHoatDong },
            new() { TenDangNhap = "nd03", MatKhau = "123456", HoTen = "Người dân 03", SoDienThoai = "0903000003", LoaiTaiKhoan = LoaiTaiKhoan.NguoiDan, TrangThai = TrangThaiTaiKhoan.DangHoatDong },
            new() { TenDangNhap = "nd04", MatKhau = "123456", HoTen = "Người dân 04", SoDienThoai = "0903000004", LoaiTaiKhoan = LoaiTaiKhoan.NguoiDan, TrangThai = TrangThaiTaiKhoan.DangHoatDong },
            new() { TenDangNhap = "nd05", MatKhau = "123456", HoTen = "Người dân 05", SoDienThoai = "0903000005", LoaiTaiKhoan = LoaiTaiKhoan.NguoiDan, TrangThai = TrangThaiTaiKhoan.DangHoatDong },
            new() { TenDangNhap = "nd06", MatKhau = "123456", HoTen = "Người dân 06", SoDienThoai = "0903000006", LoaiTaiKhoan = LoaiTaiKhoan.NguoiDan, TrangThai = TrangThaiTaiKhoan.DangHoatDong },
            new() { TenDangNhap = "nd07", MatKhau = "123456", HoTen = "Người dân 07", SoDienThoai = "0903000007", LoaiTaiKhoan = LoaiTaiKhoan.NguoiDan, TrangThai = TrangThaiTaiKhoan.DangHoatDong },
            new() { TenDangNhap = "nd08", MatKhau = "123456", HoTen = "Người dân 08", SoDienThoai = "0903000008", LoaiTaiKhoan = LoaiTaiKhoan.NguoiDan, TrangThai = TrangThaiTaiKhoan.DangHoatDong },
            new() { TenDangNhap = "nd09", MatKhau = "123456", HoTen = "Người dân 09", SoDienThoai = "0903000009", LoaiTaiKhoan = LoaiTaiKhoan.NguoiDan, TrangThai = TrangThaiTaiKhoan.DangHoatDong },
            new() { TenDangNhap = "nd10", MatKhau = "123456", HoTen = "Người dân 10", SoDienThoai = "0903000010", LoaiTaiKhoan = LoaiTaiKhoan.NguoiDan, TrangThai = TrangThaiTaiKhoan.DaKhoa }
        };

        context.TaiKhoans.AddRange(danhSach);
        context.SaveChanges();
    }
}
