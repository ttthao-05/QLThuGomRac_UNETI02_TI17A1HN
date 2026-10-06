// Họ và tên: Trần Thị Thảo
// Mã sinh viên: 23103100025
// Nội dung thực hiện: Module 1 – Quản lý tài khoản, khu vực và loại rác (§5.1, §5.5, §5.6)

using QLThuGomRac_UNETI02_TI17A1HN.Models.Entities;
using QLThuGomRac_UNETI02_TI17A1HN.Models.Enums;

namespace QLThuGomRac_UNETI02_TI17A1HN.Data.Seed;

public static class SeedData
{
    public static void EnsureSeeded(ApplicationDbContext context)
    {
        // =========================
        // MODULE 1 - Seed tài khoản §5.1
        // =========================
        if (!context.TaiKhoans.Any())
        {
            var danhSachTaiKhoan = new List<TaiKhoan>
            {
                new() { TenDangNhap = "admin", MatKhau = "123456", HoTen = "Quản trị viên", SoDienThoai = "0901000001", LoaiTaiKhoan = LoaiTaiKhoan.Admin, TrangThai = TrangThaiTaiKhoan.DangHoatDong },
                new() { TenDangNhap = "admin2", MatKhau = "123456", HoTen = "Quản trị viên phụ", SoDienThoai = "0901000002", LoaiTaiKhoan = LoaiTaiKhoan.Admin, TrangThai = TrangThaiTaiKhoan.DangHoatDong },

                new() { TenDangNhap = "nv01", MatKhau = "123456", HoTen = "Nhân viên 01", SoDienThoai = "0902000001", LoaiTaiKhoan = LoaiTaiKhoan.NhanVien, TrangThai = TrangThaiTaiKhoan.DangHoatDong },
                new() { TenDangNhap = "nv02", MatKhau = "123456", HoTen = "Nhân viên 02", SoDienThoai = "0902000002", LoaiTaiKhoan = LoaiTaiKhoan.NhanVien, TrangThai = TrangThaiTaiKhoan.DangHoatDong },
                new() { TenDangNhap = "nv03", MatKhau = "123456", HoTen = "Nhân viên 03", SoDienThoai = "0902000003", LoaiTaiKhoan = LoaiTaiKhoan.NhanVien, TrangThai = TrangThaiTaiKhoan.DangHoatDong },
                new() { TenDangNhap = "nv04", MatKhau = "123456", HoTen = "Nhân viên 04", SoDienThoai = "0902000004", LoaiTaiKhoan = LoaiTaiKhoan.NhanVien, TrangThai = TrangThaiTaiKhoan.DangHoatDong },
                new() { TenDangNhap = "nv05", MatKhau = "123456", HoTen = "Nhân viên 05", SoDienThoai = "0902000005", LoaiTaiKhoan = LoaiTaiKhoan.NhanVien, TrangThai = TrangThaiTaiKhoan.DangHoatDong },

                new() { TenDangNhap = "nd01", MatKhau = "123456", HoTen = "Người dân 01", SoDienThoai = "0903000001", LoaiTaiKhoan = LoaiTaiKhoan.NguoiDan, TrangThai = TrangThaiTaiKhoan.DangHoatDong },
                new() { TenDangNhap = "nd02", MatKhau = "123456", HoTen = "Người dân 02", SoDienThoai = "0903000002", LoaiTaiKhoan = LoaiTaiKhoan.NguoiDan, TrangThai = TrangThaiTaiKhoan.DangHoatDong },
                new() { TenDangNhap = "nd03", MatKhau = "123456", HoTen = "Người dân 03", SoDienThoai = "0903000003", LoaiTaiKhoan = LoaiTaiKhoan.NguoiDan, TrangThai = TrangThaiTaiKhoan.DangHoatDong },
                new() { TenDangNhap = "nd04", MatKhau = "123456", HoTen = "Người dân 04", SoDienThoai = "0903000004", LoaiTaiKhoan = LoaiTaiKhoan.NguoiDan, TrangThai = TrangThaiTaiKhoan.DangHoatDong },
                new() { TenDangNhap = "nd05", MatKhau = "123456", HoTen = "Người dân 05", SoDienThoai = "0903000005", LoaiTaiKhoan = LoaiTaiKhoan.NguoiDan, TrangThai = TrangThaiTaiKhoan.DangHoatDong },
                new() { TenDangNhap = "nd06", MatKhau = "123456", HoTen = "Người dân 06", SoDienThoai = "0903000006", LoaiTaiKhoan = LoaiTaiKhoan.NguoiDan, TrangThai = TrangThaiTaiKhoan.DangHoatDong },
                new() { TenDangNhap = "nd07", MatKhau = "123456", HoTen = "Người dân 07", SoDienThoai = "0903000007", LoaiTaiKhoan = LoaiTaiKhoan.NguoiDan, TrangThai = TrangThaiTaiKhoan.DangHoatDong },
                new() { TenDangNhap = "nd08", MatKhau = "123456", HoTen = "Người dân 08", SoDienThoai = "0903000008", LoaiTaiKhoan = LoaiTaiKhoan.NguoiDan, TrangThai = TrangThaiTaiKhoan.DangHoatDong },
                new() { TenDangNhap = "nd09", MatKhau = "123456", HoTen = "Người dân 09", SoDienThoai = "0903000009", LoaiTaiKhoan = LoaiTaiKhoan.NguoiDan, TrangThai = TrangThaiTaiKhoan.DangHoatDong },
                new() { TenDangNhap = "nd10", MatKhau = "123456", HoTen = "Người dân 10", SoDienThoai = "0903000010", LoaiTaiKhoan = LoaiTaiKhoan.NguoiDan, TrangThai = TrangThaiTaiKhoan.DaKhoa }
            };

            context.TaiKhoans.AddRange(danhSachTaiKhoan);
            context.SaveChanges();
        }

        // =========================
        // MODULE 2 - Tạo hồ sơ Người dân từ tài khoản Người dân
        // =========================
        var taiKhoanNguoiDanChuaCoHoSo = context.TaiKhoans
            .Where(x =>
                x.LoaiTaiKhoan == LoaiTaiKhoan.NguoiDan &&
                !context.NguoiDans.Any(n => n.MaTaiKhoan == x.MaTaiKhoan))
            .ToList();

        foreach (var account in taiKhoanNguoiDanChuaCoHoSo)
        {
            context.NguoiDans.Add(new NguoiDan
            {
                MaTaiKhoan = account.MaTaiKhoan,
                HoTen = account.HoTen,
                SoDienThoai = account.SoDienThoai
            });
        }

        context.SaveChanges();

        // =========================
        // MODULE 1 - Seed khu vực §5.5
        // =========================
        if (!context.KhuVucs.Any())
        {
            var danhSachKhuVuc = new List<KhuVuc>
            {
                new()
                {
                    TenKhuVuc = "Khu vực Cầu Giấy",
                    DiaChiMoTa = "Quận Cầu Giấy, Hà Nội",
                    TrangThai = TrangThaiKhuVuc.DangHoatDong
                },

                new()
                {
                    TenKhuVuc = "Khu vực Nam Từ Liêm",
                    DiaChiMoTa = "Quận Nam Từ Liêm, Hà Nội",
                    TrangThai = TrangThaiKhuVuc.DangHoatDong
                },

                new()
                {
                    TenKhuVuc = "Khu vực Bắc Từ Liêm",
                    DiaChiMoTa = "Quận Bắc Từ Liêm, Hà Nội",
                    TrangThai = TrangThaiKhuVuc.DangHoatDong
                }
            };

            context.KhuVucs.AddRange(danhSachKhuVuc);
            context.SaveChanges();
        }

        // =========================
        // MODULE 1 - Seed loại rác §5.6
        // =========================
        if (!context.LoaiRacs.Any())
        {
            var danhSachLoaiRac = new List<LoaiRac>
            {
                new()
                {
                    TenLoaiRac = "Giấy",
                    DonViTinh = "kg",
                    MoTa = "Giấy báo, sách, vở, thùng carton...",
                    TrangThai = TrangThaiLoaiRac.DangPhucVu
                },

                new()
                {
                    TenLoaiRac = "Nhựa",
                    DonViTinh = "kg",
                    MoTa = "Chai nhựa, hộp nhựa và các sản phẩm nhựa có thể tái chế.",
                    TrangThai = TrangThaiLoaiRac.DangPhucVu
                },

                new()
                {
                    TenLoaiRac = "Kim loại",
                    DonViTinh = "kg",
                    MoTa = "Sắt, thép, nhôm và các loại kim loại tái chế.",
                    TrangThai = TrangThaiLoaiRac.DangPhucVu
                },

                new()
                {
                    TenLoaiRac = "Chai/lon",
                    DonViTinh = "kg",
                    MoTa = "Chai thủy tinh, lon nước và các loại bao bì tương tự.",
                    TrangThai = TrangThaiLoaiRac.DangPhucVu
                },

                new()
                {
                    TenLoaiRac = "Thiết bị điện tử nhỏ",
                    DonViTinh = "cái",
                    MoTa = "Điện thoại cũ, phụ kiện điện tử và thiết bị điện tử kích thước nhỏ.",
                    TrangThai = TrangThaiLoaiRac.DangPhucVu
                }
            };

            context.LoaiRacs.AddRange(danhSachLoaiRac);
            context.SaveChanges();
        }
    }
}
