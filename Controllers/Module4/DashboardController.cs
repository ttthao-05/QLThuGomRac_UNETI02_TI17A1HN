// Họ và tên: Vũ Đức Hải
// Mã sinh viên: 23103100008
// Nội dung thực hiện: Module 4 - Dashboard tổng quan
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLThuGomRac_UNETI02_TI17A1HN.Data;
using QLThuGomRac_UNETI02_TI17A1HN.Filters;
using QLThuGomRac_UNETI02_TI17A1HN.Models.Enums;
using QLThuGomRac_UNETI02_TI17A1HN.ViewModels;

namespace QLThuGomRac_UNETI02_TI17A1HN.Controllers;

[AdminOnly]
public class DashboardController : Controller
{
    private readonly ApplicationDbContext _db;
    public DashboardController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        // "Đang xử lý" = đã xác nhận + đã phân công + đang thu gom
        var dangXuLy = new[] { TrangThaiYeuCau.DaXacNhan, TrangThaiYeuCau.DaPhanCong, TrangThaiYeuCau.DangThuGom };
        var tuChoiHuy = new[] { TrangThaiYeuCau.TuChoi, TrangThaiYeuCau.Huy };

        var vm = new DashboardVM
        {
            TongKhuVuc = await _db.KhuVucs.CountAsync(),
            TongLoaiRac = await _db.LoaiRacs.CountAsync(),
            TongNguoiDan = await _db.NguoiDans.CountAsync(),
            TongNhanVien = await _db.NhanViens.CountAsync(),
            TongYeuCau = await _db.YeuCauThuGoms.CountAsync(),
            ChoXacNhan = await _db.YeuCauThuGoms.CountAsync(x => x.TrangThai == TrangThaiYeuCau.ChoXacNhan),
            DangXuLy = await _db.YeuCauThuGoms.CountAsync(x => dangXuLy.Contains(x.TrangThai)),
            HoanThanh = await _db.YeuCauThuGoms.CountAsync(x => x.TrangThai == TrangThaiYeuCau.HoanThanh),
            TuChoiHuy = await _db.YeuCauThuGoms.CountAsync(x => tuChoiHuy.Contains(x.TrangThai)),
            // (decimal?) để không lỗi khi bảng chưa có dòng nào
            TongKhoiLuong = await _db.ChiTietKetQuaThuGoms.SumAsync(c => (decimal?)c.KhoiLuongThucTe) ?? 0
        };
        return View(vm);
    }
}
