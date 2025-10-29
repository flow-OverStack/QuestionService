using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using QuestionService.Domain.Enums;

#nullable disable

namespace QuestionService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class VoteTypeEntityAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "VoteTypeId",
                table: "Vote",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
            
            migrationBuilder.Sql("""
                UPDATE public."Vote"
                SET "VoteTypeId" = CASE
                    WHEN "ReputationChange" = 1 THEN 1
                    WHEN "ReputationChange" = -1 THEN 2
                END;
            """);
            
            migrationBuilder.DropColumn(
                name: "ReputationChange",
                table: "Vote");

            migrationBuilder.CreateTable(
                name: "VoteType",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoteType", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Vote_VoteTypeId",
                table: "Vote",
                column: "VoteTypeId");
            
            //Hardcoded values
            migrationBuilder.InsertData("VoteType", [ "Id", "Name" ], new object[,] {
                { 1L, nameof(VoteTypes.Upvote) },
                { 2L, nameof(VoteTypes.Downvote) }
            });

            migrationBuilder.AddForeignKey(
                name: "FK_Vote_VoteType_VoteTypeId",
                table: "Vote",
                column: "VoteTypeId",
                principalTable: "VoteType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vote_VoteType_VoteTypeId",
                table: "Vote");

            migrationBuilder.DropTable(
                name: "VoteType");

            migrationBuilder.DropIndex(
                name: "IX_Vote_VoteTypeId",
                table: "Vote");
            
            migrationBuilder.AddColumn<int>(
                name: "ReputationChange",
                table: "Vote",
                type: "integer",
                nullable: false,
                defaultValue: 0);
            
            migrationBuilder.Sql("""
                UPDATE public."Vote"
                SET "ReputationChange" = CASE
                    WHEN "VoteTypeId" = 1 THEN 1
                    WHEN "VoteTypeId" = 2 THEN -1
                END;
            """);
            
            migrationBuilder.DropColumn(
                name: "VoteTypeId", 
                table: "Vote");
        }
    }
}
