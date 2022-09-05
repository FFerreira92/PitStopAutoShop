using Microsoft.EntityFrameworkCore.Migrations;

namespace PitStopAutoShop.Web.Migrations
{
    public partial class updatedWorkOrdersEntityAgain : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrders_Employees_ServiceDoneById",
                table: "WorkOrders");

            migrationBuilder.AlterColumn<string>(
                name: "ServiceDoneById",
                table: "WorkOrders",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrders_AspNetUsers_ServiceDoneById",
                table: "WorkOrders",
                column: "ServiceDoneById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrders_AspNetUsers_ServiceDoneById",
                table: "WorkOrders");

            migrationBuilder.AlterColumn<int>(
                name: "ServiceDoneById",
                table: "WorkOrders",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrders_Employees_ServiceDoneById",
                table: "WorkOrders",
                column: "ServiceDoneById",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
