using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChainVote.Migrations
{
    /// <inheritdoc />
    public partial class AddDtoAndUpdateRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PositionsData");

            migrationBuilder.AddColumn<int>(
                name: "OrganizationsDataId",
                table: "OrganizationsData",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OrganizationPosition",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    OrganizationId = table.Column<int>(type: "int", nullable: false),
                    EventId = table.Column<int>(type: "int", nullable: false),
                    CandidateId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationPosition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganizationPosition_CandidatesData_CandidateId",
                        column: x => x.CandidateId,
                        principalTable: "CandidatesData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_OrganizationPosition_EventsData_EventId",
                        column: x => x.EventId,
                        principalTable: "EventsData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrganizationPosition_OrganizationsData_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "OrganizationsData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationsData_OrganizationsDataId",
                table: "OrganizationsData",
                column: "OrganizationsDataId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationPosition_CandidateId",
                table: "OrganizationPosition",
                column: "CandidateId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationPosition_EventId",
                table: "OrganizationPosition",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationPosition_OrganizationId",
                table: "OrganizationPosition",
                column: "OrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationsData_OrganizationsData_OrganizationsDataId",
                table: "OrganizationsData",
                column: "OrganizationsDataId",
                principalTable: "OrganizationsData",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationsData_OrganizationsData_OrganizationsDataId",
                table: "OrganizationsData");

            migrationBuilder.DropTable(
                name: "OrganizationPosition");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationsData_OrganizationsDataId",
                table: "OrganizationsData");

            migrationBuilder.DropColumn(
                name: "OrganizationsDataId",
                table: "OrganizationsData");

            migrationBuilder.CreateTable(
                name: "PositionsData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventId = table.Column<int>(type: "int", nullable: false),
                    PositionName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PositionsData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PositionsData_EventsData_EventId",
                        column: x => x.EventId,
                        principalTable: "EventsData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Positions_EventId",
                table: "PositionsData",
                column: "EventId");
        }
    }
}
