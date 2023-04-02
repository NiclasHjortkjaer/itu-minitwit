using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniTwit.Migrations
{
    /// <inheritdoc />
    public partial class publishdate_index : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Messages_PublishDate",
                table: "Messages",
                column: "PublishDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Messages_PublishDate",
                table: "Messages");
        }
    }
}
