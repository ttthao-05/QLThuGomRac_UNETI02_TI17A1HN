// Họ và tên: Nguyễn Tiến Đạt
// Mã sinh viên: 23103100053
// Nội dung: Module 3 – Kết quả thu gom (7.4)

using System.ComponentModel.DataAnnotations;

namespace QLThuGomRac_UNETI02_TI17A1HN.Models.Entities;

public class KetQuaThuGom
{
    [Key]
    [Display(Name = "Mã kết quả")]
    public int MaKetQua { get; set; }

    [Display(Name = "Mã yêu cầu")]
    public int MaYeuCau { get; set; }

    [Display(Name = "Nhân viên thực hiện")]
    public int MaNhanVien { get; set; }

    [Display(Name = "Thời gian hoàn thành")]
    public DateTime ThoiGianHoanThanh { get; set; }

    [StringLength(1000, ErrorMessage = "Ghi chú tối đa 1000 ký tự.")]
    [Display(Name = "Ghi chú")]
    public string? GhiChu { get; set; }

    public YeuCauThuGom? YeuCauThuGom { get; set; }

    public NhanVien? NhanVien { get; set; }

    public ICollection<ChiTietKetQuaThuGom> ChiTietKetQuas { get; set; }
        = new List<ChiTietKetQuaThuGom>();
}
