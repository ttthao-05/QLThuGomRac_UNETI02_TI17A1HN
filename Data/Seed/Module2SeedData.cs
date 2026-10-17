// Họ và tên: Phạm Văn Hào
// Mã sinh viên: 23103100041
// Nội dung thực hiện: Module 2 – Dữ liệu mẫu yêu cầu và chi tiết thu gom (§15)

using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using QLThuGomRac_UNETI02_TI17A1HN.Models.Entities;
using QLThuGomRac_UNETI02_TI17A1HN.Models.Enums;
using QLThuGomRac_UNETI02_TI17A1HN.Models.ViewModels.Module2;

namespace QLThuGomRac_UNETI02_TI17A1HN.Data.Seed;

public static class Module2SeedData
{
    public static void EnsureSeeded(ApplicationDbContext context)
    {
        var people = context.NguoiDans.AsNoTracking().Include(x => x.TaiKhoan)
            .Where(x => x.TaiKhoan.TenDangNhap.StartsWith("nd")
                && x.TaiKhoan.LoaiTaiKhoan == LoaiTaiKhoan.NguoiDan
                && x.TaiKhoan.TrangThai == TrangThaiTaiKhoan.DangHoatDong
                && x.TrangThai == TrangThaiNguoiDan.DangHoatDong)
            .OrderBy(x => x.MaNguoiDan).Take(10).ToList();
        var areas = context.KhuVucs.AsNoTracking()
            .Where(x => x.TrangThai == TrangThaiKhuVuc.DangHoatDong)
            .OrderBy(x => x.MaKhuVuc).ToList();
        var wastes = context.LoaiRacs.AsNoTracking()
            .Where(x => x.TrangThai == TrangThaiLoaiRac.DangPhucVu)
            .OrderBy(x => x.MaLoaiRac).ToList();
        if (people.Count == 0 || areas.Count == 0 || wastes.Count == 0) return;

        // Chỉ tạo trạng thái thuộc luồng Module 2. Các trạng thái đã phân công,
        // đang thu gom, hoàn thành phải có dữ liệu nghiệp vụ thật từ Module 3.
        TrangThaiYeuCau[] states = [TrangThaiYeuCau.ChoXacNhan, TrangThaiYeuCau.DaXacNhan,
            TrangThaiYeuCau.TuChoi, TrangThaiYeuCau.Huy];
        for (var i = 0; i < 20; i++)
        {
            // Nhãn cố định giúp chạy lại ứng dụng không sinh thêm bản sao,
            // kể cả khi yêu cầu mẫu đã được phân công hoặc chuyển trạng thái.
            var marker = $"[M2-MAU-{i + 1:00}]";
            if (context.YeuCauThuGoms.Any(x => x.GhiChu != null && x.GhiChu.StartsWith(marker))) continue;
            var person = people[i % people.Count];
            var area = areas[i % areas.Count];
            var date = DateTime.Today.AddDays(i + 1);
            var slot = DangKyThuGomViewModel.CacKhungGio[i % DangKyThuGomViewModel.CacKhungGio.Length];
            var address = $"Số {i + 1}, địa chỉ mẫu, {area.TenKhuVuc}";
            var key = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(
                $"{person.MaNguoiDan}|{area.MaKhuVuc}|{address.Normalize().ToUpperInvariant()}|{date:yyyy-MM-dd}|{slot}")));
            var request = new YeuCauThuGom
            {
                MaNguoiDan = person.MaNguoiDan, MaKhuVuc = area.MaKhuVuc,
                DiaChiThuGom = address, SoDienThoaiLienHe = person.SoDienThoai,
                NgayDangKy = DateTime.Today.AddDays(-i), NgayMongMuonThuGom = date,
                KhungGio = slot, KhoaChongTrung = key, TrangThai = states[i % states.Length],
                GhiChu = $"{marker} Dữ liệu minh họa Module 2."
            };
            for (var j = 0; j < Math.Min(2, wastes.Count); j++)
                request.ChiTietYeuCaus.Add(new ChiTietYeuCau
                {
                    MaLoaiRac = wastes[(i + j) % wastes.Count].MaLoaiRac,
                    SoLuongDuKien = 1 + (i + j) % 8,
                    GhiChu = "Đã phân loại, chờ thu gom."
                });
            context.YeuCauThuGoms.Add(request);
        }
        context.SaveChanges();
    }
}
