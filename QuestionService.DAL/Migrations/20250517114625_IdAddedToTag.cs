using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace QuestionService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class IdAddedToTag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
            
            migrationBuilder.Sql("""
                                    UPDATE public."Tag"
                                 SET "Id" = sub."rn"
                                 FROM (
                                     SELECT "Name", row_number() OVER (ORDER BY "Name") AS rn
                                     FROM "Tag"
                                 ) sub
                                 WHERE public."Tag"."Name" = sub."Name";
                                 """);
            
            migrationBuilder.Sql("""
                                     SELECT setval(
                                         pg_get_serial_sequence('"Tag"', 'Id'),
                                         (SELECT MAX("Id") FROM public."Tag")
                                     );
                                 """);
            
            migrationBuilder.Sql("""
                                     UPDATE public."QuestionTag"
                                     SET "TagId" = tags."Id"
                                     FROM public."Tag" tags
                                     WHERE public."QuestionTag"."TagName" = tags."Name";
                                 """);

            migrationBuilder.DropIndex(
                name: "IX_QuestionTag_TagName_QuestionId",
                table: "QuestionTag");

            migrationBuilder.DropColumn(
                name: "TagName",
                table: "QuestionTag");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tag",
                table: "Tag",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuestionTag",
                table: "QuestionTag",
                columns: new[] { "QuestionId", "TagId" });

            migrationBuilder.CreateIndex(
                name: "IX_QuestionTag_TagId_QuestionId",
                table: "QuestionTag",
                columns: new[] { "TagId", "QuestionId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionTag_Tag_TagId",
                table: "QuestionTag",
                column: "TagId",
                principalTable: "Tag",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TagName",
                table: "QuestionTag",
                type: "character varying(35)",
                nullable: false,
                defaultValue: "");
            
            migrationBuilder.Sql("""
                                 UPDATE public."QuestionTag"
                                 SET "TagName" = tags."Name"
                                 FROM public."Tag" tags
                                 WHERE public."QuestionTag"."TagId" = tags."Id";
                                 """);
            
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
                name: "IX_QuestionTag_TagId_QuestionId",
                table: "QuestionTag");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Tag");

            migrationBuilder.DropColumn(
                name: "TagId",
                table: "QuestionTag");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tag",
                table: "Tag",
                column: "Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuestionTag",
                table: "QuestionTag",
                columns: new[] { "QuestionId", "TagName" });

            migrationBuilder.CreateIndex(
                name: "IX_QuestionTag_TagName_QuestionId",
                table: "QuestionTag",
                columns: new[] { "TagName", "QuestionId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionTag_Tag_TagName",
                table: "QuestionTag",
                column: "TagName",
                principalTable: "Tag",
                principalColumn: "Name",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
