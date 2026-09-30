// Họ và tên: Trần Thị Thảo
// Mã sinh viên: 23103100025
// Nội dung thực hiện: Module 1 – Quản lý tài khoản (§5.1 Quản lý tài khoản)

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using QLThuGomRac_UNETI02_TI17A1HN.Models.Entities;
using QLThuGomRac_UNETI02_TI17A1HN.Models.Enums;

namespace QLThuGomRac_UNETI02_TI17A1HN.Models.ViewModels.Module1;

/// <summary>ViewModel cho màn danh sách tài khoản: tìm kiếm + lọc theo loại tài khoản và trạng thái.</summary>
public class TaiKhoanIndexViewModel
{
    [Display(Name = "Từ khóa")]
    public string? TuKhoa { get; set; }

    [Display(Name = "Loại tài khoản")]
    public LoaiTaiKhoan? LoaiTaiKhoan { get; set; }

    [Display(Name = "Trạng thái")]
    public TrangThaiTaiKhoan? TrangThai { get; set; }

    public List<TaiKhoan> DanhSach { get; set; } = new();

    public List<SelectListItem> LoaiTaiKhoanItems { get; set; } = new();

    public List<SelectListItem> TrangThaiItems { get; set; } = new();
}
