using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bazy_II.Migrations
{
    /// <inheritdoc />
    public partial class invoices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceProduct_Invoice_InvoiceID",
                table: "InvoiceProduct");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Invoice",
                table: "Invoice");

            migrationBuilder.RenameTable(
                name: "Invoice",
                newName: "Invoices");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Invoices",
                table: "Invoices",
                column: "InvoiceID");

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceProduct_Invoices_InvoiceID",
                table: "InvoiceProduct",
                column: "InvoiceID",
                principalTable: "Invoices",
                principalColumn: "InvoiceID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceProduct_Invoices_InvoiceID",
                table: "InvoiceProduct");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Invoices",
                table: "Invoices");

            migrationBuilder.RenameTable(
                name: "Invoices",
                newName: "Invoice");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Invoice",
                table: "Invoice",
                column: "InvoiceID");

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceProduct_Invoice_InvoiceID",
                table: "InvoiceProduct",
                column: "InvoiceID",
                principalTable: "Invoice",
                principalColumn: "InvoiceID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
