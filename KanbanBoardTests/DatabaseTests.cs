using System.Threading.Tasks;
using KanbanBoard2;
using KanbanBoard2.Controllers;
using Microsoft.Extensions.Logging;
using Npgsql;
using NUnit.Framework;

namespace KanbanBoardTests
{
    public class DatabaseTests
    {
        [Test]
        public void ShouldBeAbleToInsertRowsIntoTables()
        {
            var reader = Database.ExecuteCommand("SELECT COUNT(id) FROM stories;");
            var expectedStoriesCount = int.Parse(reader[0].ToString()) + 1;
            reader.Close();

            Database.ExecuteCommand("INSERT INTO stories(name, creator, description) VALUES ('database test', 'database', 'testing database')");

            reader = Database.ExecuteCommand("SELECT COUNT(id) FROM stories;");
            var actualStoriesCount = int.Parse(reader[0].ToString());
            Assert.AreEqual(expectedStoriesCount, actualStoriesCount);
        }
    }
}