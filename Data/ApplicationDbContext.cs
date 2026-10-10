// Họ và tên: Trần Thị Thảo
// Mã sinh viên: 23103100025
// Nội dung thực hiện: Module 1 – Cấu hình TaiKhoan, KhuVuc, LoaiRac trong DbContext

// Họ và tên: Phạm Văn Hào
// Mã sinh viên: 23103100041
// Nội dung thực hiện: Module 2 – Cấu hình NguoiDan, YeuCauThuGom, ChiTietYeuCau trong DbContext

using Microsoft.EntityFrameworkCore;
using QLThuGomRac_UNETI02_TI17A1HN.Models.Entities;

namespace QLThuGomRac_UNETI02_TI17A1HN.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Module 1 – Quản lý tài khoản (5.1)
    public DbSet<TaiKhoan> TaiKhoans => Set<TaiKhoan>();

    // Module 1 – Quản lý khu vực (5.5)
    public DbSet<KhuVuc> KhuVucs => Set<KhuVuc>();

    // Module 1 – Quản lý loại rác (5.6)
    public DbSet<LoaiRac> LoaiRacs => Set<LoaiRac>();

    // Module 2 – Quản lý người dân
    public DbSet<NguoiDan> NguoiDans => Set<NguoiDan>();

    // Module 2 – Quản lý yêu cầu thu gom
    public DbSet<YeuCauThuGom> YeuCauThuGoms => Set<YeuCauThuGom>();

    // Module 2 – Chi tiết yêu cầu
    public DbSet<ChiTietYeuCau> ChiTietYeuCaus => Set<ChiTietYeuCau>();
    // Họ và tên: Nguyễn Tiến Đạt
    // Mã sinh viên: 23103100053
    // Nội dung thực hiện: Module 3 – Quản lý nhân viên
    public DbSet<NhanVien> NhanViens => Set<NhanVien>();

    // Module 3 – Phân công và xử lý yêu cầu (7.2–7.3)
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // =========================
        // MODULE 1 - TÀI KHOẢN
        // =========================
        modelBuilder.Entity<TaiKhoan>(entity =>
        {
            entity.HasKey(x => x.MaTaiKhoan);

            entity.Property(x => x.TenDangNhap)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(x => x.MatKhau)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.HoTen)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.SoDienThoai)
                .HasMaxLength(15)
                .IsRequired();

            entity.Property(x => x.LoaiTaiKhoan)
                .HasConversion<int>();

            entity.Property(x => x.TrangThai)
                .HasConversion<int>();

            // 5.1: Tên đăng nhập không được trùng.
            entity.HasIndex(x => x.TenDangNhap)
                .IsUnique();
        });

        // =========================
        // MODULE 1 - KHU VỰC
        // =========================
        modelBuilder.Entity<KhuVuc>(entity =>
        {
            entity.HasKey(x => x.MaKhuVuc);

            entity.Property(x => x.TenKhuVuc)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.DiaChiMoTa)
                .HasMaxLength(255);

            entity.Property(x => x.TrangThai)
                .HasConversion<int>();

            // 5.5: Tên khu vực không được trùng.
            entity.HasIndex(x => x.TenKhuVuc)
                .IsUnique();
        });

        // =========================
        // MODULE 1 - LOẠI RÁC
        // =========================
        modelBuilder.Entity<LoaiRac>(entity =>
        {
            entity.HasKey(x => x.MaLoaiRac);

            entity.Property(x => x.TenLoaiRac)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.DonViTinh)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(x => x.MoTa)
                .HasMaxLength(500);

            entity.Property(x => x.TrangThai)
                .HasConversion<int>();

            // 5.6: Tên loại rác không được trùng.
            entity.HasIndex(x => x.TenLoaiRac)
                .IsUnique();
        });

        // =========================
        // MODULE 2 - NGƯỜI DÂN
        // =========================
        modelBuilder.Entity<NguoiDan>(entity =>
        {
            entity.HasIndex(x => x.MaTaiKhoan)
                .IsUnique();

            entity.HasOne(x => x.TaiKhoan)
                .WithOne()
                .HasForeignKey<NguoiDan>(x => x.MaTaiKhoan)
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(x => x.NgaySinh)
                .HasColumnType("date");
        });

        // =========================
        // MODULE 2 - YÊU CẦU THU GOM
        // =========================
        modelBuilder.Entity<YeuCauThuGom>(entity =>
        {
            entity.HasOne(x => x.NguoiDan)
                .WithMany()
                .HasForeignKey(x => x.MaNguoiDan)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.KhuVuc)
                .WithMany()
                .HasForeignKey(x => x.MaKhuVuc)
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(x => x.NgayMongMuonThuGom)
                .HasColumnType("date");

            entity.HasIndex(x => x.KhoaChongTrung)
                .IsUnique()
                .HasFilter("[TrangThai] <> 6 AND [TrangThai] <> 7");

            entity.HasIndex(x => new
            {
                x.MaNguoiDan,
                x.TrangThai,
                x.NgayDangKy
            });
        });

        // =========================
        // MODULE 2 - CHI TIẾT YÊU CẦU
        // =========================
        modelBuilder.Entity<ChiTietYeuCau>(entity =>
        {
            entity.HasOne(x => x.YeuCauThuGom)
                .WithMany(x => x.ChiTietYeuCaus)
                .HasForeignKey(x => x.MaYeuCau);

            entity.HasOne(x => x.LoaiRac)
                .WithMany()
                .HasForeignKey(x => x.MaLoaiRac)
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(x => x.SoLuongDuKien)
                .HasPrecision(12, 3);

            entity.HasIndex(x => new
            {
                x.MaYeuCau,
                x.MaLoaiRac
            })
            .IsUnique();

            entity.ToTable(t =>
                t.HasCheckConstraint(
                    "CK_ChiTietYeuCau_SoLuong",
                    "[SoLuongDuKien] > 0"));
        });
        // Họ và tên: Nguyễn Tiến Đạt
        // Mã sinh viên: 23103100053
        // Nội dung thực hiện: Module 3 – Cấu hình nhân viên
        modelBuilder.Entity<NhanVien>(entity =>
        {
            entity.HasKey(x => x.MaNhanVien);

            entity.Property(x => x.NgayVaoLam)
                  .HasColumnType("date");
            entity.Property(x => x.HoTen)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.SoDienThoai)
                .HasMaxLength(15)
                .IsRequired();

            entity.Property(x => x.TrangThai)
                .HasConversion<int>();

            entity.HasIndex(x => x.MaTaiKhoan)
                .IsUnique();

            entity.HasOne(x => x.TaiKhoan)
                .WithOne()
                .HasForeignKey<NhanVien>(x => x.MaTaiKhoan)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.KhuVuc)
                .WithMany()
                .HasForeignKey(x => x.MaKhuVuc)
                .OnDelete(DeleteBehavior.Restrict);
        });
      
    }
}
