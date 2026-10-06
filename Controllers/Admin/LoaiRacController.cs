// Họ và tên: Trần Thị Thảo
// Mã sinh viên: 23103100025
// Nội dung: Module 1 – Quản lý loại rác (5.6)

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLThuGomRac_UNETI02_TI17A1HN.Data;
using QLThuGomRac_UNETI02_TI17A1HN.Models.Entities;
using QLThuGomRac_UNETI02_TI17A1HN.Models.Enums;

namespace QLThuGomRac_UNETI02_TI17A1HN.Controllers.Admin;

[AdminOnly]
public class LoaiRacController : Controller
{
    private readonly ApplicationDbContext _context;

    public LoaiRacController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var danhSach = await _context.LoaiRacs
            .AsNoTracking()
            .OrderBy(x => x.MaLoaiRac)
            .ToListAsync();

        return View(danhSach);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int? id)
    {
        if (id is null)
            return NotFound();

        var loaiRac = await _context.LoaiRacs
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.MaLoaiRac == id);

        if (loaiRac is null)
            return NotFound();

        return View(loaiRac);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(LoaiRac loaiRac)
    {
        loaiRac.TenLoaiRac = loaiRac.TenLoaiRac?.Trim() ?? string.Empty;

        loaiRac.DonViTinh = loaiRac.DonViTinh?.Trim() ?? string.Empty;

        loaiRac.MoTa = loaiRac.MoTa?.Trim();

        bool tenBiTrung = await _context.LoaiRacs
            .AnyAsync(x => x.TenLoaiRac == loaiRac.TenLoaiRac);

        if (tenBiTrung)
        {
            ModelState.AddModelError(
                nameof(loaiRac.TenLoaiRac),"Tên loại rác đã tồn tại.");
        }

        if (!ModelState.IsValid)  return View(loaiRac);

        _context.LoaiRacs.Add(loaiRac);
        await _context.SaveChangesAsync();

        TempData["ThongBao"] = $"Đã thêm loại rác \"{loaiRac.TenLoaiRac}\".";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null)
            return NotFound();

        var loaiRac = await _context.LoaiRacs.FindAsync(id);

        if (loaiRac is null)
            return NotFound();

        return View(loaiRac);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, LoaiRac loaiRac)
    {
        if (id != loaiRac.MaLoaiRac)
            return NotFound();

        loaiRac.TenLoaiRac =
            loaiRac.TenLoaiRac?.Trim() ?? string.Empty;

        loaiRac.DonViTinh =
            loaiRac.DonViTinh?.Trim() ?? string.Empty;

        loaiRac.MoTa = loaiRac.MoTa?.Trim();

        bool tenBiTrung = await _context.LoaiRacs.AnyAsync(x =>
            x.MaLoaiRac != loaiRac.MaLoaiRac &&
            x.TenLoaiRac == loaiRac.TenLoaiRac);

        if (tenBiTrung)
        {
            ModelState.AddModelError(
                nameof(loaiRac.TenLoaiRac),
                "Tên loại rác đã tồn tại.");
        }

        if (!ModelState.IsValid)
            return View(loaiRac);

        _context.Update(loaiRac);
        await _context.SaveChangesAsync();

        TempData["ThongBao"] =
            $"Đã cập nhật loại rác \"{loaiRac.TenLoaiRac}\".";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CapNhatTrangThai(int id)
    {
        var loaiRac = await _context.LoaiRacs.FindAsync(id);

        if (loaiRac is null)
            return NotFound();

        loaiRac.TrangThai =
            loaiRac.TrangThai == TrangThaiLoaiRac.DangPhucVu
                ? TrangThaiLoaiRac.NgungPhucVu
                : TrangThaiLoaiRac.DangPhucVu;

        await _context.SaveChangesAsync();

        TempData["ThongBao"] =
            "Đã cập nhật trạng thái loại rác.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var loaiRac = await _context.LoaiRacs.FindAsync(id);

        if (loaiRac is null)
            return NotFound();

        // Không cho xóa loại rác nếu đã được sử dụng
        // trong chi tiết yêu cầu thu gom.
        bool daDuocSuDung = await _context.ChiTietYeuCaus
            .AnyAsync(x => x.MaLoaiRac == id);

        if (daDuocSuDung)
        {
            TempData["ThongBao"] =
                "Loại rác đã được sử dụng trong yêu cầu thu gom nên không thể xóa. " +
                "Bạn có thể chuyển loại rác sang trạng thái ngừng phục vụ.";

            return RedirectToAction(nameof(Index));
        }

        _context.LoaiRacs.Remove(loaiRac);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
            when (ex.InnerException is Microsoft.Data.SqlClient.SqlException
            {
                Number: 547
            })
        {
            TempData["ThongBao"] =
                "Loại rác vừa phát sinh dữ liệu liên quan nên không thể xóa.";

            return RedirectToAction(nameof(Index));
        }

        TempData["ThongBao"] =
            $"Đã xóa loại rác \"{loaiRac.TenLoaiRac}\".";

        return RedirectToAction(nameof(Index));
    }
}
