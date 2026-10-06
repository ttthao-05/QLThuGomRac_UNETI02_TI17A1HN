// Họ và tên: Phạm Văn Hào
// Mã sinh viên: 23103100041
// Nội dung thực hiện: Module 2 – Quản lý người dân (§6.1)

using System.ComponentModel.DataAnnotations;
using QLThuGomRac_UNETI02_TI17A1HN.Models.Enums;

namespace QLThuGomRac_UNETI02_TI17A1HN.Models.Entities;

public class NguoiDan
{
    [Key] public int MaNguoiDan { get; set; }
    public int MaTaiKhoan { get; set; }
    [MaxLength(100)] public string HoTen { get; set; } = string.Empty;
    public DateTime? NgaySinh { get; set; }
    public GioiTinh? GioiTinh { get; set; }
    [MaxLength(15)] public string SoDienThoai { get; set; } = string.Empty;
    [MaxLength(254)] public string? Email { get; set; }
    [MaxLength(255)] public string? DiaChi { get; set; }
    public DateTime NgayDangKy { get; set; } = DateTime.Now;
    public TrangThaiNguoiDan TrangThai { get; set; } = TrangThaiNguoiDan.DangHoatDong;
    public TaiKhoan TaiKhoan { get; set; } = null!;
}
