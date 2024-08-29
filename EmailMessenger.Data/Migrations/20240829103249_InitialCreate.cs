using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EmailMessenger.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "MAILS_API");

            migrationBuilder.CreateTable(
                name: "Messages",
                schema: "MAILS_API",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Subject = table.Column<string>(type: "text", nullable: false),
                    Body = table.Column<string>(type: "text", nullable: false),
                    Time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsPlanned = table.Column<bool>(type: "boolean", nullable: false),
                    IsSent = table.Column<bool>(type: "boolean", nullable: false),
                    HasErrors = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Recipients",
                schema: "MAILS_API",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Address = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recipients", x => x.Id);
                    table.UniqueConstraint("AK_Recipients_Address", x => x.Address);
                });

            migrationBuilder.CreateTable(
                name: "MessageEvents",
                schema: "MAILS_API",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MessageId = table.Column<Guid>(type: "uuid", nullable: false),
                    RecipientId = table.Column<long>(type: "bigint", nullable: false),
                    Time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Result = table.Column<string>(type: "text", nullable: false),
                    FailedMessage = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessageEvents_Messages_MessageId",
                        column: x => x.MessageId,
                        principalSchema: "MAILS_API",
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MessageEvents_Recipients_RecipientId",
                        column: x => x.RecipientId,
                        principalSchema: "MAILS_API",
                        principalTable: "Recipients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MessageRecipient",
                schema: "MAILS_API",
                columns: table => new
                {
                    MessagesId = table.Column<Guid>(type: "uuid", nullable: false),
                    RecipientsId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageRecipient", x => new { x.MessagesId, x.RecipientsId });
                    table.ForeignKey(
                        name: "FK_MessageRecipient_Messages_MessagesId",
                        column: x => x.MessagesId,
                        principalSchema: "MAILS_API",
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MessageRecipient_Recipients_RecipientsId",
                        column: x => x.RecipientsId,
                        principalSchema: "MAILS_API",
                        principalTable: "Recipients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MessageEvents_MessageId",
                schema: "MAILS_API",
                table: "MessageEvents",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageEvents_RecipientId",
                schema: "MAILS_API",
                table: "MessageEvents",
                column: "RecipientId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageRecipient_RecipientsId",
                schema: "MAILS_API",
                table: "MessageRecipient",
                column: "RecipientsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MessageEvents",
                schema: "MAILS_API");

            migrationBuilder.DropTable(
                name: "MessageRecipient",
                schema: "MAILS_API");

            migrationBuilder.DropTable(
                name: "Messages",
                schema: "MAILS_API");

            migrationBuilder.DropTable(
                name: "Recipients",
                schema: "MAILS_API");
        }
    }
}
