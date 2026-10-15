// Họ và tên: Nguyễn Tiến Đạt
// Mã sinh viên: 23103100053
// Nội dung: Module 3 – Form nhập kết quả thu gom (7.4)

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace QLThuGomRac_UNETI02_TI17A1HN.Models.ViewModels.Module3;

public class KetQuaThuGomCreateViewModel
{
    [Range(1, int.MaxValue, ErrorMessage = "Phân công không hợp lệ.")]
    public int MaPhanCong { get; set; }

    // Thông tin hiển thị được lấy lại từ database.
    [BindNever]
    [ValidateNever]
    public string DiaChiThuGom { get; set; } = string.Empty;

    [BindNever]
    [ValidateNever]
    public string TenKhuVuc { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Ghi chú tối đa 1000 ký tự.")]
    [Display(Name = "Ghi chú kết quả")]
    public string? GhiChu { get; set; }

    [MinLength(1, ErrorMessage = "Yêu cầu phải có ít nhất một loại rác.")]
    public List<ChiTietKetQuaInputViewModel> ChiTiets { get; set; }
        = new();
}

public class ChiTietKetQuaInputViewModel
{
    [Range(1, int.MaxValue, ErrorMessage = "Loại rác không hợp lệ.")]
    public int MaLoaiRac { get; set; }

    [BindNever]
    [ValidateNever]
    public string TenLoaiRac { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập khối lượng thực tế.")]
    [Range(
        typeof(decimal),
        "0",
        "999999999.999",
        ErrorMessage = "Khối lượng phải từ 0 đến 999999999.999 kg.")]
    [Display(Name = "Khối lượng thực tế (kg)")]
    public decimal? KhoiLuongThucTe { get; set; }
}
