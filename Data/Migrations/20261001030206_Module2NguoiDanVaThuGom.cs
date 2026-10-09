// Họ và tên: Phạm Văn Hào
// Mã sinh viên: 23103100041
// Nội dung thực hiện: Module 2 – Migration Module 2 (§6.1–§6.4)

using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace QLThuGomRac_UNETI02_TI17A1HN.Data.Migrations
{
    public partial class Module2NguoiDanVaThuGom : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "YeuCauThuGoms");

            migrationBuilder.DropTable(
                name: "NguoiDans");
        }
    }
}
