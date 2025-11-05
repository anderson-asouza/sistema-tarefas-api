using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaTarefas.Migrations
{
    /// <inheritdoc />
    public partial class DB20251018 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MTRA_USU_ID_Responsavel",
                table: "ModelosTramite",
                newName: "MTRA_USU_ID_Revisor");

            migrationBuilder.RenameIndex(
                name: "IX_ModelosTramite_MTRA_USU_ID_Responsavel",
                table: "ModelosTramite",
                newName: "IX_ModelosTramite_MTRA_USU_ID_Revisor");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MTRA_USU_ID_Revisor",
                table: "ModelosTramite",
                newName: "MTRA_USU_ID_Responsavel");

            migrationBuilder.RenameIndex(
                name: "IX_ModelosTramite_MTRA_USU_ID_Revisor",
                table: "ModelosTramite",
                newName: "IX_ModelosTramite_MTRA_USU_ID_Responsavel");
        }
    }
}
