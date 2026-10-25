// Họ và tên: Trần Thị Thảo
// Mã sinh viên: 23103100025
// Nội dung thực hiện: Bổ sung dữ liệu mẫu phân công và kết quả thu gom để tích hợp, kiểm thử Module 3

using Microsoft.EntityFrameworkCore;
using QLThuGomRac_UNETI02_TI17A1HN.Models.Entities;
using QLThuGomRac_UNETI02_TI17A1HN.Models.Enums;

namespace QLThuGomRac_UNETI02_TI17A1HN.Data.Seed;

public static class Module3SeedData
{
    public static void EnsureSeeded(ApplicationDbContext context)
    {
        // Lấy các yêu cầu mẫu Module 2 đã được xác nhận
        // nhưng chưa có nhân viên phụ trách.
        var yeuCauDaXacNhan = context.YeuCauThuGoms
            .Where(x =>
                x.TrangThai == TrangThaiYeuCau.DaXacNhan &&
                x.GhiChu != null &&
                x.GhiChu.StartsWith("[M2-MAU-") &&
                !context.PhanCongThuGoms.Any(
                    p => p.MaYeuCau == x.MaYeuCau))
            .OrderBy(x => x.MaYeuCau)
            .Take(3)
            .ToList();

        foreach (var yeuCau in yeuCauDaXacNhan)
        {
            // Chọn nhân viên đang hoạt động và đúng khu vực.
            var nhanVien = context.NhanViens
                .FirstOrDefault(x =>
                    x.TrangThai == TrangThaiNhanVien.DangHoatDong &&
                    x.MaKhuVuc == yeuCau.MaKhuVuc);

            if (nhanVien == null)
                continue;

            var phanCong = new PhanCongThuGom
            {
                MaYeuCau = yeuCau.MaYeuCau,
                MaNhanVien = nhanVien.MaNhanVien,
                NgayPhanCong = DateTime.Now.AddDays(-1),
                GhiChu = "Dữ liệu phân công mẫu Module 3.",
                TrangThai = TrangThaiYeuCau.DaPhanCong
            };

            context.PhanCongThuGoms.Add(phanCong);

            // Đồng bộ trạng thái yêu cầu
            yeuCau.TrangThai = TrangThaiYeuCau.DaPhanCong;
        }

        context.SaveChanges();

        // =========================
        // MODULE 3 - Dữ liệu kết quả thu gom mẫu 7.4
        // =========================

        // Lấy 1 phân công mẫu chưa có kết quả
        var phanCongHoanThanh = context.PhanCongThuGoms
            .Include(p => p.YeuCauThuGom)
            .Where(p =>
                p.YeuCauThuGom != null
                && p.YeuCauThuGom.GhiChu != null
                && p.YeuCauThuGom.GhiChu.StartsWith("[M2-MAU-")
                && !context.KetQuaThuGoms.Any(
                    k => k.MaYeuCau == p.MaYeuCau))
            .OrderBy(p => p.MaPhanCong)
            .FirstOrDefault();

        if (phanCongHoanThanh != null)
        {
            var chiTietYeuCaus = context.ChiTietYeuCaus
                .Where(c =>
                    c.MaYeuCau == phanCongHoanThanh.MaYeuCau)
                .ToList();

            if (chiTietYeuCaus.Count > 0)
            {
                var thoiGianHoanThanh = DateTime.Now.AddHours(-2);

                var ketQua = new KetQuaThuGom
                {
                    MaYeuCau = phanCongHoanThanh.MaYeuCau,
                    MaNhanVien = phanCongHoanThanh.MaNhanVien,
                    ThoiGianHoanThanh = thoiGianHoanThanh,
                    GhiChu = "Dữ liệu kết quả thu gom mẫu Module 3.",

                    ChiTietKetQuas = chiTietYeuCaus
                        .Select(c => new ChiTietKetQuaThuGom
                        {
                            MaLoaiRac = c.MaLoaiRac,
                            KhoiLuongThucTe = c.SoLuongDuKien
                        })
                        .ToList()
                };

                context.KetQuaThuGoms.Add(ketQua);

                phanCongHoanThanh.TrangThai =
                    TrangThaiYeuCau.HoanThanh;

                phanCongHoanThanh.NgayBatDau =
                    thoiGianHoanThanh.AddHours(-1);

                phanCongHoanThanh.NgayKetThuc =
                    thoiGianHoanThanh;

                phanCongHoanThanh.YeuCauThuGom!.TrangThai =
                    TrangThaiYeuCau.HoanThanh;
            }
        }

        context.SaveChanges();
    }
}
