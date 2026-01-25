using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AITechDATA.Migrations
{
    /// <inheritdoc />
    public partial class Add_GroupChatMessageTable1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GroupChatMessages",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GroupId = table.Column<long>(type: "bigint", nullable: false),
                    SenderUserId = table.Column<long>(type: "bigint", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsEdited = table.Column<bool>(type: "bit", nullable: false),
                    EditedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReplyToMessageId = table.Column<long>(type: "bigint", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    OtherLangs = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupChatMessages", x => x.ID);
                    table.ForeignKey(
                        name: "FK_GroupChatMessages_GroupChatMessages_ReplyToMessageId",
                        column: x => x.ReplyToMessageId,
                        principalTable: "GroupChatMessages",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_GroupChatMessages_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupChatMessages_Users_SenderUserId",
                        column: x => x.SenderUserId,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GroupChatMessages_GroupId_ID",
                table: "GroupChatMessages",
                columns: new[] { "GroupId", "ID" });

            migrationBuilder.CreateIndex(
                name: "IX_GroupChatMessages_GroupId_IsDeleted",
                table: "GroupChatMessages",
                columns: new[] { "GroupId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_GroupChatMessages_GroupId_SentAt",
                table: "GroupChatMessages",
                columns: new[] { "GroupId", "SentAt" });

            migrationBuilder.CreateIndex(
                name: "IX_GroupChatMessages_ReplyToMessageId",
                table: "GroupChatMessages",
                column: "ReplyToMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupChatMessages_SenderUserId",
                table: "GroupChatMessages",
                column: "SenderUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GroupChatMessages");
        }
    }
}
