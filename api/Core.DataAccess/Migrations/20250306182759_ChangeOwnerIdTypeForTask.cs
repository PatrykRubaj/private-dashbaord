﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class ChangeOwnerIdTypeForTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "owner_id",
                table: "tasks",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "owner_id",
                table: "tasks",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
