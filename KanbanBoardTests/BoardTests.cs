using KanbanBoard2;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace KanbanBoardTests
{
    public class BoardTests : HttpFixture
    {
        [Test]
        public void ShouldBeAbleToAddWorkItemToBoard()
        {
            var board = new Board();
            var story = new Story("name", "rorry", "description");

            board.Ready.Add(story);

            Assert.AreEqual(1, board.Ready.Count);
        }

        [Test]
        public void ShouldBeAbleToSetAWipLimit([Values(3, 5, 10)] int wipLimit)
        {
            var board = new Board(wipLimit, wipLimit, wipLimit);

            for(int i = 0; i != wipLimit + 3; i++)
            {
                var story = new Story(i.ToString(), "rorry", "Test");
                board.Ready.Add(story);
            }

            Assert.AreEqual(wipLimit, board.Ready.Count);
        }

        [Test]
        public void ShouldBeAbleToInsertBoardToDatabase()
        {
            var reader = Database.ExecuteCommand("SELECT COUNT(id) FROM boards;");
            var expectedResult = int.Parse(reader[0].ToString()) + 1;
            reader.Close();

            var board = new Board();
            board.Create();

            reader = Database.ExecuteCommand("SELECT COUNT(id) FROM boards;");
            var actualResult = int.Parse(reader[0].ToString());
            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void ShouldBeAbleToGetBoardById()
        {
            var expectedResult = new Board();
            expectedResult.Create();

            var actualResult = Board.GetById(expectedResult.Id);

            Assert.AreEqual(JsonConvert.SerializeObject(expectedResult), JsonConvert.SerializeObject(actualResult));
        }

        [Test]
        public async Task ShouldBeAbleToGetBoardThroughApi()
        {
            var board = new Board();
            board.Create();
            var expectedResult = JsonConvert.SerializeObject(board);

            var request = new HttpRequestMessage(HttpMethod.Get, $"/board/{board.Id}");
            var response = await Client.SendAsync(request);

            var actualResult = await response.Content.ReadAsStringAsync();
            Assert.AreEqual(JsonConvert.SerializeObject(expectedResult), actualResult);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [Test]
        public async Task ShouldBeAbleToInsertBoardToDatabaseThroughApi()
        {
            var reader = Database.ExecuteCommand("SELECT COUNT(id) FROM boards;");
            var expectedResult = int.Parse(reader[0].ToString()) + 1;
            reader.Close();
            var board = new Board();
            var json = JsonConvert.SerializeObject(board);

            var request = new HttpRequestMessage(HttpMethod.Post, "board");
            request.Content = new StringContent(JsonConvert.SerializeObject(json), Encoding.UTF8, "application/json");
            var response = await Client.SendAsync(request);

            reader = Database.ExecuteCommand("SELECT COUNT(id) FROM boards;");
            var actualResult = int.Parse(reader[0].ToString());
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public async Task ShouldReturn400WhenGettingBoardThatDoesntExist()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/board/041432542");
            var response = await Client.SendAsync(request);

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}