using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Magicodes.Admin.Migrations
{
    public partial class update_BoxInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsInStock",
                table: "BoxInfos");

            migrationBuilder.DropColumn(
                name: "PredictTime",
                table: "BoxInfos");

            migrationBuilder.AddColumn<string>(
                name: "ENSiteName",
                table: "SiteTables",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EndStation",
                table: "BoxInfos",
                maxLength: 8000,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 50);

            migrationBuilder.AddColumn<DateTime>(
                name: "PredictTime",
                table: "BoxDetailses",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ENSiteName",
                table: "SiteTables");

            migrationBuilder.DropColumn(
                name: "PredictTime",
                table: "BoxDetailses");

            migrationBuilder.AlterColumn<string>(
                name: "EndStation",
                table: "BoxInfos",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 8000,
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsInStock",
                table: "BoxInfos",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "PredictTime",
                table: "BoxInfos",
                nullable: true);
        }
    }
}
