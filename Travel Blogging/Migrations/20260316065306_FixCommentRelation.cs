using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Travel_Blogging.Migrations
{
    /// <inheritdoc />
    public partial class FixCommentRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Users_UserModelId",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_UserModelId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "UserModelId",
                table: "Comments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserModelId",
                table: "Comments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comments_UserModelId",
                table: "Comments",
                column: "UserModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Users_UserModelId",
                table: "Comments",
                column: "UserModelId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
