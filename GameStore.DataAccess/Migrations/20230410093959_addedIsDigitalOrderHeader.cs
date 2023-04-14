using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameStore.DataAccess.Migrations
{
    public partial class addedIsDigitalOrderHeader : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDigital",
                table: "OrderHeaders",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDigital",
                table: "OrderHeaders");
        }
    }
}
