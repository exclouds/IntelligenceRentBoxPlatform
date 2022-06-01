using Microsoft.EntityFrameworkCore.Migrations;

namespace Magicodes.Admin.Migrations
{
    public partial class update_BoxDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BoxTenantInfo",
                table: "BoxDetailses");

            migrationBuilder.AlterColumn<int>(
                name: "TenantId",
                table: "BusinessConfirms",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenantInfoId",
                table: "BusinessConfirms",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BoxTenantInfoNO",
                table: "BoxDetailses",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TenantInfoId",
                table: "BusinessConfirms");

            migrationBuilder.DropColumn(
                name: "BoxTenantInfoNO",
                table: "BoxDetailses");

            migrationBuilder.AlterColumn<string>(
                name: "TenantId",
                table: "BusinessConfirms",
                nullable: true,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BoxTenantInfo",
                table: "BoxDetailses",
                nullable: false,
                defaultValue: 0);
        }
    }
}
