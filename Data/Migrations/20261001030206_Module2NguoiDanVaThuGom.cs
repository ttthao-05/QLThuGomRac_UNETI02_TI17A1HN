// Họ và tên: Phạm Văn Hào
// Mã sinh viên: 23103100041
// Nội dung thực hiện: Module 2 – Migration Module 2 (§6.1–§6.4)

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLThuGomRac_UNETI02_TI17A1HN.Data.Migrations
{
    /// <inheritdoc />
    public partial class Module2NguoiDanVaThuGom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LoaiRacs",
                columns: table => new
                {
                    MaLoaiRac = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenLoaiRac = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DonViTinh = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DangThuGom = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoaiRacs", x => x.MaLoaiRac);
                });

            migrationBuilder.CreateTable(
                name: "NguoiDans",
                columns: table => new
                {
                    MaNguoiDan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaTaiKhoan = table.Column<int>(type: "int", nullable: false),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NgaySinh = table.Column<DateTime>(type: "date", nullable: true),
                    GioiTinh = table.Column<int>(type: "int", nullable: true),
                    SoDienThoai = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(254)", maxLength: 254, nullable: true),
                    DiaChi = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    NgayDangKy = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThai = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NguoiDans", x => x.MaNguoiDan);
                    table.ForeignKey(
                        name: "FK_NguoiDans_TaiKhoans_MaTaiKhoan",
                        column: x => x.MaTaiKhoan,
                        principalTable: "TaiKhoans",
                        principalColumn: "MaTaiKhoan",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "YeuCauThuGoms",
                columns: table => new
                {
                    MaYeuCau = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaNguoiDan = table.Column<int>(type: "int", nullable: false),
                    MaKhuVuc = table.Column<int>(type: "int", nullable: false),
                    DiaChiThuGom = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    SoDienThoaiLienHe = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    NgayDangKy = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayMongMuonThuGom = table.Column<DateTime>(type: "date", nullable: false),
                    KhungGio = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    TrangThai = table.Column<int>(type: "int", nullable: false),
                    KhoaChongTrung = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YeuCauThuGoms", x => x.MaYeuCau);
                    table.ForeignKey(
                        name: "FK_YeuCauThuGoms_KhuVucs_MaKhuVuc",
                        column: x => x.MaKhuVuc,
                        principalTable: "KhuVucs",
                        principalColumn: "MaKhuVuc",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_YeuCauThuGoms_NguoiDans_MaNguoiDan",
                        column: x => x.MaNguoiDan,
                        principalTable: "NguoiDans",
                        principalColumn: "MaNguoiDan",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChiTietYeuCaus",
                columns: table => new
                {
                    MaChiTiet = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaYeuCau = table.Column<int>(type: "int", nullable: false),
                    MaLoaiRac = table.Column<int>(type: "int", nullable: false),
                    SoLuongDuKien = table.Column<decimal>(type: "decimal(12,3)", precision: 12, scale: 3, nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietYeuCaus", x => x.MaChiTiet);
                    table.CheckConstraint("CK_ChiTietYeuCau_SoLuong", "[SoLuongDuKien] > 0");
                    table.ForeignKey(
                        name: "FK_ChiTietYeuCaus_LoaiRacs_MaLoaiRac",
                        column: x => x.MaLoaiRac,
                        principalTable: "LoaiRacs",
                        principalColumn: "MaLoaiRac",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChiTietYeuCaus_YeuCauThuGoms_MaYeuCau",
                        column: x => x.MaYeuCau,
                        principalTable: "YeuCauThuGoms",
                        principalColumn: "MaYeuCau",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietYeuCaus_MaLoaiRac",
                table: "ChiTietYeuCaus",
                column: "MaLoaiRac");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietYeuCaus_MaYeuCau_MaLoaiRac",
                table: "ChiTietYeuCaus",
                columns: new[] { "MaYeuCau", "MaLoaiRac" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LoaiRacs_TenLoaiRac",
                table: "LoaiRacs",
                column: "TenLoaiRac",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NguoiDans_MaTaiKhoan",
                table: "NguoiDans",
                column: "MaTaiKhoan",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_YeuCauThuGoms_KhoaChongTrung",
                table: "YeuCauThuGoms",
                column: "KhoaChongTrung",
                unique: true,
                filter: "[TrangThai] <> 6 AND [TrangThai] <> 7");

            migrationBuilder.CreateIndex(
                name: "IX_YeuCauThuGoms_MaKhuVuc",
                table: "YeuCauThuGoms",
                column: "MaKhuVuc");

            migrationBuilder.CreateIndex(
                name: "IX_YeuCauThuGoms_MaNguoiDan_TrangThai_NgayDangKy",
                table: "YeuCauThuGoms",
                columns: new[] { "MaNguoiDan", "TrangThai", "NgayDangKy" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChiTietYeuCaus");

            migrationBuilder.DropTable(
                name: "LoaiRacs");

            migrationBuilder.DropTable(
                name: "YeuCauThuGoms");

            migrationBuilder.DropTable(
                name: "NguoiDans");
        }
    }
}
