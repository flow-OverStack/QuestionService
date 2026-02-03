using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuestionService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class MinReputationToVoteAddedToVoteType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MinReputationToVote",
                table: "VoteType",
                type: "integer",
                nullable: false,
                defaultValue: 0);
            migrationBuilder.Sql("""
                                 UPDATE public."VoteType"
                                 SET "MinReputationToVote" = CASE
                                     WHEN "Name" = 'Upvote' THEN 15
                                     WHEN "Name" = 'Downvote' THEN 125
                                     ELSE 0
                                 END;
                                 """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MinReputationToVote",
                table: "VoteType");
        }
    }
}
