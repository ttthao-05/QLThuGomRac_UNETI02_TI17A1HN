// Họ và tên: Phạm Văn Hào
// Mã sinh viên: 23103100041
// Nội dung thực hiện: Module 2 – Chi tiết yêu cầu thu gom (§6.3)

using System.ComponentModel.DataAnnotations;

namespace QLThuGomRac_UNETI02_TI17A1HN.Models.Entities;

public class ChiTietYeuCau
{
    // §6.3: một dòng loại rác thuộc một yêu cầu thu gom.
    [Key] public int MaChiTiet { get; set; }
    public int MaYeuCau { get; set; }
    public int MaLoaiRac { get; set; }
    public decimal SoLuongDuKien { get; set; }
    [MaxLength(500)] public string? GhiChu { get; set; }
    public YeuCauThuGom YeuCauThuGom { get; set; } = null!;
    public LoaiRac LoaiRac { get; set; } = null!;
}
