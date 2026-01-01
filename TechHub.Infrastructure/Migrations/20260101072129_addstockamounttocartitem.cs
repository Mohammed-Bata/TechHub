using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addstockamounttocartitem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StockAmount",
                table: "CartItems",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StockAmount",
                table: "CartItems");
        }
    }
}
