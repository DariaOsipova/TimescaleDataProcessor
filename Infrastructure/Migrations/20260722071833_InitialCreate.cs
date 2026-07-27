using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable // отключаем проверку на null

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    { // позволяет разбить определение класса, структуры или метода на несколько частей в разных файлах
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ResultRecords",
                columns: table =>
                    new
                    {
                        Id = table.Column<Guid>(type: "uuid", nullable: false),
                        FileName = table.Column<string>(type: "character varying(255)", maxLength: 255,
                                                          nullable: false),
                        DeltaTimeSeconds = table.Column<double>(type: "double precision", precision: 18,
                                                                  scale: 6, nullable: false),
                        MinDate =
                              table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                        AvgExecutionTime = table.Column<double>(type: "double precision", precision: 18,
                                                                  scale: 6, nullable: false),
                        AvgValue = table.Column<double>(type: "double precision", precision: 18,
                                                          scale: 6, nullable: false),
                        MedianValue = table.Column<double>(type: "double precision", precision: 18,
                                                             scale: 6, nullable: false),
                        MaxValue = table.Column<double>(type: "double precision", precision: 18,
                                                          scale: 6, nullable: false),
                        MinValue = table.Column<double>(type: "double precision", precision: 18,
                                                          scale: 6, nullable: false),
                        ProcessedAt =
                              table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                    },
                constraints: table => { table.PrimaryKey("PK_ResultRecords", x => x.Id); });

            migrationBuilder.CreateTable(
                name: "ValueRecords",
                columns: table =>
                    new
                    {
                        Id = table.Column<Guid>(type: "uuid", nullable: false),
                        Date =
                              table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                        ExecutionTime = table.Column<double>(type: "double precision", precision: 18,
                                                               scale: 6, nullable: false),
                        Value = table.Column<double>(type: "double precision", precision: 18, scale: 6,
                                                       nullable: false),
                        FileName = table.Column<string>(type: "character varying(255)", maxLength: 255,
                                                          nullable: false)
                    },
                constraints: table => { table.PrimaryKey("PK_ValueRecords", x => x.Id); });

            migrationBuilder.CreateIndex(name: "IX_ResultRecords_AvgExecutionTime",
                                         table: "ResultRecords", column: "AvgExecutionTime");

            migrationBuilder.CreateIndex(name: "IX_ResultRecords_AvgValue", table: "ResultRecords",
                                         column: "AvgValue");

            migrationBuilder.CreateIndex(name: "IX_ResultRecords_FileName", table: "ResultRecords",
                                         column: "FileName", unique: true);

            migrationBuilder.CreateIndex(name: "IX_ResultRecords_MinDate", table: "ResultRecords",
                                         column: "MinDate");

            migrationBuilder.CreateIndex(name: "IX_ValueRecords_Date", table: "ValueRecords",
                                         column: "Date");

            migrationBuilder.CreateIndex(name: "IX_ValueRecords_FileName", table: "ValueRecords",
                                         column: "FileName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        { // override-переопределение метода
            migrationBuilder.DropTable(name: "ResultRecords");

            migrationBuilder.DropTable(name: "ValueRecords");
        }
    }
}
