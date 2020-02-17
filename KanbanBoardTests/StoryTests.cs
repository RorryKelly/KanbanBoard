using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using KanbanBoard2;
using Newtonsoft.Json;
using Npgsql;
using NUnit.Framework;

namespace KanbanBoardTests
{
    public class StoryTests : HttpFixture
    {
        [Test]
        public void ShouldBeAbleToInsertStoryIntoDatabase()
        {
            var expectedStoryCount = GetStoryCount() + 1;

            var story = new Story("story name", "rorry", "testing ability to insert story to database");
            story.Create();

            var actualStoryCount = GetStoryCount();
            Assert.AreEqual(expectedStoryCount, actualStoryCount);
        }

        [Test]
        public void ShouldBeAbleToEditStoryInDatabase()
        {
            var story = new Story("story name", "rorry", "testing ability to edit storys");
            story.Create();

            story.Name = "NewName";
            story.Edit();

            var reader = Database.ExecuteCommand($"SELECT name FROM stories WHERE id={story.Id};");
            Assert.AreEqual(story.Name, reader[0].ToString());
        }

        [Test]
        public void ShouldBeAbleToGetStoryById()
        {
            var expectedResult = new Story("teststory", "rorry", "testing ability to get story by Id");
            expectedResult.Create();

            var actualResult = Story.GetById(expectedResult.Id);
            
            Assert.AreEqual(expectedResult.Id, actualResult.Id);
            Assert.AreEqual(expectedResult.Name, actualResult.Name);
            Assert.AreEqual(expectedResult.Creator, actualResult.Creator);
            Assert.AreEqual(expectedResult.Description, actualResult.Description);
        }

        [Test]
        public async Task ShouldBeAbleToGetStoryThroughApi()
        {
            var story = new Story("storyname", "rorry", "testing ability to GET story through api");
            story.Create();
            var expectedResult = "\"" + JsonConvert.SerializeObject(story) + "\"";
            
            var request = new HttpRequestMessage(HttpMethod.Get, $"story/{story.Id}");
            var response = await Client.SendAsync(request);

            var actualResult = await response.Content.ReadAsStringAsync();
            actualResult = actualResult.Replace(@"\", "");
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public async Task ShouldReturn404WhenStoryIdIsNotInDatabase()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"story/9052424");
            var response = await Client.SendAsync(request);
            
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Test]
        public async Task ShouldBeAbleToCreateStoryThroughApi()
        {
            var expectedStoryCount = GetStoryCount() + 1;
            var story = new Story("StoryName", "Rorry", "testing ability to POST story through api");
            var json = JsonConvert.SerializeObject(story);
            
            var request = new HttpRequestMessage(HttpMethod.Post, "story");
            request.Content = new StringContent(JsonConvert.SerializeObject(json), Encoding.UTF8, "application/json");
            var response = await Client.SendAsync(request);

            var actualStoryCount = GetStoryCount();
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(expectedStoryCount, actualStoryCount);
        }

        [Test]
        public async Task ShouldReturn400WhenRequestHasNonJsonBody()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "story");
            request.Content = new StringContent("BAD REQUEST", Encoding.UTF8, "application/json");
            var response = await Client.SendAsync(request);
            
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Test]
        public async Task ShouldReturn400WhenEmptyNameField()
        {
            var story = new Story(string.Empty, "Rorry", "testing ability to POST story through api");
            var json = JsonConvert.SerializeObject(story);
            
            var request = new HttpRequestMessage(HttpMethod.Post, "story");
            request.Content = new StringContent(JsonConvert.SerializeObject(json), Encoding.UTF8, "application/json");
            var response = await Client.SendAsync(request);
            
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }
        
        private static int GetStoryCount()
        {
            var reader = Database.ExecuteCommand("SELECT COUNT(id) FROM stories;");
            var storyCount = int.Parse(reader[0].ToString());
            reader.Close();
            
            return storyCount;
        }
    }
}