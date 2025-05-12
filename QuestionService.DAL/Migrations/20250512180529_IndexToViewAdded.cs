using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuestionService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class IndexToViewAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_View_QuestionId",
                table: "View");

            migrationBuilder.CreateIndex(
                name: "IX_View_QuestionId_UserId_UserIp_UserFingerprint",
                table: "View",
                columns: new[] { "QuestionId", "UserId", "UserIp", "UserFingerprint" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_View_QuestionId_UserId_UserIp_UserFingerprint",
                table: "View");

            migrationBuilder.CreateIndex(
                name: "IX_View_QuestionId",
                table: "View",
                column: "QuestionId");
        }
    }
}
