using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChainVote.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexingToTheVoteRecords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VoteRecords_AspNetUsers_VoterId",
                table: "VoteRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_VoteRecords_CandidatesData_CandidateId",
                table: "VoteRecords");

            migrationBuilder.AddForeignKey(
                name: "FK_VoteRecords_AspNetUsers_VoterId",
                table: "VoteRecords",
                column: "VoterId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VoteRecords_CandidatesData_CandidateId",
                table: "VoteRecords",
                column: "CandidateId",
                principalTable: "CandidatesData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VoteRecords_AspNetUsers_VoterId",
                table: "VoteRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_VoteRecords_CandidatesData_CandidateId",
                table: "VoteRecords");

            migrationBuilder.AddForeignKey(
                name: "FK_VoteRecords_AspNetUsers_VoterId",
                table: "VoteRecords",
                column: "VoterId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VoteRecords_CandidatesData_CandidateId",
                table: "VoteRecords",
                column: "CandidateId",
                principalTable: "CandidatesData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
