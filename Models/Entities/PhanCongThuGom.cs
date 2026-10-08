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
}
