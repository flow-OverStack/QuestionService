using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuestionService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class ConstraintsUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserIp",
                table: "View",
                type: "character(39)",
                fixedLength: true,
                maxLength: 39,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(40)",
                oldMaxLength: 40,
                oldNullable: true);

            migrationBuilder.Sql("""
                                 CREATE EXTENSION IF NOT EXISTS pgcrypto;
                                 UPDATE public."View"
                                 SET "UserFingerprint" = encode(digest("UserFingerprint", 'sha256'), 'hex')
                                 WHERE "UserFingerprint" IS NOT NULL AND length("UserFingerprint") <> 64;
                                 """);
            
            migrationBuilder.AlterColumn<string>(
                name: "UserFingerprint",
                table: "View",
                type: "character(64)",
                fixedLength: true,
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.Sql("""
                                 UPDATE public."Question"
                                 SET "Title" = LEFT("Title", 150)
                                 WHERE "Title" IS NOT NULL AND length("Title") > 150;
                                 """);
            
            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Question",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.Sql("""
                                    UPDATE public."Question"
                                    SET "Body" = LEFT("Body", 30000)
                                    WHERE "Body" IS NOT NULL AND length("Body") > 30000;
                                 """);
            
            migrationBuilder.AlterColumn<string>(
                name: "Body",
                table: "Question",
                type: "character varying(30000)",
                maxLength: 30000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserIp",
                table: "View",
                type: "character varying(40)",
                maxLength: 40,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character(39)",
                oldFixedLength: true,
                oldMaxLength: 39,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UserFingerprint",
                table: "View",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character(64)",
                oldFixedLength: true,
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Question",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<string>(
                name: "Body",
                table: "Question",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(30000)",
                oldMaxLength: 30000);
        }
    }
}
