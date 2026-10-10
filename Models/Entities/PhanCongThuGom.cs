// Họ và tên: Nguyễn Tiến Đạt
// Mã sinh viên: 23103100053
// Nội dung thực hiện: Module 3 – Phân công thu gom (§7.2)

using System.ComponentModel.DataAnnotations;
using QLThuGomRac_UNETI02_TI17A1HN.Models.Enums;

namespace QLThuGomRac_UNETI02_TI17A1HN.Models.Entities;

public class PhanCongThuGom
{
    [Key]
    [Display(Name = "Mã phân công")]
    public int MaPhanCong { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn yêu cầu.")]
    [Display(Name = "Yêu cầu thu gom")]
    public int MaYeuCau { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn nhân viên.")]
    [Display(Name = "Nhân viên phụ trách")]
    public int MaNhanVien { get; set; }

    [Display(Name = "Ngày phân công")]
    public DateTime NgayPhanCong { get; set; } = DateTime.Now;

    [StringLength(1000, ErrorMessage = "Ghi chú tối đa 1000 ký tự.")]
    [Display(Name = "Ghi chú")]
    public string? GhiChu { get; set; }

    [EnumDataType(typeof(TrangThaiYeuCau))]
    [Display(Name = "Trạng thái")]
    public TrangThaiYeuCau TrangThai { get; set; }
        = TrangThaiYeuCau.DaPhanCong;

    public YeuCauThuGom? YeuCauThuGom { get; set; }

    public NhanVien? NhanVien { get; set; }
    // Nguyễn Tiến Đạt – 23103100053
    // Module 3 – Xử lý yêu cầu thu gom (7.3)

    [StringLength(1000, ErrorMessage = "Lý do tối đa 1000 ký tự.")]
    [Display(Name = "Lý do không thực hiện được")]
    public string? LyDoKhongThucHien { get; set; }

    [Display(Name = "Thời gian bắt đầu")]
    public DateTime? NgayBatDau { get; set; }

    [Display(Name = "Thời gian kết thúc")]
    public DateTime? NgayKetThuc { get; set; }
}
