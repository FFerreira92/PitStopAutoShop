using Microsoft.EntityFrameworkCore.Migrations;

namespace PitStopAutoShop.Web.Migrations
{
    public partial class updatedInvoiceEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Observations",
                table: "Invoices");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Observations",
                table: "Invoices",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
