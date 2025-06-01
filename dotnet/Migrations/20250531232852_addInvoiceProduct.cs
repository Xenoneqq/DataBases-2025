using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bazy_II.Migrations
{
    /// <inheritdoc />
    public partial class addInvoiceProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceProduct_Invoices_InvoiceID",
                table: "InvoiceProduct");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceProduct_Products_ProductID",
                table: "InvoiceProduct");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InvoiceProduct",
                table: "InvoiceProduct");

            migrationBuilder.RenameTable(
                name: "InvoiceProduct",
                newName: "InvoiceProducts");

            migrationBuilder.RenameIndex(
                name: "IX_InvoiceProduct_ProductID",
                table: "InvoiceProducts",
                newName: "IX_InvoiceProducts_ProductID");

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "InvoiceProducts",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_InvoiceProducts",
                table: "InvoiceProducts",
                columns: new[] { "InvoiceID", "ProductID" });

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_InvoiceNumber",
                table: "Invoices",
                column: "InvoiceNumber",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceProducts_Invoices_InvoiceID",
                table: "InvoiceProducts",
                column: "InvoiceID",
                principalTable: "Invoices",
                principalColumn: "InvoiceID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceProducts_Products_ProductID",
                table: "InvoiceProducts",
                column: "ProductID",
                principalTable: "Products",
                principalColumn: "ProductID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceProducts_Invoices_InvoiceID",
                table: "InvoiceProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceProducts_Products_ProductID",
                table: "InvoiceProducts");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_InvoiceNumber",
                table: "Invoices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InvoiceProducts",
                table: "InvoiceProducts");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "InvoiceProducts");

            migrationBuilder.RenameTable(
                name: "InvoiceProducts",
                newName: "InvoiceProduct");

            migrationBuilder.RenameIndex(
                name: "IX_InvoiceProducts_ProductID",
                table: "InvoiceProduct",
                newName: "IX_InvoiceProduct_ProductID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InvoiceProduct",
                table: "InvoiceProduct",
                columns: new[] { "InvoiceID", "ProductID" });

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceProduct_Invoices_InvoiceID",
                table: "InvoiceProduct",
                column: "InvoiceID",
                principalTable: "Invoices",
                principalColumn: "InvoiceID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceProduct_Products_ProductID",
                table: "InvoiceProduct",
                column: "ProductID",
                principalTable: "Products",
                principalColumn: "ProductID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
