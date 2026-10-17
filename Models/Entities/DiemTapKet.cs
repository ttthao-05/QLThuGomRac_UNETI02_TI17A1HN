// Họ và tên: Trần Thị Thảo
// Mã sinh viên: 23103100025
// Nội dung thực hiện: Module 1 – Quản lý điểm tập kết

using System.ComponentModel.DataAnnotations;
using QLThuGomRac_UNETI02_TI17A1HN.Models.Enums;

namespace QLThuGomRac_UNETI02_TI17A1HN.Models.Entities;

public class DiemTapKet
{
    [Key]
    [Display(Name = "Mã điểm tập kết")]
    public int MaDiemTapKet { get; set; }

    [Required(ErrorMessage = "Tên điểm tập kết là bắt buộc.")]
    [StringLength(100, ErrorMessage = "Tên điểm tập kết không được quá 100 ký tự.")]
    [Display(Name = "Tên điểm tập kết")]
    public string TenDiemTapKet { get; set; } = string.Empty;

    [Required(ErrorMessage = "Khu vực là bắt buộc.")]
    [Display(Name = "Khu vực")]
    public int MaKhuVuc { get; set; }

    [Required(ErrorMessage = "Địa chỉ là bắt buộc.")]
    [StringLength(255, ErrorMessage = "Địa chỉ không được quá 255 ký tự.")]
    [Display(Name = "Địa chỉ")]
    public string DiaChi { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Mô tả không được quá 500 ký tự.")]
    [Display(Name = "Mô tả")]
    public string? MoTa { get; set; }

    [Display(Name = "Trạng thái")]
    public TrangThaiDiemTapKet TrangThai { get; set; }
        = TrangThaiDiemTapKet.DangHoatDong;

    // Navigation Property
    public KhuVuc? KhuVuc { get; set; }
}
