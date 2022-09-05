using Microsoft.EntityFrameworkCore.Migrations;

namespace PitStopAutoShop.Web.Migrations
{
    public partial class updatedEstimateEntityToHaveAFaultDescription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FaultDescription",
                table: "Estimates",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FaultDescription",
                table: "Estimates");
        }
    }
}
