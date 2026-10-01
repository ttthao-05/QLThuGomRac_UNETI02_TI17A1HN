// Họ và tên: Phạm Văn Hào
// Mã sinh viên: 23103100041
// Nội dung thực hiện: Module 2 – Danh mục loại rác (§6.3–§6.4)

using System.ComponentModel.DataAnnotations;

namespace QLThuGomRac_UNETI02_TI17A1HN.Models.Entities;

public class LoaiRac
{
    [Key] public int MaLoaiRac { get; set; }
    [MaxLength(100)] public string TenLoaiRac { get; set; } = string.Empty;
    [MaxLength(20)] public string DonViTinh { get; set; } = "kg";
    public bool DangThuGom { get; set; } = true;
}
