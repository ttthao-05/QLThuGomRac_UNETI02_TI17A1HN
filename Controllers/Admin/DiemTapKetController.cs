// Họ và tên: Trần Thị Thảo
// Mã sinh viên: 23103100025
// Nội dung thực hiện: Module 1 – Quản lý điểm tập kết

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLThuGomRac_UNETI02_TI17A1HN.Data;
using QLThuGomRac_UNETI02_TI17A1HN.Models.Entities;
using QLThuGomRac_UNETI02_TI17A1HN.Models.Enums;

namespace QLThuGomRac_UNETI02_TI17A1HN.Controllers.Admin;

[AdminOnly]
public class DiemTapKetController : Controller
{
    private readonly ApplicationDbContext _context;

    public DiemTapKetController(ApplicationDbContext context)
    {
        _context = context;
    }

    // DANH SÁCH
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var danhSach = await _context.DiemTapKets
            .AsNoTracking()
            .Include(x => x.KhuVuc)
            .OrderBy(x => x.MaDiemTapKet)
            .ToListAsync();

        return View(danhSach);
    }

    // CHI TIẾT
    [HttpGet]
    public async Task<IActionResult> Details(int? id)
    {
        if (id is null)
            return NotFound();

        var diemTapKet = await _context.DiemTapKets
            .AsNoTracking()
            .Include(x => x.KhuVuc)
            .FirstOrDefaultAsync(x => x.MaDiemTapKet == id);

        if (diemTapKet is null)
            return NotFound();

        return View(diemTapKet);
    }

    // THÊM - GET
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await TaoDanhSachKhuVuc();

        return View();
    }

    // THÊM - POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(DiemTapKet diemTapKet)
    {
        diemTapKet.TenDiemTapKet =
            diemTapKet.TenDiemTapKet?.Trim() ?? string.Empty;

        diemTapKet.DiaChi =
            diemTapKet.DiaChi?.Trim() ?? string.Empty;

        diemTapKet.MoTa =
            diemTapKet.MoTa?.Trim();

        bool tenBiTrung = await _context.DiemTapKets
            .AnyAsync(x =>
                x.TenDiemTapKet == diemTapKet.TenDiemTapKet);

        if (tenBiTrung)
        {
            ModelState.AddModelError(
                nameof(diemTapKet.TenDiemTapKet),
                "Tên điểm tập kết đã tồn tại.");
        }

        bool khuVucTonTai = await _context.KhuVucs
            .AnyAsync(x => x.MaKhuVuc == diemTapKet.MaKhuVuc);

        if (!khuVucTonTai)
        {
            ModelState.AddModelError(
                nameof(diemTapKet.MaKhuVuc),
                "Khu vực không hợp lệ.");
        }

        if (!ModelState.IsValid)
        {
            await TaoDanhSachKhuVuc(diemTapKet.MaKhuVuc);
            return View(diemTapKet);
        }

        _context.DiemTapKets.Add(diemTapKet);
        await _context.SaveChangesAsync();

        TempData["ThongBao"] =
            $"Đã thêm điểm tập kết \"{diemTapKet.TenDiemTapKet}\".";

        return RedirectToAction(nameof(Index));
    }

    // SỬA - GET
    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null)
            return NotFound();

        var diemTapKet = await _context.DiemTapKets
            .FindAsync(id);

        if (diemTapKet is null)
            return NotFound();

        await TaoDanhSachKhuVuc(diemTapKet.MaKhuVuc);

        return View(diemTapKet);
    }

    // SỬA - POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        DiemTapKet diemTapKet)
    {
        if (id != diemTapKet.MaDiemTapKet)
            return NotFound();

        diemTapKet.TenDiemTapKet =
            diemTapKet.TenDiemTapKet?.Trim() ?? string.Empty;

        diemTapKet.DiaChi =
            diemTapKet.DiaChi?.Trim() ?? string.Empty;

        diemTapKet.MoTa =
            diemTapKet.MoTa?.Trim();

        bool tenBiTrung = await _context.DiemTapKets.AnyAsync(x =>
            x.MaDiemTapKet != diemTapKet.MaDiemTapKet &&
            x.TenDiemTapKet == diemTapKet.TenDiemTapKet);

        if (tenBiTrung)
        {
            ModelState.AddModelError(
                nameof(diemTapKet.TenDiemTapKet),
                "Tên điểm tập kết đã tồn tại.");
        }

        bool khuVucTonTai = await _context.KhuVucs
            .AnyAsync(x => x.MaKhuVuc == diemTapKet.MaKhuVuc);

        if (!khuVucTonTai)
        {
            ModelState.AddModelError(
                nameof(diemTapKet.MaKhuVuc),
                "Khu vực không hợp lệ.");
        }

        if (!ModelState.IsValid)
        {
            await TaoDanhSachKhuVuc(diemTapKet.MaKhuVuc);
            return View(diemTapKet);
        }

        _context.Update(diemTapKet);
        await _context.SaveChangesAsync();

        TempData["ThongBao"] =
            $"Đã cập nhật điểm tập kết \"{diemTapKet.TenDiemTapKet}\".";

        return RedirectToAction(nameof(Index));
    }

    // ĐỔI TRẠNG THÁI
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CapNhatTrangThai(int id)
    {
        var diemTapKet = await _context.DiemTapKets
            .FindAsync(id);

        if (diemTapKet is null)
            return NotFound();

        diemTapKet.TrangThai =
            diemTapKet.TrangThai == TrangThaiDiemTapKet.DangHoatDong
                ? TrangThaiDiemTapKet.NgungHoatDong
                : TrangThaiDiemTapKet.DangHoatDong;

        await _context.SaveChangesAsync();

        TempData["ThongBao"] =
            "Đã cập nhật trạng thái điểm tập kết.";

        return RedirectToAction(nameof(Index));
    }

    // XÓA
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var diemTapKet = await _context.DiemTapKets
            .FindAsync(id);

        if (diemTapKet is null)
            return NotFound();

        _context.DiemTapKets.Remove(diemTapKet);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            TempData["ThongBao"] =
                "Điểm tập kết đang có dữ liệu liên quan nên không thể xóa.";

            return RedirectToAction(nameof(Index));
        }

        TempData["ThongBao"] =
            $"Đã xóa điểm tập kết \"{diemTapKet.TenDiemTapKet}\".";

        return RedirectToAction(nameof(Index));
    }

    // TẠO DROPDOWN KHU VỰC
    private async Task TaoDanhSachKhuVuc(
        int? maKhuVuc = null)
    {
        var danhSach = await _context.KhuVucs
            .AsNoTracking()
            .OrderBy(x => x.TenKhuVuc)
            .ToListAsync();

        ViewBag.MaKhuVuc = new SelectList(
            danhSach,
            "MaKhuVuc",
            "TenKhuVuc",
            maKhuVuc);
    }
}
