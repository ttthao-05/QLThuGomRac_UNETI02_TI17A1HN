// Họ và tên: Nguyễn Tiến Đạt
// Mã sinh viên: 23103100053
// Nội dung: Module 3 – Form phân công thu gom (7.2)

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace QLThuGomRac_UNETI02_TI17A1HN.Models.ViewModels.Module3;

public class PhanCongCreateViewModel
{
    [Range(1, int.MaxValue)]
    public int MaYeuCau { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn nhân viên.")]
    [Display(Name = "Nhân viên phụ trách")]
    public int MaNhanVien { get; set; }

    [StringLength(1000, ErrorMessage = "Ghi chú tối đa 1000 ký tự.")]
    [Display(Name = "Ghi chú")]
    public string? GhiChu { get; set; }

    [BindNever]
    public string? DiaChiThuGom { get; set; }

    [BindNever]
    public string? TenKhuVuc { get; set; }

    [BindNever]
    public List<SelectListItem> NhanViens { get; set; } = [];
}
