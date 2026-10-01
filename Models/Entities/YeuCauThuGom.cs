// Họ và tên: Phạm Văn Hào
// Mã sinh viên: 23103100041
// Nội dung thực hiện: Module 2 – Yêu cầu thu gom (§6.2)

using System.ComponentModel.DataAnnotations;
using QLThuGomRac_UNETI02_TI17A1HN.Models.Enums;

namespace QLThuGomRac_UNETI02_TI17A1HN.Models.Entities;

public class YeuCauThuGom
{
    [Key] public int MaYeuCau { get; set; }
    public int MaNguoiDan { get; set; }
    public int MaKhuVuc { get; set; }
    [MaxLength(255)] public string DiaChiThuGom { get; set; } = string.Empty;
    [MaxLength(15)] public string SoDienThoaiLienHe { get; set; } = string.Empty;
    public DateTime NgayDangKy { get; set; } = DateTime.Now;
    public DateTime NgayMongMuonThuGom { get; set; }
    [MaxLength(30)] public string KhungGio { get; set; } = string.Empty;
    [MaxLength(1000)] public string? GhiChu { get; set; }
    public TrangThaiYeuCau TrangThai { get; set; } = TrangThaiYeuCau.ChoXacNhan;
    // SHA-256 của người dân/khu vực/địa chỉ/ngày/khung giờ; unique cho yêu cầu chưa từ chối/hủy.
    [MaxLength(64)] public string KhoaChongTrung { get; set; } = string.Empty;
    public NguoiDan NguoiDan { get; set; } = null!;
    public KhuVuc KhuVuc { get; set; } = null!;
    public List<ChiTietYeuCau> ChiTietYeuCaus { get; set; } = [];
}
