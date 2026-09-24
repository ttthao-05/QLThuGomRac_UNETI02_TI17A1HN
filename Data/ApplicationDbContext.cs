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

    // Khai báo DbSet của các Module còn lại sau khi Entity được xây dựng.

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

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
