using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bazy_II.Migrations
{
    /// <inheritdoc />
    public partial class bidirectionalRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductID",
                table: "Suppliers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductID",
                table: "Suppliers",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
