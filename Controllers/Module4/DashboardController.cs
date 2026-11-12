// Họ và tên: Vũ Đức Hải
// Mã sinh viên: 23103100008
// Nội dung thực hiện: Module 4 - Dashboard tổng quan
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLThuGomRac_UNETI02_TI17A1HN.Data;
using QLThuGomRac_UNETI02_TI17A1HN.Filters;
using QLThuGomRac_UNETI02_TI17A1HN.Models.Enums;
using QLThuGomRac_UNETI02_TI17A1HN.ViewModels;

namespace QLThuGomRac_UNETI02_TI17A1HN.Controllers;

[AdminOnly]
public class DashboardController : Controller
{
    private readonly ApplicationDbContext _db;
    public DashboardController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var dangXuLy = new[] { TrangThaiYeuCau.DaXacNhan, TrangThaiYeuCau.DaPhanCong, TrangThaiYeuCau.DangThuGom };
        var tuChoiHuy = new[] { TrangThaiYeuCau.TuChoi, TrangThaiYeuCau.Huy };
        var dauThang = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-11);

        // Dữ liệu theo tháng: lấy một lần rồi tạo đủ 12 tháng (bao gồm tháng không có yêu cầu).
        var theoThang = await _db.YeuCauThuGoms.AsNoTracking()
            .Where(x => x.NgayDangKy >= dauThang)
            .GroupBy(x => new { x.NgayDangKy.Year, x.NgayDangKy.Month })
            .Select(g => new {
                g.Key.Year,
                g.Key.Month,
                Tong = g.Count(),
                HoanThanh = g.Count(x => x.TrangThai == TrangThaiYeuCau.HoanThanh)
            }).ToListAsync();

        var ganDay = await _db.YeuCauThuGoms.AsNoTracking()
            .OrderByDescending(x => x.NgayDangKy).ThenByDescending(x => x.MaYeuCau)
            .Take(5)
            .Select(x => new DashboardYeuCauVM
            {
                MaYeuCau = x.MaYeuCau,
                NguoiDan = x.NguoiDan.HoTen,
                KhuVuc = x.KhuVuc.TenKhuVuc,
                DiaChi = x.DiaChiThuGom,
                NgayDangKy = x.NgayDangKy,
                NgayMongMuon = x.NgayMongMuonThuGom,
                TrangThai = x.TrangThai.ToString()
            }).ToListAsync();

        var sapToi = await _db.YeuCauThuGoms.AsNoTracking()
            .Where(x => x.NgayMongMuonThuGom >= DateTime.Today
                && x.TrangThai != TrangThaiYeuCau.HoanThanh
                && x.TrangThai != TrangThaiYeuCau.TuChoi
                && x.TrangThai != TrangThaiYeuCau.Huy
                && x.TrangThai != TrangThaiYeuCau.KhongThucHienDuoc)
            .OrderBy(x => x.NgayMongMuonThuGom)
            .Take(5)
            .Select(x => new DashboardYeuCauVM
            {
                MaYeuCau = x.MaYeuCau,
                NguoiDan = x.NguoiDan.HoTen,
                KhuVuc = x.KhuVuc.TenKhuVuc,
                DiaChi = x.DiaChiThuGom,
                NgayDangKy = x.NgayDangKy,
                NgayMongMuon = x.NgayMongMuonThuGom,
                TrangThai = x.TrangThai.ToString()
            }).ToListAsync();

        // Chỉ cộng loại có đơn vị kg: không trộn "cái" với kg.
        var theoLoaiKg = await _db.ChiTietKetQuaThuGoms.AsNoTracking()
            .Where(x => x.LoaiRac != null && x.LoaiRac.DonViTinh.ToLower() == "kg")
            .GroupBy(x => x.LoaiRac!.TenLoaiRac)
            .Select(g => new NhomKhoiLuongVM
            {
                Ten = g.Key,
                KhoiLuong = g.Sum(x => x.KhoiLuongThucTe)
            })
            .OrderByDescending(x => x.KhoiLuong).ToListAsync();

        var vm = new DashboardVM
        {
            TongKhuVuc = await _db.KhuVucs.CountAsync(),
            TongLoaiRac = await _db.LoaiRacs.CountAsync(),
            TongNguoiDan = await _db.NguoiDans.CountAsync(),
            TongNhanVien = await _db.NhanViens.CountAsync(),
            TongDiemTapKet = await _db.DiemTapKets.CountAsync(),
            TongYeuCau = await _db.YeuCauThuGoms.CountAsync(),
            ChoXacNhan = await _db.YeuCauThuGoms.CountAsync(x => x.TrangThai == TrangThaiYeuCau.ChoXacNhan),
            DangXuLy = await _db.YeuCauThuGoms.CountAsync(x => dangXuLy.Contains(x.TrangThai)),
            HoanThanh = await _db.YeuCauThuGoms.CountAsync(x => x.TrangThai == TrangThaiYeuCau.HoanThanh),
            TuChoiHuy = await _db.YeuCauThuGoms.CountAsync(x => tuChoiHuy.Contains(x.TrangThai)),
            KhongThucHienDuoc = await _db.YeuCauThuGoms.CountAsync(x => x.TrangThai == TrangThaiYeuCau.KhongThucHienDuoc),
            KhoiLuongTheoLoaiKg = theoLoaiKg,
            TongKhoiLuong = theoLoaiKg.Sum(x => x.KhoiLuong),
            YeuCauGanDay = ganDay,
            LichSapToi = sapToi
        };

        for (int i = 0; i < 12; i++)
        {
            var thang = dauThang.AddMonths(i);
            var row = theoThang.FirstOrDefault(x => x.Year == thang.Year && x.Month == thang.Month);
            vm.NhanThang.Add(thang.ToString("MM/yyyy"));
            vm.YeuCauTheoThang.Add(row?.Tong ?? 0);
            vm.HoanThanhTheoThang.Add(row?.HoanThanh ?? 0);
        }

        return View(vm);
    }
}
