// Họ và tên: Phạm Văn Hào
// Mã sinh viên: 23103100041
// Nội dung thực hiện: Module 2 – Thông tin cá nhân (§6.1)

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLThuGomRac_UNETI02_TI17A1HN.Data;
using QLThuGomRac_UNETI02_TI17A1HN.Models.Enums;
using QLThuGomRac_UNETI02_TI17A1HN.Models.ViewModels.Module2;

namespace QLThuGomRac_UNETI02_TI17A1HN.Controllers.Module2;

[Module2Access(LoaiTaiKhoan.NguoiDan)]
public class HoSoController(ApplicationDbContext db) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var person = await db.NguoiDans.AsNoTracking().SingleOrDefaultAsync(x => x.MaTaiKhoan == HttpContext.Session.GetInt32("MaTaiKhoan"));
        if (person == null) return NotFound("Chưa có hồ sơ người dân. Vui lòng liên hệ quản trị viên.");
        ViewBag.TrangThai = person.TrangThai.GetDisplayName();
        return View(new HoSoViewModel { HoTen = person.HoTen, NgaySinh = person.NgaySinh, GioiTinh = person.GioiTinh, SoDienThoai = person.SoDienThoai, Email = person.Email, DiaChi = person.DiaChi });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(HoSoViewModel model)
    {
        var person = await db.NguoiDans.Include(x => x.TaiKhoan).SingleOrDefaultAsync(x => x.MaTaiKhoan == HttpContext.Session.GetInt32("MaTaiKhoan"));
        if (person == null) return NotFound();
        if (person.TrangThai != TrangThaiNguoiDan.DangHoatDong) return StatusCode(403, "Hồ sơ đã bị khóa. Vui lòng liên hệ quản trị viên.");
        ViewBag.TrangThai = person.TrangThai.GetDisplayName();
        if (!ModelState.IsValid) return View(model);
        person.HoTen = model.HoTen.Trim();
        person.NgaySinh = model.NgaySinh?.Date;
        person.GioiTinh = model.GioiTinh;
        person.SoDienThoai = model.SoDienThoai.Trim();
        person.Email = model.Email?.Trim();
        person.DiaChi = model.DiaChi?.Trim();
        person.TaiKhoan.HoTen = person.HoTen;
        person.TaiKhoan.SoDienThoai = person.SoDienThoai;
        await db.SaveChangesAsync();
        HttpContext.Session.SetString("HoTen", person.HoTen);
        TempData["ThongBao"] = "Đã cập nhật thông tin cá nhân.";
        return RedirectToAction(nameof(Index));
    }
}
