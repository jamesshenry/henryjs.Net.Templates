using FluentMigrator;

namespace CAFConsole.Data.Migrations;

[Migration(1)]
public class CreateTaskTable : Migration
{
    public override void Up()
    {
        var table = Create.Table("Tasks").WithColumn("Id").AsInt32().PrimaryKey().Identity();
        table.WithColumn("Title").AsString().NotNullable();
        table.WithColumn("IsComplete").AsBoolean().WithDefaultValue(false);
        table.WithColumn("CreatedAt").AsDateTime().WithDefaultValue(SystemMethods.CurrentDateTime);
    }

    public override void Down() => Delete.Table("Tasks");
}
