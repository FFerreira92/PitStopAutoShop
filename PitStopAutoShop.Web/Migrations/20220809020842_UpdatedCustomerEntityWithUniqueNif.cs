using Microsoft.EntityFrameworkCore.Migrations;

namespace PitStopAutoShop.Web.Migrations
{
    public partial class UpdatedCustomerEntityWithUniqueNif : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Nif",
                table: "Customers",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_Nif",
                table: "Customers",
                column: "Nif",
                unique: true,
                filter: "[Nif] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Customers_Nif",
                table: "Customers");

            migrationBuilder.AlterColumn<string>(
                name: "Nif",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}
