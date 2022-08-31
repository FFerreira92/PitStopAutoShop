using Microsoft.EntityFrameworkCore.Migrations;

namespace PitStopAutoShop.Web.Migrations
{
    public partial class updatedAppointmentEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EstimateId",
                table: "Appointments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_EstimateId",
                table: "Appointments",
                column: "EstimateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Estimates_EstimateId",
                table: "Appointments",
                column: "EstimateId",
                principalTable: "Estimates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Estimates_EstimateId",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_EstimateId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "EstimateId",
                table: "Appointments");
        }
    }
}
