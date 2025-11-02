using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace QuestionService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class OutboxMessageIdTypeChangedToLong : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_OutboxMessage",
                table: "OutboxMessage");
            
            migrationBuilder.AddColumn<long>(name: "NewId",
                table: "OutboxMessage",
                type: "bigint",
                nullable: false)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);
            
            migrationBuilder.Sql("""
                                    UPDATE public."OutboxMessage"
                                    SET "NewId" = sub."rn"
                                    FROM (
                                        SELECT "Id", row_number() OVER (ORDER BY "ProcessedAt") AS rn
                                        FROM "OutboxMessage"
                                    ) sub
                                    WHERE public."OutboxMessage"."Id" = sub."Id";
                                 """);
            
            migrationBuilder.Sql("""
                                     SELECT setval(
                                         pg_get_serial_sequence('"OutboxMessage"', 'NewId'),
                                         (SELECT MAX("NewId") FROM public."OutboxMessage")
                                     );
                                 """);
                
            migrationBuilder.DropColumn(
                name: "Id",
                table: "OutboxMessage");
            
            migrationBuilder.RenameColumn(
                name: "NewId",
                table: "OutboxMessage",
                newName: "Id");
            
            migrationBuilder.AddPrimaryKey(
                name: "PK_OutboxMessage",
                table: "OutboxMessage",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "NewId",
                table: "OutboxMessage",
                type: "uuid",
                defaultValueSql: "gen_random_uuid()",
                nullable: false);
            
            migrationBuilder.DropPrimaryKey(
                name: "PK_OutboxMessage",
                table: "OutboxMessage");
            
            migrationBuilder.DropColumn(
                name: "Id",
                table: "OutboxMessage");
            
            migrationBuilder.RenameColumn(
                name: "NewId",
                table: "OutboxMessage",
                newName: "Id");
            
            migrationBuilder.AddPrimaryKey(
                name: "PK_OutboxMessage",
                table: "OutboxMessage",
                column: "Id");
        }
    }
}
