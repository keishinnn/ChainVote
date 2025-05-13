using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChainVote.Migrations
{
    /// <inheritdoc />
    public partial class RemoveElectionTypeColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ElectionType",
                table: "EventsData");

            migrationBuilder.AddColumn<int>(
                name: "ElectionType",
                table: "OrganizationsData",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ElectionType",
                table: "OrganizationsData");

            migrationBuilder.AddColumn<int>(
                name: "ElectionType",
                table: "EventsData",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
