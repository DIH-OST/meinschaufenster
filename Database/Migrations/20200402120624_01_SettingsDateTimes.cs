// DigitalesSchaufenster (C) 2020 DIH-OST

using System;

namespace Database.Migrations
{
    public partial class _01_SettingsDateTimes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                "From",
                "VirtualWorkTime",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                "IsMigrated",
                "VirtualWorkTime",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                "To",
                "VirtualWorkTime",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                "From",
                "OpeningHours",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                "IsMigrated",
                "OpeningHours",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                "To",
                "OpeningHours",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                "AppointmentDate",
                "Appointment",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                "Duration",
                "Appointment",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                "IsMigrated",
                "Appointment",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                "Time",
                "Appointment",
                nullable: true);

            migrationBuilder.CreateTable(
                "DynamicSetting",
                table => new
                         {
                             Id = table.Column<int>(nullable: false)
                                 .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                             Key = table.Column<string>(nullable: true),
                             Value = table.Column<string>(nullable: true)
                         },
                constraints: table => { table.PrimaryKey("PK_DynamicSetting", x => x.Id); });

            migrationBuilder.CreateTable(
                "StaticSetting",
                table => new
                         {
                             Id = table.Column<int>(nullable: false)
                                 .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                             EMailsToInform = table.Column<string>(nullable: true),
                             MaintenanceInfo = table.Column<string>(nullable: true)
                         },
                constraints: table => { table.PrimaryKey("PK_StaticSetting", x => x.Id); });

            // Zusätzliches SQL Script zum füllen der neuen Spalten
            migrationBuilder.Sql("UPDATE Appointment SET AppointmentDate = CONVERT(date,  ValidFrom );");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "DynamicSetting");

            migrationBuilder.DropTable(
                "StaticSetting");

            migrationBuilder.DropColumn(
                "From",
                "VirtualWorkTime");

            migrationBuilder.DropColumn(
                "IsMigrated",
                "VirtualWorkTime");

            migrationBuilder.DropColumn(
                "To",
                "VirtualWorkTime");

            migrationBuilder.DropColumn(
                "From",
                "OpeningHours");

            migrationBuilder.DropColumn(
                "IsMigrated",
                "OpeningHours");

            migrationBuilder.DropColumn(
                "To",
                "OpeningHours");

            migrationBuilder.DropColumn(
                "AppointmentDate",
                "Appointment");

            migrationBuilder.DropColumn(
                "Duration",
                "Appointment");

            migrationBuilder.DropColumn(
                "IsMigrated",
                "Appointment");

            migrationBuilder.DropColumn(
                "Time",
                "Appointment");
        }
    }
}