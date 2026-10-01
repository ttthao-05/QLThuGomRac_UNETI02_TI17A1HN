// Họ và tên: Trần Thị Thảo
// Mã sinh viên: 23103100025
// Nội dung: Module 1 – Quản lý khu vực (§5.5)

using System.ComponentModel.DataAnnotations;
using QLThuGomRac_UNETI02_TI17A1HN.Models.Enums;

namespace QLThuGomRac_UNETI02_TI17A1HN.Models.Entities;

public class KhuVuc
{
    [Key]
    [Display(Name = "Mã khu vực")]
    public int MaKhuVuc { get; set; }

    [Required(ErrorMessage = "Tên khu vực là bắt buộc")]
    [StringLength(100, ErrorMessage = "Tên khu vực không được quá 100 ký tự")]
    [Display(Name = "Tên khu vực")]
    public string TenKhuVuc { get; set; } = string.Empty;

    [StringLength(255, ErrorMessage = "Địa chỉ mô tả không được quá 255 ký tự")]
    [Display(Name = "Địa chỉ mô tả")]
    public string? DiaChiMoTa { get; set; }

    [Display(Name = "Trạng thái")]
    public TrangThaiKhuVuc TrangThai { get; set; } = TrangThaiKhuVuc.DangHoatDong;
}
