// Họ và tên: Trần Thị Thảo
// Mã sinh viên: 23103100025
// Nội dung thực hiện: Module 1 – Người dân xem loại rác và khu vực phục vụ

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLThuGomRac_UNETI02_TI17A1HN.Data;
using QLThuGomRac_UNETI02_TI17A1HN.Models.Enums;

namespace QLThuGomRac_UNETI02_TI17A1HN.Controllers;

public class ThongTinThuGomController : Controller
{
    private readonly ApplicationDbContext _context;

    public ThongTinThuGomController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Kiểm tra quyền Người dân
    private IActionResult? KiemTraQuyenNguoiDan()
    {
        var maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
        var vaiTro = HttpContext.Session.GetString("VaiTro");

        if (maTaiKhoan == null)
        {
            return RedirectToAction("Login", "Account");
        }

        if (vaiTro != "NguoiDan")
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }

        return null;
    }

    // Người dân xem các loại rác đang phục vụ
    [HttpGet]
    public async Task<IActionResult> LoaiRac()
    {
        var kiemTra = KiemTraQuyenNguoiDan();

        if (kiemTra != null)
            return kiemTra;

        var danhSach = await _context.LoaiRacs
            .AsNoTracking()
            .Where(x => x.TrangThai == TrangThaiLoaiRac.DangPhucVu)
            .OrderBy(x => x.TenLoaiRac)
            .ToListAsync();

        return View(danhSach);
    }

    // Người dân xem các khu vực đang phục vụ
    [HttpGet]
    public async Task<IActionResult> KhuVuc()
    {
        var kiemTra = KiemTraQuyenNguoiDan();

        if (kiemTra != null)
            return kiemTra;

        var danhSach = await _context.KhuVucs
            .AsNoTracking()
            .Where(x => x.TrangThai == TrangThaiKhuVuc.DangHoatDong)
            .OrderBy(x => x.TenKhuVuc)
            .ToListAsync();

        return View(danhSach);
    }
}
