using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CareActionTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CareActionTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Plants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Species = table.Column<string>(type: "TEXT", nullable: false),
                    Location = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    PlantStatusId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CareActions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ExecutedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Executor = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Parameters = table.Column<string>(type: "TEXT", nullable: false),
                    TriggeredBy = table.Column<string>(type: "TEXT", nullable: false),
                    PlantId = table.Column<int>(type: "INTEGER", nullable: false),
                    CareActionTypeId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CareActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CareActions_CareActionTypes_CareActionTypeId",
                        column: x => x.CareActionTypeId,
                        principalTable: "CareActionTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CareActions_Plants_PlantId",
                        column: x => x.PlantId,
                        principalTable: "Plants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Observations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Timestamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ObservationType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Source = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    DataPayload = table.Column<string>(type: "TEXT", nullable: false),
                    MediaUrl = table.Column<string>(type: "TEXT", nullable: false),
                    PlantId = table.Column<int>(type: "INTEGER", nullable: false),
                    HealthAssessmentId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Observations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Observations_Plants_PlantId",
                        column: x => x.PlantId,
                        principalTable: "Plants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HealthAssessments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AssessmentSource = table.Column<string>(type: "TEXT", nullable: false),
                    HealthStatus = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    HealthScore = table.Column<decimal>(type: "TEXT", nullable: true),
                    DetectedIssues = table.Column<string>(type: "TEXT", nullable: false),
                    ConfidenceLevel = table.Column<decimal>(type: "TEXT", nullable: true),
                    Recommendations = table.Column<string>(type: "TEXT", nullable: false),
                    ObservationId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HealthAssessments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HealthAssessments_Observations_ObservationId",
                        column: x => x.ObservationId,
                        principalTable: "Observations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CareActions_CareActionTypeId",
                table: "CareActions",
                column: "CareActionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CareActions_PlantId",
                table: "CareActions",
                column: "PlantId");

            migrationBuilder.CreateIndex(
                name: "IX_HealthAssessments_ObservationId",
                table: "HealthAssessments",
                column: "ObservationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Observations_PlantId",
                table: "Observations",
                column: "PlantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CareActions");

            migrationBuilder.DropTable(
                name: "HealthAssessments");

            migrationBuilder.DropTable(
                name: "CareActionTypes");

            migrationBuilder.DropTable(
                name: "Observations");

            migrationBuilder.DropTable(
                name: "Plants");
        }
    }
}
