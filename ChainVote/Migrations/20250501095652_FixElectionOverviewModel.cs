using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChainVote.Migrations
{
    /// <inheritdoc />
    public partial class FixElectionOverviewModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EventId",
                table: "OrganizationsData",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationsData_EventId",
                table: "OrganizationsData",
                column: "EventId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationsData_EventsData_EventId",
                table: "OrganizationsData",
                column: "EventId",
                principalTable: "EventsData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationsData_EventsData_EventId",
                table: "OrganizationsData");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationsData_EventId",
                table: "OrganizationsData");

            migrationBuilder.DropColumn(
                name: "EventId",
                table: "OrganizationsData");
        }
    }
}
