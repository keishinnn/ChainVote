using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChainVote.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CandidatesData_Voters_VoterId",
                table: "CandidatesData");

            migrationBuilder.DropTable(
                name: "Voters");

            migrationBuilder.DropIndex(
                name: "IX_CandidatesData_VoterId",
                table: "CandidatesData");

            migrationBuilder.DropColumn(
                name: "VoterId",
                table: "CandidatesData");

            migrationBuilder.AddColumn<bool>(
                name: "HasVoted",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasVoted",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<int>(
                name: "VoterId",
                table: "CandidatesData",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Voters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EventId = table.Column<int>(type: "int", nullable: false),
                    Course = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HasVoted = table.Column<bool>(type: "bit", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Section = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StudentId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    YearLevel = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Voters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Voters_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Voters_EventsData_EventId",
                        column: x => x.EventId,
                        principalTable: "EventsData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CandidatesData_VoterId",
                table: "CandidatesData",
                column: "VoterId");

            migrationBuilder.CreateIndex(
                name: "IX_Voters_ApplicationUserId",
                table: "Voters",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Voters_EventId",
                table: "Voters",
                column: "EventId");

            migrationBuilder.AddForeignKey(
                name: "FK_CandidatesData_Voters_VoterId",
                table: "CandidatesData",
                column: "VoterId",
                principalTable: "Voters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
