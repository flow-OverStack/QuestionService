using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuestionService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class ReputationPropertyRemoved : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Reputation",
                table: "Question");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Reputation",
                table: "Question",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
