using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Magicodes.Admin.Migrations
{
    public partial class AddLiveChat : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IMChatMsgs",
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
                    ClientChatId = table.Column<long>(nullable: true),
                    Role = table.Column<string>(maxLength: 20, nullable: true),
                    ContentType = table.Column<string>(maxLength: 20, nullable: true),
                    Content = table.Column<string>(maxLength: 500, nullable: true),
                    IsNewMsg = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IMChatMsgs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IMClientEns",
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
                    ClientChatId = table.Column<long>(nullable: true),
                    ClientChatName = table.Column<string>(maxLength: 20, nullable: true),
                    ServerChatId = table.Column<long>(nullable: true),
                    State = table.Column<string>(maxLength: 20, nullable: true),
                    AccessTime = table.Column<DateTime>(nullable: true),
                    BillNO = table.Column<string>(maxLength: 50, nullable: true),
                    InputContent = table.Column<string>(maxLength: 500, nullable: true),
                    NewMsgCount = table.Column<int>(nullable: true),
                    IsFollow = table.Column<bool>(nullable: true),
                    LastMsgTime = table.Column<DateTime>(nullable: true),
                    LastMsgContent = table.Column<string>(maxLength: 500, nullable: true),
                    LastMsgShowTime = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IMClientEns", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IMServerEns",
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
                    ServerChatId = table.Column<long>(nullable: true),
                    ServerChatName = table.Column<string>(maxLength: 20, nullable: true),
                    State = table.Column<string>(nullable: true),
                    AccessTime = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IMServerEns", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IMChatMsgs");

            migrationBuilder.DropTable(
                name: "IMClientEns");

            migrationBuilder.DropTable(
                name: "IMServerEns");
        }
    }
}
