using AetherSystem.OperationReport.DataSources;
using AetherSystem.OperationReport.DataSources.Schema;

namespace OperationReport.Test.DataSources;

public class ReferenceSourceInfoTest
{
    [Fact]
    public void Constructor_ShouldInitializesCorrectly()
    {
        var table = new Table("table", [
            new Column("id", ColumnType.Integer),
            new Column("label", ColumnType.Text),
        ]);
        
        var value = new ReferenceSourceInfo("data.db", FileType.Sqlite, table, table.Columns[0], table.Columns[1]);
        Assert.Equal(table.Columns[0], value.IdColumn);
        Assert.Equal(table.Columns[1], value.LabelColumn);
        Assert.Equal(table, value.Table);
    }

    [Theory]
    [InlineData("id", ColumnType.Text, "label", ColumnType.Text)]
    [InlineData("id", ColumnType.Integer, "label", ColumnType.Integer)]
    [InlineData("index", ColumnType.Integer, "label", ColumnType.Text)]
    [InlineData("id", ColumnType.Integer, "name", ColumnType.Text)]
    public void Constructor_ShouldThrowsIfColumnNotExists(
        string idColumnName, 
        ColumnType idColumnType,
        string labelColumnName, 
        ColumnType labelColumnType)
    {
        var table = new Table("table", [
            new Column("id", ColumnType.Integer),
            new Column("label", ColumnType.Text),
        ]);
        
        var idColumn = new Column(idColumnName, idColumnType);
        var labelColumn = new Column(labelColumnName, labelColumnType);
        
        Assert.Throws<ArgumentException>(() => new ReferenceSourceInfo("data.db", FileType.Sqlite, table, idColumn, labelColumn));
    }
    
    [Fact]
    public void IdColumnIndex_IsIndexOfColumn()
    {
        var table = new Table("table", [
            new Column("id", ColumnType.Integer),
            new Column("label", ColumnType.Text),
        ]);

        var value = new ReferenceSourceInfo("data.db", FileType.Sqlite, table, table.Columns[0], table.Columns[1]);
        Assert.Equal(0, value.IdColumnIndex);
        Assert.Equal(table.IndexOf(value.IdColumn), value.IdColumnIndex);
        Assert.Equal(table.IndexOf(table.Columns[0]), value.IdColumnIndex);
        Assert.Equal(table.IndexOf(new Column("id", ColumnType.Integer)), value.IdColumnIndex);
        Assert.Equal(table.IndexOf(new DateTimeColumn("id", ColumnType.Integer, DateTimeResolution.Seconds)), value.IdColumnIndex);
    }
    
    [Fact]
    public void LabelColumnIndex_IsIndexOfColumn()
    {
        var table = new Table("table", [
            new Column("id", ColumnType.Integer),
            new Column("label", ColumnType.Text),
        ]);

        var value = new ReferenceSourceInfo("data.db", FileType.Sqlite, table, table.Columns[0], table.Columns[1]);
        Assert.Equal(1, value.LabelColumnIndex);
        Assert.Equal(table.IndexOf(value.LabelColumn), value.LabelColumnIndex);
        Assert.Equal(table.IndexOf(table.Columns[1]), value.LabelColumnIndex);
        Assert.Equal(table.IndexOf(new Column("label", ColumnType.Text)), value.LabelColumnIndex);
        Assert.Equal(table.IndexOf(new DateTimeColumn("label", ColumnType.Text)), value.LabelColumnIndex);
    }
}