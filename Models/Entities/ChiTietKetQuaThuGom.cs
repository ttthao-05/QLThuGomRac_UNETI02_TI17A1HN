// Họ và tên: Nguyễn Tiến Đạt
// Mã sinh viên: 23103100053
// Nội dung: Module 3 – Chi tiết kết quả thu gom (7.4)

using System.ComponentModel.DataAnnotations;

namespace QLThuGomRac_UNETI02_TI17A1HN.Models.Entities;

public class ChiTietKetQuaThuGom
{
    [Key]
    public int MaChiTietKetQua { get; set; }

    public int MaKetQua { get; set; }

    [Display(Name = "Loại rác")]
    public int MaLoaiRac { get; set; }

    [Range(
        typeof(decimal),
        "0",
        "999999999.999",
        ErrorMessage = "Khối lượng phải từ 0 đến 999999999.999 kg.")]
    [Display(Name = "Khối lượng thực tế (kg)")]
    public decimal KhoiLuongThucTe { get; set; }

    public KetQuaThuGom KetQuaThuGom { get; set; } = null!;

    public LoaiRac LoaiRac { get; set; } = null!;
}
