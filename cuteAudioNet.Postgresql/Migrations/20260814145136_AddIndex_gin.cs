using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace cuteAudioNet.Postgresql.Migrations
{
    /// <inheritdoc />
    public partial class AddIndex_gin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_tracks_Name",
                table: "tracks",
                column: "Name")
                .Annotation("Npgsql:IndexMethod", "gin")
                .Annotation("Npgsql:IndexOperators", new[] { "gin_trgm_ops" });

            migrationBuilder.CreateIndex(
                name: "IX_albums_AlbumName",
                table: "albums",
                column: "AlbumName")
                .Annotation("Npgsql:IndexMethod", "gin")
                .Annotation("Npgsql:IndexOperators", new[] { "gin_trgm_ops" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_tracks_Name",
                table: "tracks");

            migrationBuilder.DropIndex(
                name: "IX_albums_AlbumName",
                table: "albums");
        }
    }
}
