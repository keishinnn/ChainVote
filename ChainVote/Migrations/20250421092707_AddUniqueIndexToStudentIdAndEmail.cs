using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChainVote.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueIndexToStudentIdAndEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ApplicationUser_Email",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_ApplicationUser_StudentId",
                table: "AspNetUsers");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUser_Email",
                table: "AspNetUsers",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUser_StudentId",
                table: "AspNetUsers",
                column: "StudentId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ApplicationUser_Email",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_ApplicationUser_StudentId",
                table: "AspNetUsers");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUser_Email",
                table: "AspNetUsers",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUser_StudentId",
                table: "AspNetUsers",
                column: "StudentId");
        }
    }
}
