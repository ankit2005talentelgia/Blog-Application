using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Travel_Blogging.Migrations
{
    /// <inheritdoc />
    public partial class AddPublicImgIdOfCloudinary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PublicImgId",
                table: "Posts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PublicImgId",
                table: "Posts");
        }
    }
}
