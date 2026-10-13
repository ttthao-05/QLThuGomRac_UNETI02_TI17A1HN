// Họ và tên: [Vũ Đức Hải]
// Mã sinh viên: [23103100008]
// Nội dung thực hiện: Module 4 - Thống kê số lượng yêu cầu bằng LINQ
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLThuGomRac_UNETI02_TI17A1HN.Data;
using QLThuGomRac_UNETI02_TI17A1HN.Filters;
using QLThuGomRac_UNETI02_TI17A1HN.Helpers;
using QLThuGomRac_UNETI02_TI17A1HN.ViewModels;

namespace QLThuGomRac_UNETI02_TI17A1HN.Controllers;

[AdminOnly]
public class ThongKeController : Controller
{
    private readonly ApplicationDbContext _db;
    public ThongKeController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var vm = new ThongKeVM();

        //  Số yêu cầu theo trạng thái (nhóm theo enum, đổi sang tên hiển thị sau khi lấy dữ liệu)
        var theoTrangThai = await _db.YeuCauThuGoms
            .GroupBy(x => x.TrangThai)
            .Select(g => new { g.Key, SoLuong = g.Count() })
            .ToListAsync();
        vm.TheoTrangThai = theoTrangThai
            .Select(x => new NhomSoLuongVM { Ten = x.Key.TenHienThi(), SoLuong = x.SoLuong })
            .OrderByDescending(x => x.SoLuong).ToList();

        //  Số yêu cầu theo khu vực
        vm.TheoKhuVuc = await _db.YeuCauThuGoms
            .GroupBy(x => x.KhuVuc.TenKhuVuc)
            .Select(g => new NhomSoLuongVM { Ten = g.Key, SoLuong = g.Count() })
            .OrderByDescending(x => x.SoLuong).ToListAsync();

        //  Số yêu cầu theo tháng đăng ký
        var theoThang = await _db.YeuCauThuGoms
            .GroupBy(x => new { x.NgayDangKy.Year, x.NgayDangKy.Month })
            .Select(g => new { g.Key.Year, g.Key.Month, SoLuong = g.Count() })
            .OrderBy(x => x.Year).ThenBy(x => x.Month).ToListAsync();
        vm.TheoThang = theoThang
            .Select(x => new NhomSoLuongVM { Ten = $"{x.Month:00}/{x.Year}", SoLuong = x.SoLuong }).ToList();

        //  Số yêu cầu theo nhân viên (qua bảng phân công)
        vm.TheoNhanVien = await _db.PhanCongThuGoms
            .GroupBy(p => p.NhanVien!.HoTen)
            .Select(g => new NhomSoLuongVM { Ten = g.Key, SoLuong = g.Count() })
            .OrderByDescending(x => x.SoLuong).ToListAsync();

        return View(vm);
    }
}
