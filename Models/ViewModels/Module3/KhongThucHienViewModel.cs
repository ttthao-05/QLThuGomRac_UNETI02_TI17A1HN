// Họ và tên: Nguyễn Tiến Đạt
// Mã sinh viên: 23103100053
// Nội dung: Module 3 – Xử lý yêu cầu thu gom (7.3)

using System.ComponentModel.DataAnnotations;

namespace QLThuGomRac_UNETI02_TI17A1HN.Models.ViewModels.Module3;

public class KhongThucHienViewModel
{
    [Range(1, int.MaxValue, ErrorMessage = "Phân công không hợp lệ.")]
    public int MaPhanCong { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập lý do không thực hiện được.")]
    [StringLength(1000, ErrorMessage = "Lý do tối đa 1000 ký tự.")]
    [Display(Name = "Lý do không thực hiện được")]
    public string LyDoKhongThucHien { get; set; } = string.Empty;
}
