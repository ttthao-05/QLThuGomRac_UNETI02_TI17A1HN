// Họ và tên: Phạm Văn Hào
// Mã sinh viên: 23103100041
// Nội dung thực hiện: Module 2 – Validation người dân (§6.1)

using System.ComponentModel.DataAnnotations;
using QLThuGomRac_UNETI02_TI17A1HN.Models.Enums;

namespace QLThuGomRac_UNETI02_TI17A1HN.Models.ViewModels.Module2;

public class HoSoViewModel : IValidatableObject
{
    [Required(ErrorMessage = "Họ tên là bắt buộc."), StringLength(100), Display(Name = "Họ tên")]
    public string HoTen { get; set; } = string.Empty;
    [DataType(DataType.Date), Display(Name = "Ngày sinh")]
    public DateTime? NgaySinh { get; set; }
    [EnumDataType(typeof(GioiTinh)), Display(Name = "Giới tính")]
    public GioiTinh? GioiTinh { get; set; }
    [Required(ErrorMessage = "Số điện thoại là bắt buộc."), RegularExpression(@"^(0[35789]\d{8}|\+84[35789]\d{8})$", ErrorMessage = "Nhập số di động Việt Nam hợp lệ (0 hoặc +84)."), Display(Name = "Số điện thoại")]
    public string SoDienThoai { get; set; } = string.Empty;
    [EmailAddress(ErrorMessage = "Email không đúng định dạng."), StringLength(254), Display(Name = "Email")]
    public string? Email { get; set; }
    [StringLength(255), Display(Name = "Địa chỉ")]
    public string? DiaChi { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (NgaySinh.HasValue && (NgaySinh.Value.Date > DateTime.Today || NgaySinh.Value.Date < DateTime.Today.AddYears(-120)))
            yield return new ValidationResult("Ngày sinh phải nằm trong 120 năm gần đây và không ở tương lai.", [nameof(NgaySinh)]);
    }
}
