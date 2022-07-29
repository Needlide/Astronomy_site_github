using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVC_app_main.Migrations
{
    public partial class NewNASAImagesModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "data",
                table: "NASAImages");

            migrationBuilder.DropColumn(
                name: "links",
                table: "NASAImages");

            migrationBuilder.AlterColumn<string>(
                name: "href",
                table: "NASAImages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "center",
                table: "NASAImages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "date_created",
                table: "NASAImages",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "NASAImages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "description_508",
                table: "NASAImages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "keywords",
                table: "NASAImages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "media_type",
                table: "NASAImages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "nasa_id",
                table: "NASAImages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "secondary_creator",
                table: "NASAImages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "title",
                table: "NASAImages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "center",
                table: "NASAImages");

            migrationBuilder.DropColumn(
                name: "date_created",
                table: "NASAImages");

            migrationBuilder.DropColumn(
                name: "description",
                table: "NASAImages");

            migrationBuilder.DropColumn(
                name: "description_508",
                table: "NASAImages");

            migrationBuilder.DropColumn(
                name: "keywords",
                table: "NASAImages");

            migrationBuilder.DropColumn(
                name: "media_type",
                table: "NASAImages");

            migrationBuilder.DropColumn(
                name: "nasa_id",
                table: "NASAImages");

            migrationBuilder.DropColumn(
                name: "secondary_creator",
                table: "NASAImages");

            migrationBuilder.DropColumn(
                name: "title",
                table: "NASAImages");

            migrationBuilder.AlterColumn<string>(
                name: "href",
                table: "NASAImages",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "data",
                table: "NASAImages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "links",
                table: "NASAImages",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
