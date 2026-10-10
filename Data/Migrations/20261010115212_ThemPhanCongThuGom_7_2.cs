using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLThuGomRac_UNETI02_TI17A1HN.Data.Migrations
{
    /// <inheritdoc />
    public partial class ThemPhanCongThuGom_7_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PhanCongThuGoms",
                columns: table => new
                {
                    MaPhanCong = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaYeuCau = table.Column<int>(type: "int", nullable: false),
                    MaNhanVien = table.Column<int>(type: "int", nullable: false),
                    NgayPhanCong = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    TrangThai = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhanCongThuGoms", x => x.MaPhanCong);
                    table.ForeignKey(
                        name: "FK_PhanCongThuGoms_NhanViens_MaNhanVien",
                        column: x => x.MaNhanVien,
                        principalTable: "NhanViens",
                        principalColumn: "MaNhanVien",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PhanCongThuGoms_YeuCauThuGoms_MaYeuCau",
                        column: x => x.MaYeuCau,
                        principalTable: "YeuCauThuGoms",
                        principalColumn: "MaYeuCau",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PhanCongThuGoms_MaNhanVien",
                table: "PhanCongThuGoms",
                column: "MaNhanVien");

            migrationBuilder.CreateIndex(
                name: "IX_PhanCongThuGoms_MaYeuCau",
                table: "PhanCongThuGoms",
                column: "MaYeuCau",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PhanCongThuGoms");
        }
    }
}
