// Họ và tên: Phạm Văn Hào
// Mã sinh viên: 23103100041
// Nội dung thực hiện: Module 2 – Đăng ký lịch thu gom (§6.3–§6.4)

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace QLThuGomRac_UNETI02_TI17A1HN.Models.ViewModels.Module2;

public class DangKyThuGomViewModel : IValidatableObject
{
    public static readonly string[] CacKhungGio = ["08:00 - 10:00", "10:00 - 12:00", "13:00 - 15:00", "15:00 - 17:00"];
    [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn khu vực."), Display(Name = "Khu vực")]
    public int MaKhuVuc { get; set; }
    [Required(ErrorMessage = "Địa chỉ thu gom là bắt buộc."), StringLength(255), Display(Name = "Địa chỉ thu gom")]
    public string DiaChiThuGom { get; set; } = string.Empty;
    [Required, RegularExpression(@"^(0[35789]\d{8}|\+84[35789]\d{8})$", ErrorMessage = "Số điện thoại liên hệ không hợp lệ."), Display(Name = "Số điện thoại liên hệ")]
    public string SoDienThoaiLienHe { get; set; } = string.Empty;
    [DataType(DataType.Date), Display(Name = "Ngày mong muốn thu gom")]
    public DateTime NgayMongMuonThuGom { get; set; } = DateTime.Today.AddDays(1);
    [Required, Display(Name = "Khung giờ")]
    public string KhungGio { get; set; } = string.Empty;
    [StringLength(1000), Display(Name = "Ghi chú")]
    public string? GhiChu { get; set; }
    public List<ChiTietDangKyViewModel> ChiTiet { get; set; } = [];
    [ValidateNever] public List<SelectListItem> KhuVucItems { get; set; } = [];
    [ValidateNever] public List<SelectListItem> LoaiRacItems { get; set; } = [];

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (NgayMongMuonThuGom.Date <= DateTime.Today)
            yield return new ValidationResult("Vui lòng chọn ngày thu gom từ ngày mai.", [nameof(NgayMongMuonThuGom)]);
        if (!CacKhungGio.Contains(KhungGio))
            yield return new ValidationResult("Khung giờ không hợp lệ.", [nameof(KhungGio)]);
        if (ChiTiet == null || ChiTiet.Count == 0)
            yield return new ValidationResult("Yêu cầu phải có ít nhất một loại rác.", [nameof(ChiTiet)]);
        else if (ChiTiet.Select(x => x.MaLoaiRac).Distinct().Count() != ChiTiet.Count)
            yield return new ValidationResult("Mỗi loại rác chỉ được chọn một lần.", [nameof(ChiTiet)]);
    }
}

public class ChiTietDangKyViewModel
{
    [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn loại rác.")]
    public int MaLoaiRac { get; set; }
    [Range(typeof(decimal), "0.001", "999999999.999", ErrorMessage = "Số lượng phải từ 0,001 đến 999.999.999,999.")]
    public decimal SoLuongDuKien { get; set; }
    [StringLength(500)] public string? GhiChu { get; set; }
}
