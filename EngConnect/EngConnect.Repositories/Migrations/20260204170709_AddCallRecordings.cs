using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EngConnect.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class AddCallRecordings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "recording_completed",
                table: "call_session",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "recording_file_path",
                table: "call_session",
                type: "character varying(1024)",
                maxLength: 1024,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "recording_completed",
                table: "call_session");

            migrationBuilder.DropColumn(
                name: "recording_file_path",
                table: "call_session");
        }
    }
}
