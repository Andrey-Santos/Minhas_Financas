using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinhasFinancas.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemovidoNumeroConta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumeroConta",
                table: "MF_TRANSACAO");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NumeroConta",
                table: "MF_TRANSACAO",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
