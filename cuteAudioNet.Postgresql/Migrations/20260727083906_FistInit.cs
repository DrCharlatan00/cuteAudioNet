using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace cuteAudioNet.Postgresql.Migrations
{
    /// <inheritdoc />
    public partial class FistInit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "artists",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    ArtistName = table.Column<string>(type: "text", nullable: false),
                    NickName = table.Column<string>(type: "text", nullable: false),
                    Surname = table.Column<string>(type: "text", nullable: true),
                    BornDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Pathonymic = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_artists", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "albums",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    AlbumName = table.Column<string>(type: "text", nullable: false),
                    DateRelease = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ArtistID = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_albums", x => x.ID);
                    table.ForeignKey(
                        name: "FK_albums_artists_ArtistID",
                        column: x => x.ArtistID,
                        principalTable: "artists",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tracks",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    AlbumID = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Genre = table.Column<int>(type: "integer", nullable: false),
                    TimeRelease = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SubArtist = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tracks", x => x.ID);
                    table.ForeignKey(
                        name: "FK_tracks_albums_AlbumID",
                        column: x => x.AlbumID,
                        principalTable: "albums",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_albums_ArtistID",
                table: "albums",
                column: "ArtistID");

            migrationBuilder.CreateIndex(
                name: "IX_tracks_AlbumID",
                table: "tracks",
                column: "AlbumID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tracks");

            migrationBuilder.DropTable(
                name: "albums");

            migrationBuilder.DropTable(
                name: "artists");
        }
    }
}
