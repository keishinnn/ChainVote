using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChainVote.Migrations
{
    /// <inheritdoc />
    public partial class CheckForChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "CandidatesData",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "CandidatesData");
        }
    }
}
