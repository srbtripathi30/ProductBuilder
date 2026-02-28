using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductBuilder.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixStaticSeedHash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                column: "password_hash",
                value: "$2a$11$ClXb35kkrElNMZw.SG3YeOw1bwXBImytjNaOek.QFmxMaK7mSnKqS");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                column: "password_hash",
                value: "$2a$11$ZE0pRtvQcTDsoQnDW4d3AOwFtG15eoI/H7Y4Pvv8sJptEDvsICJwe");
        }
    }
}
