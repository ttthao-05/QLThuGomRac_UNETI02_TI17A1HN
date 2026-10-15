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

    // Lấy nhân viên đang đăng nhập từ database.
    private async Task<int?> MaNhanVienAsync()
    {
        int? maTaiKhoan =
            HttpContext.Session.GetInt32("MaTaiKhoan");

        if (!maTaiKhoan.HasValue)
            return null;

        return await _context.NhanViens
            .Where(n =>
                n.MaTaiKhoan == maTaiKhoan.Value
                && n.TrangThai == TrangThaiNhanVien.DangHoatDong
                && _context.TaiKhoans.Any(t =>
                    t.MaTaiKhoan == n.MaTaiKhoan
                    && t.LoaiTaiKhoan == LoaiTaiKhoan.NhanVien
                    && t.TrangThai ==
                        TrangThaiTaiKhoan.DangHoatDong))
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

    private IQueryable<PhanCongThuGom> TruyVanCongViec()
    {
        return _context.PhanCongThuGoms
            .Include(p => p.YeuCauThuGom)
                .ThenInclude(y => y!.KhuVuc)
            .Include(p => p.YeuCauThuGom)
                .ThenInclude(y => y!.ChiTietYeuCaus)
                .ThenInclude(c => c.LoaiRac);
    }

    // Chỉ ghi nhận kết quả khi công việc đang thu gom.
    private static bool DangThuGom(PhanCongThuGom congViec)
    {
        return congViec.TrangThai == TrangThaiYeuCau.DangThuGom
            && congViec.YeuCauThuGom?.TrangThai ==
                TrangThaiYeuCau.DangThuGom;
    }

    private static void NapThongTinHienThi(
        KetQuaThuGomCreateViewModel model,
        PhanCongThuGom congViec)
    {
        var yeuCau = congViec.YeuCauThuGom!;

        model.DiaChiThuGom = yeuCau.DiaChiThuGom;
        model.TenKhuVuc = yeuCau.KhuVuc.TenKhuVuc;

        foreach (var dong in model.ChiTiets)
        {
            var chiTiet = yeuCau.ChiTietYeuCaus
                .FirstOrDefault(c => c.MaLoaiRac == dong.MaLoaiRac);

            dong.TenLoaiRac =
                chiTiet?.LoaiRac.TenLoaiRac ?? "Loại rác không hợp lệ";
        }
    }

    // Mở form nhập kết quả.
    [HttpGet]
    public async Task<IActionResult> Create(int id)
    {
        var maNhanVien = await MaNhanVienAsync();

        if (maNhanVien == null)
            return TuChoiTruyCap();

        var congViec = await TruyVanCongViec()
            .AsNoTracking()
            .SingleOrDefaultAsync(p =>
                p.MaPhanCong == id
                && p.MaNhanVien == maNhanVien.Value);

        if (congViec == null)
            return NotFound();

        bool daCoKetQua = await _context.KetQuaThuGoms
            .AnyAsync(k => k.MaPhanCong == id);

        if (!DangThuGom(congViec) || daCoKetQua)
        {
            TempData["Loi"] =
                "Công việc phải đang thu gom và chưa có kết quả.";

            return RedirectToAction(
                "Details", "CongViec", new { id });
        }

        var model = new KetQuaThuGomCreateViewModel
        {
            MaPhanCong = id,
            ChiTiets = congViec.YeuCauThuGom!.ChiTietYeuCaus
                .OrderBy(c => c.MaLoaiRac)
                .Select(c => new ChiTietKetQuaInputViewModel
                {
                    MaLoaiRac = c.MaLoaiRac
                })
                .ToList()
        };

        NapThongTinHienThi(model, congViec);

        return View(model);
    }

    // Lưu kết quả và hoàn thành công việc trong cùng transaction.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        KetQuaThuGomCreateViewModel model)
    {
        await using var transaction = await _context.Database
            .BeginTransactionAsync(IsolationLevel.Serializable);

        try
        {
            var maNhanVien = await MaNhanVienAsync();

            if (maNhanVien == null)
                return TuChoiTruyCap();

            var congViec = await TruyVanCongViec()
                .SingleOrDefaultAsync(p =>
                    p.MaPhanCong == model.MaPhanCong
                    && p.MaNhanVien == maNhanVien.Value);

            if (congViec == null)
                return NotFound();

            bool daCoKetQua = await _context.KetQuaThuGoms
                .AnyAsync(k => k.MaPhanCong == model.MaPhanCong);

            if (!DangThuGom(congViec) || daCoKetQua)
            {
                TempData["Loi"] =
                    "Trạng thái đã thay đổi hoặc đã có kết quả.";

                return RedirectToAction(
                    "Details",
                    "CongViec",
                    new { id = model.MaPhanCong });
            }

            model.ChiTiets ??= new();

            var maLoaiRacHopLe = congViec.YeuCauThuGom!
                .ChiTietYeuCaus
                .Select(c => c.MaLoaiRac)
                .ToHashSet();

            var maLoaiRacGuiLen = model.ChiTiets
                .Select(c => c.MaLoaiRac)
                .ToList();

            // Chặn thiếu dòng, trùng dòng hoặc gửi loại rác khác.
            if (maLoaiRacHopLe.Count == 0
                || maLoaiRacGuiLen.Count != maLoaiRacHopLe.Count
                || maLoaiRacGuiLen.Distinct().Count()
                    != maLoaiRacGuiLen.Count
                || !maLoaiRacHopLe.SetEquals(maLoaiRacGuiLen))
            {
                return BadRequest(
                    "Danh sách loại rác không hợp lệ. "
                    + "Vui lòng mở lại form từ công việc.");
            }

            for (int i = 0; i < model.ChiTiets.Count; i++)
            {
                var khoiLuong = model.ChiTiets[i].KhoiLuongThucTe;

                if (khoiLuong.HasValue
                    && decimal.Round(khoiLuong.Value, 3)
                        != khoiLuong.Value)
                {
                    ModelState.AddModelError(
                        $"ChiTiets[{i}].KhoiLuongThucTe",
                        "Chỉ nhập tối đa 3 chữ số sau dấu thập phân.");
                }
            }

            if (!model.ChiTiets.Any(c => c.KhoiLuongThucTe > 0))
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Phải có ít nhất một loại rác có khối lượng lớn hơn 0.");
            }

            if (!ModelState.IsValid)
            {
                NapThongTinHienThi(model, congViec);
                return View(model);
            }

            var thoiDiem = DateTime.Now;

            var ketQua = new KetQuaThuGom
            {
                MaPhanCong = congViec.MaPhanCong,
                NgayGhiNhan = thoiDiem,
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

            congViec.TrangThai = TrangThaiYeuCau.HoanThanh;
            congViec.NgayKetThuc = thoiDiem;

            congViec.YeuCauThuGom!.TrangThai =
                TrangThaiYeuCau.HoanThanh;

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            TempData["ThongBao"] =
                "Đã lưu kết quả và hoàn thành thu gom.";

            return RedirectToAction(
                "Details",
                "CongViec",
                new { id = model.MaPhanCong });
        }
        catch (DbUpdateException ex)
            when (ex.InnerException is SqlException sql
                && (sql.Number == 2601
                    || sql.Number == 2627
                    || sql.Number == 1205
                    || sql.Number == 547))
        {
            await transaction.RollbackAsync();

            TempData["Loi"] =
                "Dữ liệu đã thay đổi. Vui lòng tải lại công việc.";

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
}
