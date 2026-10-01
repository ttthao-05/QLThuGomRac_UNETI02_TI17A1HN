// Họ và tên: Trần Thị Thảo
// Mã sinh viên: 23103100025
// Nội dung: Module 1 – Quản lý loại rác (5.6)

using System.ComponentModel.DataAnnotations;
using QLThuGomRac_UNETI02_TI17A1HN.Models.Enums;

namespace QLThuGomRac_UNETI02_TI17A1HN.Models.Entities;

public class LoaiRac
{
    [Key]
    [Display(Name = "Mã loại rác")]
    public int MaLoaiRac { get; set; }

    [Required(ErrorMessage = "Tên loại rác là bắt buộc.")]
    [StringLength(100, ErrorMessage = "Tên loại rác không được quá 100 ký tự.")]
    [Display(Name = "Tên loại rác")]
    public string TenLoaiRac { get; set; } = string.Empty;

    [Required(ErrorMessage = "Đơn vị tính là bắt buộc.")]
    [StringLength(50)]
    [Display(Name = "Đơn vị tính")]
    public string DonViTinh { get; set; } = string.Empty;

    [StringLength(500)]
    [Display(Name = "Mô tả")]
    public string? MoTa { get; set; }

    [Display(Name = "Trạng thái")]
    public TrangThaiLoaiRac TrangThai { get; set; }
        = TrangThaiLoaiRac.DangPhucVu;
}
