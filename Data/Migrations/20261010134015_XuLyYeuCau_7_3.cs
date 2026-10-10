using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLThuGomRac_UNETI02_TI17A1HN.Data.Migrations
{
    /// <inheritdoc />
    public partial class XuLyYeuCau_7_3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LyDoKhongThucHien",
                table: "PhanCongThuGoms",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayBatDau",
                table: "PhanCongThuGoms",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NgayKetThuc",
                table: "PhanCongThuGoms",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LyDoKhongThucHien",
                table: "PhanCongThuGoms");

            migrationBuilder.DropColumn(
                name: "NgayBatDau",
                table: "PhanCongThuGoms");

            migrationBuilder.DropColumn(
                name: "NgayKetThuc",
                table: "PhanCongThuGoms");
        }
    }
}
