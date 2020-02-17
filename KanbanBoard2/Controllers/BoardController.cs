using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KanbanBoard2.Controllers
{
    [Route("board")]
    public class BoardController : ControllerBase
    {
        [Route("{id}")]
        public IActionResult Get(int id)
        {
            Board board = new Board();
            try
            {
                board = Board.GetById(id);
            }
            catch(Exception e)
            {
                return NotFound(e.Message);
            }
            return Ok(JsonConvert.SerializeObject(board));
        }

        [Route("")]
        public IActionResult Post([FromBody] string json)
        {
            var board = Board.JsonDeserialize(json);
            board.Create();
            return Ok();
        }
    }
}
