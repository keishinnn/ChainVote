using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChainVote.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "CandidatesData");

            migrationBuilder.DropColumn(
                name: "Position",
                table: "CandidatesData");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "CandidatesData");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "CandidatesData",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_CandidatesData_ApplicationUserId",
                table: "CandidatesData",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_CandidatesData_AspNetUsers_ApplicationUserId",
                table: "CandidatesData",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CandidatesData_AspNetUsers_ApplicationUserId",
                table: "CandidatesData");

            migrationBuilder.DropIndex(
                name: "IX_CandidatesData_ApplicationUserId",
                table: "CandidatesData");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "CandidatesData");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "CandidatesData",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Position",
                table: "CandidatesData",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "CandidatesData",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
