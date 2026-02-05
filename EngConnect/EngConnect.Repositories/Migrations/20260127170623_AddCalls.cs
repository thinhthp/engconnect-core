using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EngConnect.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class AddCalls : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "call_session",
                columns: table => new
                {
                    call_session_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    caller_id = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    callee_id = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    started_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    ended_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValueSql: "'pending'::character varying"),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    note = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    create_by = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    update_by = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    update_date = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("callsession_pkey", x => x.call_session_id);
                    table.ForeignKey(
                        name: "callsession_callee_id_fkey",
                        column: x => x.callee_id,
                        principalTable: "application_user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "callsession_caller_id_fkey",
                        column: x => x.caller_id,
                        principalTable: "application_user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "call_participant",
                columns: table => new
                {
                    participant_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    call_session_id = table.Column<long>(type: "bigint", nullable: false),
                    user_id = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    is_caller = table.Column<bool>(type: "boolean", nullable: false),
                    is_connected = table.Column<bool>(type: "boolean", nullable: false),
                    is_muted = table.Column<bool>(type: "boolean", nullable: false),
                    is_video_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    joined_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    left_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    note = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    create_by = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    update_by = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    update_date = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("callparticipant_pkey", x => x.participant_id);
                    table.ForeignKey(
                        name: "callparticipant_call_session_id_fkey",
                        column: x => x.call_session_id,
                        principalTable: "call_session",
                        principalColumn: "call_session_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "callparticipant_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "application_user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "call_signal",
                columns: table => new
                {
                    signal_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    call_session_id = table.Column<long>(type: "bigint", nullable: false),
                    sender_id = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    receiver_id = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    type = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    payload = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    note = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    create_by = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    update_by = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    update_date = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("callsignal_pkey", x => x.signal_id);
                    table.ForeignKey(
                        name: "callsignal_call_session_id_fkey",
                        column: x => x.call_session_id,
                        principalTable: "call_session",
                        principalColumn: "call_session_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "callsignal_receiver_id_fkey",
                        column: x => x.receiver_id,
                        principalTable: "application_user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "callsignal_sender_id_fkey",
                        column: x => x.sender_id,
                        principalTable: "application_user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_call_participant_user_id",
                table: "call_participant",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ux_call_participant_session_user",
                table: "call_participant",
                columns: new[] { "call_session_id", "user_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_call_session_callee_id",
                table: "call_session",
                column: "callee_id");

            migrationBuilder.CreateIndex(
                name: "IX_call_session_caller_id",
                table: "call_session",
                column: "caller_id");

            migrationBuilder.CreateIndex(
                name: "IX_call_signal_receiver_id",
                table: "call_signal",
                column: "receiver_id");

            migrationBuilder.CreateIndex(
                name: "IX_call_signal_sender_id",
                table: "call_signal",
                column: "sender_id");

            migrationBuilder.CreateIndex(
                name: "ix_call_signal_session_created",
                table: "call_signal",
                columns: new[] { "call_session_id", "created_at" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "call_participant");

            migrationBuilder.DropTable(
                name: "call_signal");

            migrationBuilder.DropTable(
                name: "call_session");
        }
    }
}
