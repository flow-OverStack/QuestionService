using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace QuestionService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class TagIdRemoved : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuestionTag_Tag_TagId",
                table: "QuestionTag");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tag",
                table: "Tag");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QuestionTag",
                table: "QuestionTag");

            migrationBuilder.DropIndex(
                name: "IX_QuestionTag_TagId",
                table: "QuestionTag");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Tag");

            migrationBuilder.DropColumn(
                name: "TagId",
                table: "QuestionTag");

            migrationBuilder.AddColumn<string>(
                name: "TagName",
                table: "QuestionTag",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tag",
                table: "Tag",
                column: "Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuestionTag",
                table: "QuestionTag",
                columns: new[] { "QuestionId", "TagName" });

            migrationBuilder.CreateIndex(
                name: "IX_QuestionTag_TagName",
                table: "QuestionTag",
                column: "TagName");

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionTag_Tag_TagName",
                table: "QuestionTag",
                column: "TagName",
                principalTable: "Tag",
                principalColumn: "Name",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuestionTag_Tag_TagName",
                table: "QuestionTag");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tag",
                table: "Tag");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QuestionTag",
                table: "QuestionTag");

            migrationBuilder.DropIndex(
                name: "IX_QuestionTag_TagName",
                table: "QuestionTag");

            migrationBuilder.DropColumn(
                name: "TagName",
                table: "QuestionTag");

            migrationBuilder.AddColumn<long>(
                name: "Id",
                table: "Tag",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<long>(
                name: "TagId",
                table: "QuestionTag",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tag",
                table: "Tag",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuestionTag",
                table: "QuestionTag",
                columns: new[] { "QuestionId", "TagId" });

            migrationBuilder.CreateIndex(
                name: "IX_QuestionTag_TagId",
                table: "QuestionTag",
                column: "TagId");

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionTag_Tag_TagId",
                table: "QuestionTag",
                column: "TagId",
                principalTable: "Tag",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
