using KanbanBoard2;
using KanbanBoard2.WorkItems;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace KanbanBoardTests
{
    class WorkTaskTests : HttpFixture
    {
        [Test]
        public void ShouldBeAbleToInsertTaskIntoDatabase()
        {
            var reader = Database.ExecuteCommand("SELECT count(id) FROM tasks");
            var expectedResult = int.Parse(reader[0].ToString()) + 1;

            var task = new WorkTask();
            task.Create();

            reader = Database.ExecuteCommand("SELECT count(id) FROM tasks");
            var actualResult = int.Parse(reader[0].ToString());
            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void ShouldBeAbleToEditTaskInDatabase()
        {
            var task = new WorkTask("worktasktest", "testt task", String.Empty, 0, "gad");
            task.Create();

            task.Name = "newname";
            task.Edit();

            var reader = Database.ExecuteCommand($"SELECT name FROM tasks WHERE id={task.Id}");
            Assert.AreEqual(task.Name, reader[0].ToString());
        }

        [Test]
        public async Task ShouldBeAbleToGetJsonTaskThroughApi()
        {
            var task = new WorkTask("TestWorkTask", "test task", String.Empty, 0, "gad");
            task.Create();
            var expectedResult = "\"" + JsonConvert.SerializeObject(task) + "\"";

            var request = new HttpRequestMessage(HttpMethod.Get, $"/task/{task.Id}");
            var response = await Client.SendAsync(request);

            var actualResult = await response.Content.ReadAsStringAsync();
            actualResult = actualResult.Replace(@"\", "");
            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public async Task ShouldBeAbleToCreateTaskThroughApi()
        {
            var task = new WorkTask("testwork", "testing apis", string.Empty, 0, "rorry");
            var json = JsonConvert.SerializeObject(task);
            var reader = Database.ExecuteCommand("SELECT count(id) FROM tasks");
            var expectedResult = int.Parse(reader[0].ToString()) + 1;

            var request = new HttpRequestMessage(HttpMethod.Post, "/task/");
            request.Content = new StringContent(JsonConvert.SerializeObject(json), Encoding.UTF8, "application/json");
            await Client.SendAsync(request);

            reader = Database.ExecuteCommand("SELECT count(id) FROM tasks");
            var actualResult = int.Parse(reader[0].ToString());
            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void ShouldBeAbleToAddSubtasks()
        {
            var task = new WorkTask("parenttask", "desc", String.Empty, -1, "he");
            var subtask = new WorkTask("subtask", "desc", String.Empty, -1, "he");

            task.AddSubTask(subtask);

            Assert.AreEqual("[" + JsonConvert.SerializeObject(subtask) + "]", JsonConvert.SerializeObject(task.Subtasks));
        }

        [Test]
        public async Task ShouldReturn400WhenPostingIncorrectJson()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "/task/");
            request.Content = new StringContent("NotJson", Encoding.UTF8, "application/json");
            var response = await Client.SendAsync(request);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Test]
        public async Task ShouldReturn404WhenWorkTaskIdDoesNotExist()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/task/543535335");
            var response = await Client.SendAsync(request);

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
