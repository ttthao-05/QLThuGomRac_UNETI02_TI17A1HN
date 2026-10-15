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

    [Display(Name = "Phân công thu gom")]
    public int MaPhanCong { get; set; }

    [Display(Name = "Thời gian ghi nhận")]
    public DateTime NgayGhiNhan { get; set; } = DateTime.Now;

    [StringLength(1000, ErrorMessage = "Ghi chú tối đa 1000 ký tự.")]
    [Display(Name = "Ghi chú kết quả")]
    public string? GhiChu { get; set; }

    public PhanCongThuGom PhanCongThuGom { get; set; } = null!;

    public List<ChiTietKetQuaThuGom> ChiTietKetQuas { get; set; }
        = new();
}
