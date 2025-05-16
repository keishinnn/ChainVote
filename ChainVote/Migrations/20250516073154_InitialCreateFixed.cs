using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChainVote.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateFixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CandidatesData_OrganizationsData_OrganizationId",
                table: "CandidatesData");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationPosition_CandidatesData_CandidateId",
                table: "OrganizationPosition");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationPosition_EventsData_EventsDataId",
                table: "OrganizationPosition");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationPosition_OrganizationsData_OrganizationId",
                table: "OrganizationPosition");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationsData_EventsData_EventId",
                table: "OrganizationsData");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationPosition_CandidateId",
                table: "OrganizationPosition");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationPosition_EventsDataId",
                table: "OrganizationPosition");

            migrationBuilder.DropColumn(
                name: "CandidateId",
                table: "OrganizationPosition");

            migrationBuilder.DropColumn(
                name: "EventsDataId",
                table: "OrganizationPosition");

            migrationBuilder.RenameColumn(
                name: "OrganizationId",
                table: "CandidatesData",
                newName: "PositionId");

            migrationBuilder.RenameIndex(
                name: "IX_CandidatesData_OrganizationId",
                table: "CandidatesData",
                newName: "IX_CandidatesData_PositionId");

            migrationBuilder.AlterColumn<int>(
                name: "OrganizationId",
                table: "OrganizationPosition",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "OrganizationsDataId",
                table: "CandidatesData",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CandidatesData_OrganizationsDataId",
                table: "CandidatesData",
                column: "OrganizationsDataId");

            migrationBuilder.AddForeignKey(
                name: "FK_CandidatesData_OrganizationPosition_PositionId",
                table: "CandidatesData",
                column: "PositionId",
                principalTable: "OrganizationPosition",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_CandidatesData_OrganizationsData_OrganizationsDataId",
                table: "CandidatesData",
                column: "OrganizationsDataId",
                principalTable: "OrganizationsData",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationPosition_OrganizationsData_OrganizationId",
                table: "OrganizationPosition",
                column: "OrganizationId",
                principalTable: "OrganizationsData",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationsData_EventsData_EventId",
                table: "OrganizationsData",
                column: "EventId",
                principalTable: "EventsData",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CandidatesData_OrganizationPosition_PositionId",
                table: "CandidatesData");

            migrationBuilder.DropForeignKey(
                name: "FK_CandidatesData_OrganizationsData_OrganizationsDataId",
                table: "CandidatesData");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationPosition_OrganizationsData_OrganizationId",
                table: "OrganizationPosition");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationsData_EventsData_EventId",
                table: "OrganizationsData");

            migrationBuilder.DropIndex(
                name: "IX_CandidatesData_OrganizationsDataId",
                table: "CandidatesData");

            migrationBuilder.DropColumn(
                name: "OrganizationsDataId",
                table: "CandidatesData");

            migrationBuilder.RenameColumn(
                name: "PositionId",
                table: "CandidatesData",
                newName: "OrganizationId");

            migrationBuilder.RenameIndex(
                name: "IX_CandidatesData_PositionId",
                table: "CandidatesData",
                newName: "IX_CandidatesData_OrganizationId");

            migrationBuilder.AlterColumn<int>(
                name: "OrganizationId",
                table: "OrganizationPosition",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CandidateId",
                table: "OrganizationPosition",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EventsDataId",
                table: "OrganizationPosition",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationPosition_CandidateId",
                table: "OrganizationPosition",
                column: "CandidateId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationPosition_EventsDataId",
                table: "OrganizationPosition",
                column: "EventsDataId");

            migrationBuilder.AddForeignKey(
                name: "FK_CandidatesData_OrganizationsData_OrganizationId",
                table: "CandidatesData",
                column: "OrganizationId",
                principalTable: "OrganizationsData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationPosition_CandidatesData_CandidateId",
                table: "OrganizationPosition",
                column: "CandidateId",
                principalTable: "CandidatesData",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationPosition_EventsData_EventsDataId",
                table: "OrganizationPosition",
                column: "EventsDataId",
                principalTable: "EventsData",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationPosition_OrganizationsData_OrganizationId",
                table: "OrganizationPosition",
                column: "OrganizationId",
                principalTable: "OrganizationsData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationsData_EventsData_EventId",
                table: "OrganizationsData",
                column: "EventId",
                principalTable: "EventsData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
