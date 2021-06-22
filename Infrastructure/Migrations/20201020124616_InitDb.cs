using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Migrations
{
    public partial class InitDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Usr_UserInfo",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    Age = table.Column<int>(nullable: false),
                    UserCode = table.Column<string>(type: "varchar(100)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usr_UserInfo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usr_UserAddress",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AddressTitle = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    ZIP = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    Province = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    Area = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    Street = table.Column<string>(type: "nvarchar(200)", nullable: true),
                    UserInfoId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usr_UserAddress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Usr_UserAddress_Usr_UserInfo_UserInfoId",
                        column: x => x.UserInfoId,
                        principalTable: "Usr_UserInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Usr_UserAddress_UserInfoId",
                table: "Usr_UserAddress",
                column: "UserInfoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Usr_UserAddress");

            migrationBuilder.DropTable(
                name: "Usr_UserInfo");
        }
    }
}
