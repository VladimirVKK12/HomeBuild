using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeBuild.Migrations
{
    /// <inheritdoc />
    public partial class op : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Usernamne",
                table: "ShoppingCartHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Usernamne",
                table: "ShoppingCartHistories");
        }
    }
}
