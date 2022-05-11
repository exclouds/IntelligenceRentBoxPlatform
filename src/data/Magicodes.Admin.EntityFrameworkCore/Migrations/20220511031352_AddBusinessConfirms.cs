using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Magicodes.Admin.Migrations
{
    public partial class AddBusinessConfirms : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Country",
                table: "SiteTables");

            migrationBuilder.DropColumn(
                name: "CertificateUrl",
                table: "BoxDetailses");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "BoxDetailses");

            migrationBuilder.AlterColumn<int>(
                name: "Line",
                table: "TenantInfos",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BillNO",
                table: "TenantInfos",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "Finish",
                table: "TenantInfos",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "InquiryNum",
                table: "TenantInfos",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsVerify",
                table: "TenantInfos",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "VerifyRem",
                table: "TenantInfos",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CountryCode",
                table: "SiteTables",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Line",
                table: "BoxInfos",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BillNO",
                table: "BoxInfos",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "Finish",
                table: "BoxInfos",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "InquiryNum",
                table: "BoxInfos",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UserNature",
                table: "AbpUsers",
                nullable: false,
                oldClrType: typeof(bool));

            migrationBuilder.AddColumn<bool>(
                name: "IsVerify",
                table: "AbpUsers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "VerifyRem",
                table: "AbpUsers",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsVerify",
                table: "AbpOrganizationUnits",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VerifyRem",
                table: "AbpOrganizationUnits",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BusinessConfirms",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatorUserId = table.Column<long>(nullable: true),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Remarks = table.Column<string>(maxLength: 500, nullable: true),
                    BoxInfoBillNO = table.Column<string>(maxLength: 50, nullable: true),
                    BoxId = table.Column<int>(nullable: false),
                    TenantInfoBillNO = table.Column<string>(maxLength: 50, nullable: true),
                    TenantId = table.Column<string>(nullable: true),
                    TenantMargin = table.Column<decimal>(nullable: false),
                    SellingPrice = table.Column<decimal>(nullable: false),
                    PurchasePrice = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessConfirms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatorUserId = table.Column<long>(nullable: true),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    TenantId = table.Column<int>(nullable: true),
                    Remarks = table.Column<string>(maxLength: 500, nullable: true),
                    Code = table.Column<string>(maxLength: 50, nullable: false),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    IsEnable = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Lines",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatorUserId = table.Column<long>(nullable: true),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    TenantId = table.Column<int>(nullable: true),
                    Remarks = table.Column<string>(maxLength: 500, nullable: true),
                    LineName = table.Column<string>(maxLength: 50, nullable: true),
                    IsEnable = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lines", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LinSites",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatorUserId = table.Column<long>(nullable: true),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    TenantId = table.Column<int>(nullable: true),
                    Remarks = table.Column<string>(maxLength: 500, nullable: true),
                    Code = table.Column<string>(maxLength: 50, nullable: false),
                    LineId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LinSites", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BusinessConfirms");

            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropTable(
                name: "Lines");

            migrationBuilder.DropTable(
                name: "LinSites");

            migrationBuilder.DropColumn(
                name: "BillNO",
                table: "TenantInfos");

            migrationBuilder.DropColumn(
                name: "Finish",
                table: "TenantInfos");

            migrationBuilder.DropColumn(
                name: "InquiryNum",
                table: "TenantInfos");

            migrationBuilder.DropColumn(
                name: "IsVerify",
                table: "TenantInfos");

            migrationBuilder.DropColumn(
                name: "VerifyRem",
                table: "TenantInfos");

            migrationBuilder.DropColumn(
                name: "CountryCode",
                table: "SiteTables");

            migrationBuilder.DropColumn(
                name: "BillNO",
                table: "BoxInfos");

            migrationBuilder.DropColumn(
                name: "Finish",
                table: "BoxInfos");

            migrationBuilder.DropColumn(
                name: "InquiryNum",
                table: "BoxInfos");

            migrationBuilder.DropColumn(
                name: "IsVerify",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "VerifyRem",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "IsVerify",
                table: "AbpOrganizationUnits");

            migrationBuilder.DropColumn(
                name: "VerifyRem",
                table: "AbpOrganizationUnits");

            migrationBuilder.AlterColumn<string>(
                name: "Line",
                table: "TenantInfos",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(int),
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "SiteTables",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Line",
                table: "BoxInfos",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(int),
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CertificateUrl",
                table: "BoxDetailses",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "BoxDetailses",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "UserNature",
                table: "AbpUsers",
                nullable: false,
                oldClrType: typeof(int));
        }
    }
}
