using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChainVote.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOrgsData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationPosition_EventsData_EventId",
                table: "OrganizationPosition");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationsData_OrganizationsData_OrganizationsDataId",
                table: "OrganizationsData");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationsData_OrganizationsDataId",
                table: "OrganizationsData");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationPosition_EventId",
                table: "OrganizationPosition");

            migrationBuilder.DropColumn(
                name: "OrganizationsDataId",
                table: "OrganizationsData");

            migrationBuilder.DropColumn(
                name: "EventId",
                table: "OrganizationPosition");

            migrationBuilder.AddColumn<int>(
                name: "EventsDataId",
                table: "OrganizationPosition",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationPosition_EventsDataId",
                table: "OrganizationPosition",
                column: "EventsDataId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationPosition_EventsData_EventsDataId",
                table: "OrganizationPosition",
                column: "EventsDataId",
                principalTable: "EventsData",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationPosition_EventsData_EventsDataId",
                table: "OrganizationPosition");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationPosition_EventsDataId",
                table: "OrganizationPosition");

            migrationBuilder.DropColumn(
                name: "EventsDataId",
                table: "OrganizationPosition");

            migrationBuilder.AddColumn<int>(
                name: "OrganizationsDataId",
                table: "OrganizationsData",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EventId",
                table: "OrganizationPosition",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationsData_OrganizationsDataId",
                table: "OrganizationsData",
                column: "OrganizationsDataId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationPosition_EventId",
                table: "OrganizationPosition",
                column: "EventId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationPosition_EventsData_EventId",
                table: "OrganizationPosition",
                column: "EventId",
                principalTable: "EventsData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationsData_OrganizationsData_OrganizationsDataId",
                table: "OrganizationsData",
                column: "OrganizationsDataId",
                principalTable: "OrganizationsData",
                principalColumn: "Id");
        }
    }
}
