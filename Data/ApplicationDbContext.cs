using Microsoft.EntityFrameworkCore;
using QLThuGomRac_UNETI02_TI17A1HN.Models.Entities;

namespace QLThuGomRac_UNETI02_TI17A1HN.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Module 1 – Quản lý tài khoản (§5.1)
    public DbSet<TaiKhoan> TaiKhoans => Set<TaiKhoan>();

<<<<<<< Updated upstream
    // Khai báo DbSet của các Module còn lại sau khi Entity được xây dựng.
=======
    //Module 1 – Quản lý khu vực (§5.5)
    public DbSet<KhuVuc> KhuVucs => Set<KhuVuc>();

    public DbSet<NguoiDan> NguoiDans => Set<NguoiDan>();
    public DbSet<LoaiRac> LoaiRacs => Set<LoaiRac>();
    public DbSet<YeuCauThuGom> YeuCauThuGoms => Set<YeuCauThuGom>();
    public DbSet<ChiTietYeuCau> ChiTietYeuCaus => Set<ChiTietYeuCau>();
>>>>>>> Stashed changes

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<NguoiDan>(e =>
        {
            e.HasIndex(x => x.MaTaiKhoan).IsUnique();
            e.HasOne(x => x.TaiKhoan).WithOne().HasForeignKey<NguoiDan>(x => x.MaTaiKhoan).OnDelete(DeleteBehavior.Restrict);
            e.Property(x => x.NgaySinh).HasColumnType("date");
        });
        modelBuilder.Entity<LoaiRac>().HasIndex(x => x.TenLoaiRac).IsUnique();
        modelBuilder.Entity<YeuCauThuGom>(e =>
        {
            e.HasOne(x => x.NguoiDan).WithMany().HasForeignKey(x => x.MaNguoiDan).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.KhuVuc).WithMany().HasForeignKey(x => x.MaKhuVuc).OnDelete(DeleteBehavior.Restrict);
            e.Property(x => x.NgayMongMuonThuGom).HasColumnType("date");
            e.HasIndex(x => x.KhoaChongTrung).IsUnique().HasFilter("[TrangThai] <> 6 AND [TrangThai] <> 7");
            e.HasIndex(x => new { x.MaNguoiDan, x.TrangThai, x.NgayDangKy });
        });
        modelBuilder.Entity<ChiTietYeuCau>(e =>
        {
            e.HasOne(x => x.YeuCauThuGom).WithMany(x => x.ChiTietYeuCaus).HasForeignKey(x => x.MaYeuCau);
            e.HasOne(x => x.LoaiRac).WithMany().HasForeignKey(x => x.MaLoaiRac).OnDelete(DeleteBehavior.Restrict);
            e.Property(x => x.SoLuongDuKien).HasPrecision(12, 3);
            e.HasIndex(x => new { x.MaYeuCau, x.MaLoaiRac }).IsUnique();
            e.ToTable(t => t.HasCheckConstraint("CK_ChiTietYeuCau_SoLuong", "[SoLuongDuKien] > 0"));
        });

        modelBuilder.Entity<TaiKhoan>(entity =>
        {
            entity.HasKey(x => x.MaTaiKhoan);

            entity.Property(x => x.TenDangNhap).HasMaxLength(50).IsRequired();
            entity.Property(x => x.MatKhau).HasMaxLength(100).IsRequired();
            entity.Property(x => x.HoTen).HasMaxLength(100).IsRequired();
            entity.Property(x => x.SoDienThoai).HasMaxLength(15).IsRequired();

            // Enum lưu kiểu int trong SQL Server.
            entity.Property(x => x.LoaiTaiKhoan).HasConversion<int>();
            entity.Property(x => x.TrangThai).HasConversion<int>();

            // §5.1: Tên đăng nhập không được trùng.
            entity.HasIndex(x => x.TenDangNhap).IsUnique();
        });
    }
}
