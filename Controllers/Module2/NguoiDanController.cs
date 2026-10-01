// Họ và tên: Phạm Văn Hào
// Mã sinh viên: 23103100041
// Nội dung thực hiện: Module 2 – Quản lý người dân (§6.1)

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLThuGomRac_UNETI02_TI17A1HN.Data;
using QLThuGomRac_UNETI02_TI17A1HN.Models.Enums;

namespace QLThuGomRac_UNETI02_TI17A1HN.Controllers.Module2;

[Module2Access(LoaiTaiKhoan.Admin)]
public class NguoiDanController(ApplicationDbContext db) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(string? tuKhoa, TrangThaiNguoiDan? trangThai)
    {
        var query = db.NguoiDans.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(tuKhoa))
        {
            var keyword = tuKhoa.Trim();
            query = query.Where(x => x.HoTen.Contains(keyword) || x.SoDienThoai.Contains(keyword) || (x.Email != null && x.Email.Contains(keyword)));
        }
        if (trangThai.HasValue) query = query.Where(x => x.TrangThai == trangThai);
        ViewBag.TuKhoa = tuKhoa;
        ViewBag.TrangThai = trangThai;
        return View(await query.OrderByDescending(x => x.MaNguoiDan).ToListAsync());
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var person = await db.NguoiDans.AsNoTracking().Include(x => x.TaiKhoan).SingleOrDefaultAsync(x => x.MaNguoiDan == id);
        return person == null ? NotFound() : View(person);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CapNhatTrangThai(int id, TrangThaiNguoiDan trangThai)
    {
        if (!ModelState.IsValid || !Enum.IsDefined(trangThai)) return BadRequest("Trạng thái không hợp lệ.");
        var person = await db.NguoiDans.FindAsync(id);
        if (person == null) return NotFound();
        person.TrangThai = trangThai;
        await db.SaveChangesAsync();
        TempData["ThongBao"] = "Đã cập nhật trạng thái người dân.";
        return RedirectToAction(nameof(Details), new { id });
    }
}
