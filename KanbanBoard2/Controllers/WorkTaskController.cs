using KanbanBoard2.WorkItems;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KanbanBoard2.Controllers
{
    [Route("task")]
    public class WorkTaskController : ControllerBase
    {
        [Route("{id}")]
        public IActionResult Get(int id)
        {
            WorkTask task;
            try
            {
                task = WorkTask.GetById(id);
            }
            catch(Exception e)
            {
                return NotFound(e.Message);
            }

            return Ok(JsonConvert.SerializeObject(task));
        }

        [Route("")]
        public IActionResult Post([FromBody] string json)
        {
            try
            {
                var task = JsonConvert.DeserializeObject<WorkTask>(json);
                task.Create();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok();
        }
    }
}
