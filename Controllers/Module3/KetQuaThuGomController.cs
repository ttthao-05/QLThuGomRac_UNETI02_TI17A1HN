// Họ và tên: Nguyễn Tiến Đạt
// Mã sinh viên: 23103100053
// Nội dung: Module 3 – Kết quả thu gom (7.4)

using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QLThuGomRac_UNETI02_TI17A1HN.Data;
using QLThuGomRac_UNETI02_TI17A1HN.Models.Entities;
using QLThuGomRac_UNETI02_TI17A1HN.Models.Enums;
using QLThuGomRac_UNETI02_TI17A1HN.Models.ViewModels.Module3;

namespace QLThuGomRac_UNETI02_TI17A1HN.Controllers.Module3;

public class KetQuaThuGomController : Controller
{
    private readonly ApplicationDbContext _context;

    public KetQuaThuGomController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Lấy nhân viên từ tài khoản đang đăng nhập.
    private async Task<int?> MaNhanVienAsync()
    {
        int? maTaiKhoan =
            HttpContext.Session.GetInt32("MaTaiKhoan");

        if (maTaiKhoan == null)
            return null;

        return await _context.NhanViens
            .Where(n =>
                n.MaTaiKhoan == maTaiKhoan.Value
                && n.TrangThai == TrangThaiNhanVien.DangHoatDong
                && _context.TaiKhoans.Any(t =>
                    t.MaTaiKhoan == n.MaTaiKhoan
                    && t.LoaiTaiKhoan == LoaiTaiKhoan.NhanVien
                    && t.TrangThai == TrangThaiTaiKhoan.DangHoatDong))
            .Select(n => (int?)n.MaNhanVien)
            .SingleOrDefaultAsync();
    }

    private IActionResult TuChoiTruyCap()
    {
        if (HttpContext.Session.GetInt32("MaTaiKhoan") == null)
            return RedirectToAction("Login", "Account");

        return StatusCode(
            403,
            "Bạn cần tài khoản và hồ sơ nhân viên đang hoạt động.");
    }

    // Cả phân công và yêu cầu phải đang thu gom.
    private static bool DuocNhapKetQua(PhanCongThuGom phanCong)
    {
        return phanCong.TrangThai == TrangThaiYeuCau.DangThuGom
            && phanCong.YeuCauThuGom?.TrangThai
                == TrangThaiYeuCau.DangThuGom;
    }

    // Nạp lại thông tin hiển thị khi biểu mẫu có lỗi.
    private async Task NapFormAsync(
        KetQuaThuGomCreateViewModel model,
        PhanCongThuGom phanCong)
    {
        model.DiaChiThuGom =
            phanCong.YeuCauThuGom!.DiaChiThuGom;

        var maLoaiRacs = phanCong.YeuCauThuGom.ChiTietYeuCaus
            .Select(x => x.MaLoaiRac)
            .ToList();

        var tenLoaiRacs = await _context.LoaiRacs
            .Where(x => maLoaiRacs.Contains(x.MaLoaiRac))
            .ToDictionaryAsync(
                x => x.MaLoaiRac,
                x => x.TenLoaiRac);

        foreach (var dong in model.ChiTiets)
        {
            dong.TenLoaiRac = tenLoaiRacs.GetValueOrDefault(
                dong.MaLoaiRac,
                "Loại rác không hợp lệ");
        }
    }

    // id là mã phân công.
    [HttpGet]
    public async Task<IActionResult> Create(int id)
    {
        var maNhanVien = await MaNhanVienAsync();

        if (maNhanVien == null)
            return TuChoiTruyCap();

        var phanCong = await _context.PhanCongThuGoms
            .AsNoTracking()
            .Include(p => p.YeuCauThuGom)
                .ThenInclude(y => y!.ChiTietYeuCaus)
            .SingleOrDefaultAsync(p =>
                p.MaPhanCong == id
                && p.MaNhanVien == maNhanVien.Value);

        if (phanCong == null)
            return NotFound();

        bool daCoKetQua = await _context.KetQuaThuGoms
            .AnyAsync(k => k.MaYeuCau == phanCong.MaYeuCau);

        if (daCoKetQua)
        {
            return RedirectToAction(
                nameof(Details),
                new { id = phanCong.MaYeuCau });
        }

        if (!DuocNhapKetQua(phanCong))
        {
            TempData["Loi"] =
                "Chỉ nhập kết quả khi công việc đang thu gom.";

            return RedirectToAction(
                "Details",
                "CongViec",
                new { id });
        }

        var model = new KetQuaThuGomCreateViewModel
        {
            MaPhanCong = id,

            ChiTiets = phanCong.YeuCauThuGom!.ChiTietYeuCaus
                .OrderBy(c => c.MaLoaiRac)
                .Select(c => new DongKetQuaViewModel
                {
                    MaLoaiRac = c.MaLoaiRac
                })
                .ToList()
        };

        await NapFormAsync(model, phanCong);

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        KetQuaThuGomCreateViewModel model)
    {
        await using var transaction = await _context.Database
            .BeginTransactionAsync(IsolationLevel.Serializable);

        try
        {
            // Kiểm tra lại quyền ở POST, không chỉ kiểm tra khi mở form.
            var maNhanVien = await MaNhanVienAsync();

            if (maNhanVien == null)
                return TuChoiTruyCap();

            var phanCong = await _context.PhanCongThuGoms
                .Include(p => p.YeuCauThuGom)
                    .ThenInclude(y => y!.ChiTietYeuCaus)
                .SingleOrDefaultAsync(p =>
                    p.MaPhanCong == model.MaPhanCong
                    && p.MaNhanVien == maNhanVien.Value);

            if (phanCong == null)
                return NotFound();

            bool daCoKetQua = await _context.KetQuaThuGoms
                .AnyAsync(k => k.MaYeuCau == phanCong.MaYeuCau);

            if (daCoKetQua)
            {
                return RedirectToAction(
                    nameof(Details),
                    new { id = phanCong.MaYeuCau });
            }

            if (!DuocNhapKetQua(phanCong))
            {
                TempData["Loi"] =
                    "Trạng thái đã thay đổi. Vui lòng tải lại công việc.";

                return RedirectToAction(
                    "Details",
                    "CongViec",
                    new { id = phanCong.MaPhanCong });
            }

            model.ChiTiets ??= new List<DongKetQuaViewModel>();

            // Danh sách gửi lên phải đúng các loại rác của yêu cầu.
            // Chặn thêm loại lạ, bỏ dòng hoặc gửi trùng loại rác.
            var maLoaiRacDuKien = phanCong.YeuCauThuGom!
                .ChiTietYeuCaus
                .Select(c => c.MaLoaiRac)
                .OrderBy(id => id)
                .ToArray();

            var maLoaiRacGuiLen = model.ChiTiets
                .Select(c => c.MaLoaiRac)
                .OrderBy(id => id)
                .ToArray();

            if (maLoaiRacDuKien.Length == 0
                || !maLoaiRacDuKien.SequenceEqual(maLoaiRacGuiLen))
            {
                return BadRequest(
                    "Danh sách loại rác không hợp lệ. "
                    + "Vui lòng mở lại công việc.");
            }

            // Kiểm tra giới hạn decimal(12,3).
            for (int i = 0; i < model.ChiTiets.Count; i++)
            {
                decimal? khoiLuong =
                    model.ChiTiets[i].KhoiLuongThucTe;

                if (khoiLuong.HasValue
                    && (khoiLuong.Value < 0
                        || khoiLuong.Value > 999999999.999m
                        || decimal.Round(khoiLuong.Value, 3)
                            != khoiLuong.Value))
                {
                    ModelState.AddModelError(
                        $"ChiTiets[{i}].KhoiLuongThucTe",
                        "Khối lượng không âm, tối đa 3 chữ số thập phân "
                        + "và không quá 999999999.999 kg.");
                }
            }

            if (!ModelState.IsValid)
            {
                await NapFormAsync(model, phanCong);
                return View(model);
            }

            var thoiGian = DateTime.Now;

            // Mã yêu cầu, mã nhân viên lấy từ database và đăng nhập.
            var ketQua = new KetQuaThuGom
            {
                MaYeuCau = phanCong.MaYeuCau,
                MaNhanVien = maNhanVien.Value,
                ThoiGianHoanThanh = thoiGian,
                GhiChu = model.GhiChu?.Trim(),

                ChiTietKetQuas = model.ChiTiets
                    .Select(c => new ChiTietKetQuaThuGom
                    {
                        MaLoaiRac = c.MaLoaiRac,
                        KhoiLuongThucTe = c.KhoiLuongThucTe!.Value
                    })
                    .ToList()
            };

            _context.KetQuaThuGoms.Add(ketQua);

            // Đồng bộ trạng thái khi lưu kết quả thành công.
            phanCong.TrangThai = TrangThaiYeuCau.HoanThanh;
            phanCong.YeuCauThuGom!.TrangThai =
                TrangThaiYeuCau.HoanThanh;

            phanCong.NgayKetThuc = thoiGian;

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            TempData["ThongBao"] =
                "Đã lưu kết quả và hoàn thành công việc.";

            return RedirectToAction(
                nameof(Details),
                new { id = phanCong.MaYeuCau });
        }
        catch (DbUpdateException ex)
            when (ex.InnerException is SqlException sql
                && (sql.Number == 2601
                    || sql.Number == 2627
                    || sql.Number == 1205))
        {
            await transaction.RollbackAsync();

            TempData["Loi"] =
                "Dữ liệu đã thay đổi hoặc kết quả đã được lưu. "
                + "Vui lòng tải lại công việc.";

            return RedirectToAction("Index", "CongViec");
        }
        catch (SqlException ex) when (ex.Number == 1205)
        {
            await transaction.RollbackAsync();

            TempData["Loi"] =
                "Có thao tác đồng thời. Vui lòng thử lại.";

            return RedirectToAction("Index", "CongViec");
        }
    }

    // id là mã yêu cầu.
    // Admin được xem; nhân viên chỉ xem kết quả của mình.
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        int? maTaiKhoan =
            HttpContext.Session.GetInt32("MaTaiKhoan");

        if (maTaiKhoan == null)
            return TuChoiTruyCap();

        bool laAdmin = await _context.TaiKhoans.AnyAsync(t =>
            t.MaTaiKhoan == maTaiKhoan.Value
            && t.LoaiTaiKhoan == LoaiTaiKhoan.Admin
            && t.TrangThai == TrangThaiTaiKhoan.DangHoatDong);

        int? maNhanVien = laAdmin
            ? null
            : await MaNhanVienAsync();

        if (!laAdmin && maNhanVien == null)
            return TuChoiTruyCap();

        var ketQua = await _context.KetQuaThuGoms
            .AsNoTracking()
            .Include(k => k.NhanVien)
            .Include(k => k.YeuCauThuGom)
            .Include(k => k.ChiTietKetQuas)
                .ThenInclude(c => c.LoaiRac)
            .SingleOrDefaultAsync(k =>
                k.MaYeuCau == id
                && (laAdmin || k.MaNhanVien == maNhanVien));

        if (ketQua == null)
            return NotFound();

        ViewBag.LaAdmin = laAdmin;

        return View(ketQua);
    }
}
