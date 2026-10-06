// Họ và tên: Nguyễn Tiến Đạt
// Mã sinh viên: 23103100053
// Nội dung: Module 3 – Hiển thị danh sách nhân viên

using System;
using QLThuGomRac_UNETI02_TI17A1HN.Models.Enums;

namespace QLThuGomRac_UNETI02_TI17A1HN.Models.ViewModels.Module3;

public class NhanVienDanhSachViewModel
{
    public int MaNhanVien { get; set; }

    public string TenDangNhap { get; set; } = string.Empty;

    public string HoTen { get; set; } = string.Empty;

    public string SoDienThoai { get; set; } = string.Empty;

    public string TenKhuVuc { get; set; } = string.Empty;

    public DateTime NgayVaoLam { get; set; }

    public TrangThaiNhanVien TrangThai { get; set; }
}
