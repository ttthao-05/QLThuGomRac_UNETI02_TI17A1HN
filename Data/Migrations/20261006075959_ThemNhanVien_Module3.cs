// Họ và tên: Nguyễn Tiến Đạt
// Mã sinh viên: 23103100053
// Nội dung: Module 3 – Tạo hoặc hoàn thiện bảng nhân viên

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLThuGomRac_UNETI02_TI17A1HN.Data.Migrations
{
    public partial class ThemNhanVien_Module3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'dbo.NhanViens', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.NhanViens
    (
        MaNhanVien int IDENTITY(1,1) NOT NULL,
        MaTaiKhoan int NOT NULL,
        HoTen nvarchar(100) NOT NULL,
        SoDienThoai nvarchar(15) NOT NULL,
        MaKhuVuc int NOT NULL,
        NgayVaoLam date NOT NULL,
        TrangThai int NOT NULL,
        CONSTRAINT PK_NhanViens PRIMARY KEY (MaNhanVien)
    );
END;

IF NOT EXISTS
(
    SELECT 1 FROM sys.indexes
    WHERE object_id = OBJECT_ID(N'dbo.NhanViens')
      AND name = N'IX_NhanViens_MaTaiKhoan'
)
BEGIN
    CREATE UNIQUE INDEX IX_NhanViens_MaTaiKhoan
    ON dbo.NhanViens(MaTaiKhoan);
END;

IF NOT EXISTS
(
    SELECT 1 FROM sys.indexes
    WHERE object_id = OBJECT_ID(N'dbo.NhanViens')
      AND name = N'IX_NhanViens_MaKhuVuc'
)
BEGIN
    CREATE INDEX IX_NhanViens_MaKhuVuc
    ON dbo.NhanViens(MaKhuVuc);
END;

IF NOT EXISTS
(
    SELECT 1 FROM sys.foreign_keys
    WHERE parent_object_id = OBJECT_ID(N'dbo.NhanViens')
      AND name = N'FK_NhanViens_TaiKhoans_MaTaiKhoan'
)
BEGIN
    ALTER TABLE dbo.NhanViens WITH CHECK
    ADD CONSTRAINT FK_NhanViens_TaiKhoans_MaTaiKhoan
    FOREIGN KEY (MaTaiKhoan)
    REFERENCES dbo.TaiKhoans(MaTaiKhoan)
    ON DELETE NO ACTION;
END;

IF NOT EXISTS
(
    SELECT 1 FROM sys.foreign_keys
    WHERE parent_object_id = OBJECT_ID(N'dbo.NhanViens')
      AND name = N'FK_NhanViens_KhuVucs_MaKhuVuc'
)
BEGIN
    ALTER TABLE dbo.NhanViens WITH CHECK
    ADD CONSTRAINT FK_NhanViens_KhuVucs_MaKhuVuc
    FOREIGN KEY (MaKhuVuc)
    REFERENCES dbo.KhuVucs(MaKhuVuc)
    ON DELETE NO ACTION;
END;
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Bảng có thể tồn tại trước migration này.
            // Chặn rollback để tránh xóa dữ liệu nhân viên đã có.
            throw new NotSupportedException(
                "Migration này tiếp nhận bảng NhanViens có sẵn. " +
                "Cần kiểm tra database trước khi rollback.");
        }
    }
}
