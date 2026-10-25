using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLThuGomRac_UNETI02_TI17A1HN.Data.Migrations
{
    /// <inheritdoc />
    public partial class KetQuaThuGom_7_4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KetQuaThuGoms",
                columns: table => new
                {
                    MaKetQua = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaYeuCau = table.Column<int>(type: "int", nullable: false),
                    MaNhanVien = table.Column<int>(type: "int", nullable: false),
                    ThoiGianHoanThanh = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KetQuaThuGoms", x => x.MaKetQua);
                    table.ForeignKey(
                        name: "FK_KetQuaThuGoms_NhanViens_MaNhanVien",
                        column: x => x.MaNhanVien,
                        principalTable: "NhanViens",
                        principalColumn: "MaNhanVien",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KetQuaThuGoms_YeuCauThuGoms_MaYeuCau",
                        column: x => x.MaYeuCau,
                        principalTable: "YeuCauThuGoms",
                        principalColumn: "MaYeuCau",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChiTietKetQuaThuGoms",
                columns: table => new
                {
                    MaChiTietKetQua = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaKetQua = table.Column<int>(type: "int", nullable: false),
                    MaLoaiRac = table.Column<int>(type: "int", nullable: false),
                    KhoiLuongThucTe = table.Column<decimal>(type: "decimal(12,3)", precision: 12, scale: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietKetQuaThuGoms", x => x.MaChiTietKetQua);
                    table.CheckConstraint("CK_ChiTietKetQua_KhoiLuong", "[KhoiLuongThucTe] >= 0");
                    table.ForeignKey(
                        name: "FK_ChiTietKetQuaThuGoms_KetQuaThuGoms_MaKetQua",
                        column: x => x.MaKetQua,
                        principalTable: "KetQuaThuGoms",
                        principalColumn: "MaKetQua",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChiTietKetQuaThuGoms_LoaiRacs_MaLoaiRac",
                        column: x => x.MaLoaiRac,
                        principalTable: "LoaiRacs",
                        principalColumn: "MaLoaiRac",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietKetQuaThuGoms_MaKetQua_MaLoaiRac",
                table: "ChiTietKetQuaThuGoms",
                columns: new[] { "MaKetQua", "MaLoaiRac" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietKetQuaThuGoms_MaLoaiRac",
                table: "ChiTietKetQuaThuGoms",
                column: "MaLoaiRac");

            migrationBuilder.CreateIndex(
                name: "IX_KetQuaThuGoms_MaNhanVien",
                table: "KetQuaThuGoms",
                column: "MaNhanVien");

            migrationBuilder.CreateIndex(
                name: "IX_KetQuaThuGoms_MaYeuCau",
                table: "KetQuaThuGoms",
                column: "MaYeuCau",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChiTietKetQuaThuGoms");

            migrationBuilder.DropTable(
                name: "KetQuaThuGoms");
        }
    }
}
