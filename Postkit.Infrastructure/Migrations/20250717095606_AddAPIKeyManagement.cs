using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Postkit.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAPIKeyManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_ApplicationClients_ApplicationClientId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_ApplicationClients_ApplicationClientId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_ApplicationClients_ApplicationClientId",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_Reactions_ApplicationClients_ApplicationClientId",
                table: "Reactions");

            migrationBuilder.DropTable(
                name: "ApplicationClients");

            migrationBuilder.RenameColumn(
                name: "ApplicationClientId",
                table: "Reactions",
                newName: "ApiClientId");

            migrationBuilder.RenameIndex(
                name: "IX_Reactions_ApplicationClientId",
                table: "Reactions",
                newName: "IX_Reactions_ApiClientId");

            migrationBuilder.RenameColumn(
                name: "ApplicationClientId",
                table: "Posts",
                newName: "ApiClientId");

            migrationBuilder.RenameIndex(
                name: "IX_Posts_ApplicationClientId",
                table: "Posts",
                newName: "IX_Posts_ApiClientId");

            migrationBuilder.RenameColumn(
                name: "ApplicationClientId",
                table: "Comments",
                newName: "ApiClientId");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_ApplicationClientId",
                table: "Comments",
                newName: "IX_Comments_ApiClientId");

            migrationBuilder.RenameColumn(
                name: "ApplicationClientId",
                table: "AspNetUsers",
                newName: "ApiClientId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUsers_ApplicationClientId",
                table: "AspNetUsers",
                newName: "IX_AspNetUsers_ApiClientId");

            migrationBuilder.CreateTable(
                name: "ApiClients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HashedApiKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUsedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiClients", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_ApiClients_ApiClientId",
                table: "AspNetUsers",
                column: "ApiClientId",
                principalTable: "ApiClients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_ApiClients_ApiClientId",
                table: "Comments",
                column: "ApiClientId",
                principalTable: "ApiClients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_ApiClients_ApiClientId",
                table: "Posts",
                column: "ApiClientId",
                principalTable: "ApiClients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reactions_ApiClients_ApiClientId",
                table: "Reactions",
                column: "ApiClientId",
                principalTable: "ApiClients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_ApiClients_ApiClientId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_ApiClients_ApiClientId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_ApiClients_ApiClientId",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_Reactions_ApiClients_ApiClientId",
                table: "Reactions");

            migrationBuilder.DropTable(
                name: "ApiClients");

            migrationBuilder.RenameColumn(
                name: "ApiClientId",
                table: "Reactions",
                newName: "ApplicationClientId");

            migrationBuilder.RenameIndex(
                name: "IX_Reactions_ApiClientId",
                table: "Reactions",
                newName: "IX_Reactions_ApplicationClientId");

            migrationBuilder.RenameColumn(
                name: "ApiClientId",
                table: "Posts",
                newName: "ApplicationClientId");

            migrationBuilder.RenameIndex(
                name: "IX_Posts_ApiClientId",
                table: "Posts",
                newName: "IX_Posts_ApplicationClientId");

            migrationBuilder.RenameColumn(
                name: "ApiClientId",
                table: "Comments",
                newName: "ApplicationClientId");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_ApiClientId",
                table: "Comments",
                newName: "IX_Comments_ApplicationClientId");

            migrationBuilder.RenameColumn(
                name: "ApiClientId",
                table: "AspNetUsers",
                newName: "ApplicationClientId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUsers_ApiClientId",
                table: "AspNetUsers",
                newName: "IX_AspNetUsers_ApplicationClientId");

            migrationBuilder.CreateTable(
                name: "ApplicationClients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationClients", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_ApplicationClients_ApplicationClientId",
                table: "AspNetUsers",
                column: "ApplicationClientId",
                principalTable: "ApplicationClients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_ApplicationClients_ApplicationClientId",
                table: "Comments",
                column: "ApplicationClientId",
                principalTable: "ApplicationClients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_ApplicationClients_ApplicationClientId",
                table: "Posts",
                column: "ApplicationClientId",
                principalTable: "ApplicationClients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reactions_ApplicationClients_ApplicationClientId",
                table: "Reactions",
                column: "ApplicationClientId",
                principalTable: "ApplicationClients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
