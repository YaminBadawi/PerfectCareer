using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PerfectCareer.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCandidateCvs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CandidateCvs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CandidateProfileId = table.Column<int>(type: "int", nullable: false),
                    PositionId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    PublishedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CandidateCvs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CandidateCvs_CandidateProfiles_CandidateProfileId",
                        column: x => x.CandidateProfileId,
                        principalTable: "CandidateProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CandidateCvs_Positions_PositionId",
                        column: x => x.PositionId,
                        principalTable: "Positions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CandidateCvProjects",
                columns: table => new
                {
                    CandidateCvId = table.Column<int>(type: "int", nullable: false),
                    CandidateProjectId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CandidateCvProjects", x => new { x.CandidateCvId, x.CandidateProjectId });
                    table.ForeignKey(
                        name: "FK_CandidateCvProjects_CandidateCvs_CandidateCvId",
                        column: x => x.CandidateCvId,
                        principalTable: "CandidateCvs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CandidateCvProjects_CandidateProjects_CandidateProjectId",
                        column: x => x.CandidateProjectId,
                        principalTable: "CandidateProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CandidateCvProjects_CandidateProjectId",
                table: "CandidateCvProjects",
                column: "CandidateProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateCvs_CandidateProfileId_PositionId",
                table: "CandidateCvs",
                columns: new[] { "CandidateProfileId", "PositionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CandidateCvs_PositionId",
                table: "CandidateCvs",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateCvs_Status_UpdatedAtUtc",
                table: "CandidateCvs",
                columns: new[] { "Status", "UpdatedAtUtc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CandidateCvProjects");

            migrationBuilder.DropTable(
                name: "CandidateCvs");
        }
    }
}
