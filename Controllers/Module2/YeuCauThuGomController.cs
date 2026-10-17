// Họ và tên: Phạm Văn Hào
// Mã sinh viên: 23103100041
// Nội dung thực hiện: Module 2 – Quản lý yêu cầu thu gom (§6.2), đăng ký và tra cứu (§6.3–§6.5)

using System.Data;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QLThuGomRac_UNETI02_TI17A1HN.Data;
using QLThuGomRac_UNETI02_TI17A1HN.Models.Entities;
using QLThuGomRac_UNETI02_TI17A1HN.Models.Enums;
using QLThuGomRac_UNETI02_TI17A1HN.Models.ViewModels.Module2;

namespace QLThuGomRac_UNETI02_TI17A1HN.Controllers.Module2;

[Module2Access(LoaiTaiKhoan.NguoiDan)]
public class YeuCauThuGomController(ApplicationDbContext db) : Controller
{
    private int? AccountId => HttpContext.Session.GetInt32("MaTaiKhoan");
    private IQueryable<YeuCauThuGom> MyRequests => db.YeuCauThuGoms.AsNoTracking().Where(x => x.NguoiDan.MaTaiKhoan == AccountId);

    [HttpGet]
    public async Task<IActionResult> Index(TrangThaiYeuCau? trangThai, string? nhom,
        string? tuKhoa, int? maKhuVuc, DateTime? tuNgay, DateTime? denNgay,
        string? sapXep, int trang = 1)
    {
        // §6.5: danh sách chỉ truy xuất các yêu cầu thuộc tài khoản đang đăng nhập.
        var query = MyRequests.Include(x => x.KhuVuc).AsQueryable();
        if (!string.IsNullOrWhiteSpace(tuKhoa))
        {
            var keyword = tuKhoa.Trim();
            var id = int.TryParse(keyword.TrimStart('#'), out var parsed) ? parsed : 0;
            query = query.Where(x => x.MaYeuCau == id || x.DiaChiThuGom.Contains(keyword)
                || x.SoDienThoaiLienHe.Contains(keyword));
        }
        if (maKhuVuc.HasValue) query = query.Where(x => x.MaKhuVuc == maKhuVuc);
        if (tuNgay.HasValue && denNgay.HasValue && tuNgay.Value.Date > denNgay.Value.Date)
            ModelState.AddModelError("", "Ngày bắt đầu không được sau ngày kết thúc.");
        if (tuNgay.HasValue) query = query.Where(x => x.NgayMongMuonThuGom >= tuNgay.Value.Date);
        if (denNgay.HasValue) query = query.Where(x => x.NgayMongMuonThuGom <= denNgay.Value.Date);
        if (trangThai.HasValue) query = query.Where(x => x.TrangThai == trangThai);
        if (nhom == "dang-xu-ly") query = query.Where(x => x.TrangThai == TrangThaiYeuCau.DaPhanCong || x.TrangThai == TrangThaiYeuCau.DangThuGom);
        if (nhom == "tu-choi-huy") query = query.Where(x => x.TrangThai == TrangThaiYeuCau.TuChoi || x.TrangThai == TrangThaiYeuCau.Huy);
        ViewBag.TrangThai = trangThai;
        ViewBag.Nhom = nhom;
        ViewBag.TuKhoa = tuKhoa;
        ViewBag.MaKhuVuc = maKhuVuc;
        ViewBag.TuNgay = tuNgay?.ToString("yyyy-MM-dd");
        ViewBag.DenNgay = denNgay?.ToString("yyyy-MM-dd");
        ViewBag.SapXep = sapXep;
        // Gồm cả khu vực đã ngừng hoạt động để vẫn tra cứu được lịch sử.
        ViewBag.KhuVucs = await MyRequests.Select(x => new { x.MaKhuVuc, x.KhuVuc.TenKhuVuc })
            .Distinct().OrderBy(x => x.TenKhuVuc)
            .Select(x => new SelectListItem(x.TenKhuVuc, x.MaKhuVuc.ToString())).ToListAsync();
        const int pageSize = 10;
        var total = await query.CountAsync();
        var pages = Math.Max(1, (int)Math.Ceiling(total / (double)pageSize));
        trang = Math.Clamp(trang, 1, pages);
        ViewBag.Trang = trang;
        ViewBag.SoTrang = pages;
        ViewBag.TongSo = total;
        var ordered = sapXep == "ngay-thu-gom"
            ? query.OrderBy(x => x.NgayMongMuonThuGom).ThenByDescending(x => x.MaYeuCau)
            : query.OrderByDescending(x => x.NgayDangKy).ThenByDescending(x => x.MaYeuCau);
        return View(await ordered.Skip((trang - 1) * pageSize).Take(pageSize).ToListAsync());
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var request = await MyRequests.Include(x => x.KhuVuc).Include(x => x.ChiTietYeuCaus).ThenInclude(x => x.LoaiRac).SingleOrDefaultAsync(x => x.MaYeuCau == id);
        return request == null ? NotFound() : View(request);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var person = await db.NguoiDans.AsNoTracking().SingleOrDefaultAsync(x => x.MaTaiKhoan == AccountId);
        if (person == null || person.TrangThai != TrangThaiNguoiDan.DangHoatDong)
            return StatusCode(403, "Hồ sơ người dân không tồn tại hoặc đã bị khóa. Vui lòng liên hệ quản trị viên.");
        var model = new DangKyThuGomViewModel { DiaChiThuGom = person.DiaChi ?? "", SoDienThoaiLienHe = person.SoDienThoai, ChiTiet = [new()] };
        await LoadOptions(model);
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(DangKyThuGomViewModel model)
    {
        // §6.4: mọi điều kiện đăng ký phải hợp lệ trước khi tạo yêu cầu.
        if (!ModelState.IsValid) { await LoadOptions(model); return View(model); }
        // Giữ các trạng thái tài khoản/khu vực/loại rác ổn định đến khi lưu toàn bộ yêu cầu.
        try
        {
            await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            var person = await db.NguoiDans.Include(x => x.TaiKhoan).SingleOrDefaultAsync(x => x.MaTaiKhoan == AccountId);
            if (person == null || person.TrangThai != TrangThaiNguoiDan.DangHoatDong || person.TaiKhoan.TrangThai != TrangThaiTaiKhoan.DangHoatDong || person.TaiKhoan.LoaiTaiKhoan != LoaiTaiKhoan.NguoiDan)
                ModelState.AddModelError("", "Tài khoản/người dân không tồn tại hoặc không còn hoạt động.");
            if (!await db.KhuVucs.AnyAsync(x => x.MaKhuVuc == model.MaKhuVuc && x.TrangThai == TrangThaiKhuVuc.DangHoatDong))
                ModelState.AddModelError(nameof(model.MaKhuVuc), "Khu vực không tồn tại hoặc đã ngừng hoạt động.");
            var ids = model.ChiTiet.Select(x => x.MaLoaiRac).ToList();
            if (await db.LoaiRacs.CountAsync(x =>ids.Contains(x.MaLoaiRac) &&
                x.TrangThai == TrangThaiLoaiRac.DangPhucVu) != ids.Count)
                ModelState.AddModelError(nameof(model.ChiTiet), "Có loại rác không tồn tại hoặc đã ngừng thu gom.");
            var address = string.Join(" ", model.DiaChiThuGom.Trim().Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
            var key = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes($"{person?.MaNguoiDan}|{model.MaKhuVuc}|{address.Normalize().ToUpperInvariant()}|{model.NgayMongMuonThuGom:yyyy-MM-dd}|{model.KhungGio}")));
            if (await db.YeuCauThuGoms.AnyAsync(x => x.KhoaChongTrung == key && x.TrangThai != TrangThaiYeuCau.TuChoi && x.TrangThai != TrangThaiYeuCau.Huy))
                ModelState.AddModelError("", "Bạn đã đăng ký tại địa chỉ, khu vực, ngày và khung giờ này. Vui lòng kiểm tra lịch sử yêu cầu.");
            if (ModelState.IsValid)
            {
                var request = new YeuCauThuGom
                {
                    MaNguoiDan = person!.MaNguoiDan, MaKhuVuc = model.MaKhuVuc,
                    DiaChiThuGom = address, SoDienThoaiLienHe = model.SoDienThoaiLienHe.Trim(),
                    NgayMongMuonThuGom = model.NgayMongMuonThuGom.Date, KhungGio = model.KhungGio,
                    GhiChu = model.GhiChu?.Trim(), KhoaChongTrung = key,
                    ChiTietYeuCaus = model.ChiTiet.Select(x => new ChiTietYeuCau { MaLoaiRac = x.MaLoaiRac, SoLuongDuKien = x.SoLuongDuKien, GhiChu = x.GhiChu?.Trim() }).ToList()
                };
                db.YeuCauThuGoms.Add(request);
                await db.SaveChangesAsync();
                await transaction.CommitAsync();
                TempData["ThongBao"] = "Đăng ký thành công. Yêu cầu đang chờ xác nhận.";
                return RedirectToAction(nameof(Details), new { id = request.MaYeuCau });
            }
            await transaction.RollbackAsync();
        }
        catch (Exception ex) when (IsConcurrentWriteConflict(ex))
        {
            db.ChangeTracker.Clear();
            ModelState.AddModelError("", "Yêu cầu trùng hoặc dữ liệu vừa thay đổi. Vui lòng kiểm tra lịch sử và thử lại.");
        }
        await LoadOptions(model);
        return View(model);
    }

    private static bool IsConcurrentWriteConflict(Exception exception)
    {
        // EF có thể bọc deadlock trong InvalidOperationException rồi DbUpdateException.
        for (Exception? current = exception; current != null; current = current.InnerException)
            if (current is SqlException { Number: 2601 or 2627 or 1205 }) return true;
        return false;
    }

    private async Task LoadOptions(DangKyThuGomViewModel model)
    {
        model.KhuVucItems = await db.KhuVucs.AsNoTracking()
            .Where(x => x.TrangThai == TrangThaiKhuVuc.DangHoatDong)
            .OrderBy(x => x.TenKhuVuc).Select(x => new SelectListItem(
                x.TenKhuVuc, x.MaKhuVuc.ToString()))
            .ToListAsync();
        model.LoaiRacItems = await db.LoaiRacs
            .AsNoTracking()
            .Where(x => x.TrangThai == TrangThaiLoaiRac.DangPhucVu)
            .OrderBy(x => x.TenLoaiRac)
            .Select(x => new SelectListItem(
               x.TenLoaiRac + " (" + x.DonViTinh + ")",
               x.MaLoaiRac.ToString()))
            .ToListAsync();
    }
}
