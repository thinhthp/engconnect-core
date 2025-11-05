using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EngConnect.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class AddChats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "chat_thread",
                columns: table => new
                {
                    thread_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    is_group = table.Column<bool>(type: "boolean", nullable: false),
                    note = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    update_date = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    create_by = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    update_by = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("chatthread_pkey", x => x.thread_id);
                });

            migrationBuilder.CreateTable(
                name: "chat_message",
                columns: table => new
                {
                    message_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    thread_id = table.Column<int>(type: "integer", nullable: false),
                    sender_id = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    content = table.Column<string>(type: "text", nullable: false),
                    content_type = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    is_edited = table.Column<bool>(type: "boolean", nullable: false),
                    note = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    update_date = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    create_by = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    update_by = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("chatmessage_pkey", x => x.message_id);
                    table.ForeignKey(
                        name: "chatmessage_sender_id_fkey",
                        column: x => x.sender_id,
                        principalTable: "application_user",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "chatmessage_thread_id_fkey",
                        column: x => x.thread_id,
                        principalTable: "chat_thread",
                        principalColumn: "thread_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "chat_participant",
                columns: table => new
                {
                    participant_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    thread_id = table.Column<int>(type: "integer", nullable: false),
                    user_id = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    joined_at = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    last_read_at = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    is_muted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("chatparticipant_pkey", x => x.participant_id);
                    table.ForeignKey(
                        name: "chatparticipant_thread_id_fkey",
                        column: x => x.thread_id,
                        principalTable: "chat_thread",
                        principalColumn: "thread_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "chatparticipant_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "application_user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_chat_message_sender_id",
                table: "chat_message",
                column: "sender_id");

            migrationBuilder.CreateIndex(
                name: "ix_chat_message_thread_created",
                table: "chat_message",
                columns: new[] { "thread_id", "created_at" });

            migrationBuilder.CreateIndex(
                name: "IX_chat_participant_user_id",
                table: "chat_participant",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ux_chat_participant_thread_user",
                table: "chat_participant",
                columns: new[] { "thread_id", "user_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "chat_message");

            migrationBuilder.DropTable(
                name: "chat_participant");

            migrationBuilder.DropTable(
                name: "chat_thread");
        }
    }
}
