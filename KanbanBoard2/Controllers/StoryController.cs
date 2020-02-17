using System;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace KanbanBoard2.Controllers
{
    [Route("story")]
    public class StoryController : ControllerBase
    {
        [Route("{id}")]
        public IActionResult Get(int id)
        {
            Story story;
            try
            {
                story = Story.GetById(id);
            }
            catch(Exception e)
            {
                return NotFound(e.Message);
            }

            var storyJson = JsonConvert.SerializeObject(story);
            return Ok(storyJson);
        }

        [Route("")]
        public async Task<IActionResult> Post([FromBody] string json)
        {
            Story story;
            try
            {
                story = JsonConvert.DeserializeObject<Story>(json);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            
            var validation = Story.IsValid(story);
            if (!validation.IsValid)
                return BadRequest(validation.Errors);
            
            story.Create();
            return Ok();
        }
    }
}