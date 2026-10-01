// Họ và tên: Trần Thị Thảo
// Mã sinh viên: 23103100025
// Nội dung: Module 1 – Quản lý khu vực (§5.5)

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLThuGomRac_UNETI02_TI17A1HN.Data;
using QLThuGomRac_UNETI02_TI17A1HN.Models.Entities;
using QLThuGomRac_UNETI02_TI17A1HN.Models.Enums;

namespace QLThuGomRac_UNETI02_TI17A1HN.Controllers.Admin;

[AdminOnly]
public class KhuVucController : Controller
{
    private readonly ApplicationDbContext _context;

    public KhuVucController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: /KhuVuc
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var danhSach = await _context.KhuVucs
            .AsNoTracking()
            .OrderBy(x => x.MaKhuVuc)
            .ToListAsync();

        return View(danhSach);
    }

    // GET: /KhuVuc/Details/5
    [HttpGet]
    public async Task<IActionResult> Details(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var khuVuc = await _context.KhuVucs
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.MaKhuVuc == id);

        if (khuVuc is null)
        {
            return NotFound();
        }

        return View(khuVuc);
    }

    // GET: /KhuVuc/Create
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    // POST: /KhuVuc/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(KhuVuc khuVuc)
    {
        khuVuc.TenKhuVuc = khuVuc.TenKhuVuc?.Trim() ?? string.Empty;
        khuVuc.DiaChiMoTa = khuVuc.DiaChiMoTa?.Trim();

        bool tenBiTrung = await _context.KhuVucs
            .AnyAsync(x => x.TenKhuVuc == khuVuc.TenKhuVuc);

        if (tenBiTrung)
        {
            ModelState.AddModelError(
                nameof(khuVuc.TenKhuVuc),
                "Tên khu vực đã tồn tại.");
        }

        if (!ModelState.IsValid)
        {
            return View(khuVuc);
        }

        _context.KhuVucs.Add(khuVuc);
        await _context.SaveChangesAsync();

        TempData["ThongBao"] =
            $"Đã thêm khu vực \"{khuVuc.TenKhuVuc}\".";

        return RedirectToAction(nameof(Index));
    }

    // GET: /KhuVuc/Edit/5
    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var khuVuc = await _context.KhuVucs.FindAsync(id);

        if (khuVuc is null)
        {
            return NotFound();
        }

        return View(khuVuc);
    }

    // POST: /KhuVuc/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, KhuVuc khuVuc)
    {
        if (id != khuVuc.MaKhuVuc)
        {
            return NotFound();
        }

        khuVuc.TenKhuVuc = khuVuc.TenKhuVuc?.Trim() ?? string.Empty;
        khuVuc.DiaChiMoTa = khuVuc.DiaChiMoTa?.Trim();

        bool tenBiTrung = await _context.KhuVucs.AnyAsync(x =>
            x.MaKhuVuc != khuVuc.MaKhuVuc &&
            x.TenKhuVuc == khuVuc.TenKhuVuc);

        if (tenBiTrung)
        {
            ModelState.AddModelError(
                nameof(khuVuc.TenKhuVuc),
                "Tên khu vực đã tồn tại.");
        }

        if (!ModelState.IsValid)
        {
            return View(khuVuc);
        }

        _context.Update(khuVuc);
        await _context.SaveChangesAsync();

        TempData["ThongBao"] =
            $"Đã cập nhật khu vực \"{khuVuc.TenKhuVuc}\".";

        return RedirectToAction(nameof(Index));
    }

    // POST: /KhuVuc/CapNhatTrangThai/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CapNhatTrangThai(int id)
    {
        var khuVuc = await _context.KhuVucs.FindAsync(id);

        if (khuVuc is null)
        {
            return NotFound();
        }

        khuVuc.TrangThai =
            khuVuc.TrangThai == TrangThaiKhuVuc.DangHoatDong
                ? TrangThaiKhuVuc.NgungHoatDong
                : TrangThaiKhuVuc.DangHoatDong;

        await _context.SaveChangesAsync();

        TempData["ThongBao"] = "Đã cập nhật trạng thái khu vực.";

        return RedirectToAction(nameof(Index));
    }

    // POST: /KhuVuc/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var khuVuc = await _context.KhuVucs.FindAsync(id);

        if (khuVuc is null)
        {
            return NotFound();
        }

        if (await _context.YeuCauThuGoms.AnyAsync(x => x.MaKhuVuc == id))
        {
            TempData["ThongBao"] = "Khu vực đã có yêu cầu thu gom nên không thể xóa. Bạn có thể ngừng hoạt động khu vực.";
            return RedirectToAction(nameof(Index));
        }

        _context.KhuVucs.Remove(khuVuc);
        try { await _context.SaveChangesAsync(); }
        catch (DbUpdateException ex) when (ex.InnerException is Microsoft.Data.SqlClient.SqlException { Number: 547 })
        {
            TempData["ThongBao"] = "Khu vực vừa phát sinh dữ liệu liên quan nên không thể xóa.";
            return RedirectToAction(nameof(Index));
        }

        TempData["ThongBao"] =
            $"Đã xóa khu vực \"{khuVuc.TenKhuVuc}\".";

        return RedirectToAction(nameof(Index));
    }
}
