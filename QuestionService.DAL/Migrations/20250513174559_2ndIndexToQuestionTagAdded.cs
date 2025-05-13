using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuestionService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class _2ndIndexToQuestionTagAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_QuestionTag_TagName",
                table: "QuestionTag");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionTag_TagName_QuestionId",
                table: "QuestionTag",
                columns: new[] { "TagName", "QuestionId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_QuestionTag_TagName_QuestionId",
                table: "QuestionTag");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionTag_TagName",
                table: "QuestionTag",
                column: "TagName");
        }
    }
}
