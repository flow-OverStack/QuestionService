using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuestionService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class ReputationChangePropertyAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReputationChange",
                table: "VoteType",
                type: "integer",
                nullable: false,
                defaultValue: 0);
            
            migrationBuilder.Sql("""
                                 UPDATE public."VoteType"
                                 SET "ReputationChange" = CASE
                                     WHEN "Name" = 'Upvote' THEN 1
                                     WHEN "Name" = 'Downvote' THEN -1
                                     ELSE 0
                                 END;
                                 """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReputationChange",
                table: "VoteType");
        }
    }
}
