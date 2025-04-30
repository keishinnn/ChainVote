using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChainVote.Migrations
{
    /// <inheritdoc />
    public partial class AddConfirmPasswordAdminModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ConfirmPassword",
                table: "AdminAccount",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConfirmPassword",
                table: "AdminAccount");
        }
    }
}
