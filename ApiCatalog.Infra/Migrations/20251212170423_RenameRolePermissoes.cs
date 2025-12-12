using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiCatalog.Infra.Migrations
{
    /// <inheritdoc />
    public partial class RenameRolePermissoes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RoleLevel",
                table: "Users",
                newName: "RolePermissoes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RolePermissoes",
                table: "Users",
                newName: "RoleLevel");
        }
    }
}
