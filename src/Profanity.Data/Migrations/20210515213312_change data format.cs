using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Profanity.Data.Migrations
{
    public partial class changedataformat : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte[]>(
                name: "ProfanityWord",
                table: "ProfanityEntities",
                type: "BLOB",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Language",
                table: "ProfanityEntities",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Language",
                table: "ProfanityEntities");

            migrationBuilder.AlterColumn<string>(
                name: "ProfanityWord",
                table: "ProfanityEntities",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(byte[]),
                oldType: "BLOB",
                oldNullable: true);
        }
    }
}
