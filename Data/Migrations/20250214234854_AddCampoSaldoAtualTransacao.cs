using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinhasFinancas.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCampoSaldoAtualTransacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DataProximaTransacao",
                table: "MF_TRANSACAO",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<decimal>(
                name: "SaldoAtual",
                table: "MF_TRANSACAO",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataProximaTransacao",
                table: "MF_TRANSACAO");

            migrationBuilder.DropColumn(
                name: "SaldoAtual",
                table: "MF_TRANSACAO");
        }
    }
}
