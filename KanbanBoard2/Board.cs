using KanbanBoard2.WorkItems;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace KanbanBoard2
{
    public class Board
    {
        public int Id { get; set; }
        public Stage Ready { get; set; }
        public Stage Indevelopment { get; set; }
        public Stage Done { get; set; }
        public int ReadyWip { get; set; }
        public int IndevelopmentWip { get; set; }
        public int DoneWip { get; set; }

        public Board()
        {
            Ready = new Stage();
            Indevelopment = new Stage();
            Done = new Stage();

            ReadyWip = 0;             
            IndevelopmentWip = 0;
            DoneWip = 0;
        }

        public Board(int readyWip, int indevelopmentWip, int doneWip)
        {
            Ready = new Stage(readyWip);
            Indevelopment = new Stage(indevelopmentWip);
            Done = new Stage(doneWip);

            ReadyWip = readyWip;
            IndevelopmentWip = indevelopmentWip;
            DoneWip = doneWip;
        }

        public Board(int id, string ready, int readyWip, string indevelopment, int indevelopmentWip, string done, int doneWip)
        {
            Id = id;

            Ready = Stage.JsonDeserialize(ready, readyWip);
            Indevelopment = Stage.JsonDeserialize(indevelopment, indevelopmentWip);
            Done = Stage.JsonDeserialize(done, doneWip);

            ReadyWip = readyWip;
            IndevelopmentWip = indevelopmentWip;
            DoneWip = doneWip;
        }

        public void Create()
        {
            var readyJson = JsonConvert.SerializeObject(Ready);
            var indevelopmentJson = JsonConvert.SerializeObject(Indevelopment);
            var doneJson = JsonConvert.SerializeObject(Done);

            var id = Database.ExecuteCommand($"INSERT INTO boards(ready, readyWip, indevelopment, indevelopmentWip, done, doneWip) VALUES ('{readyJson}', {Ready.Capacity}, '{indevelopmentJson}', {Indevelopment.Capacity}, '{doneJson}', {Done.Capacity}) RETURNING id;");

            Id = int.Parse(id[0].ToString());
        }

        public static Board GetById(int id)
        {
            var reader = Database.ExecuteCommand($"SELECT ready, readyWip, indevelopment, indevelopmentWip, done, doneWip FROM boards WHERE id={id}");

            if (!reader.HasRows)
                throw new Exception($"Board with id={id} does not exist");

            return new Board(id, reader[0].ToString(), int.Parse(reader[1].ToString()), reader[2].ToString(), int.Parse(reader[3].ToString()), reader[4].ToString(), int.Parse(reader[5].ToString()));
        }

        public static Board JsonDeserialize(string json)
        {
            Board board = new Board();
            json = TrimJson(json);
            var o = JObject.Parse(json);

            board.Ready = Stage.JsonDeserialize(o["Ready"].ToString(), int.Parse(o["ReadyWip"].ToString()));
            board.Indevelopment = Stage.JsonDeserialize(o["Indevelopment"].ToString(), int.Parse(o["IndevelopmentWip"].ToString()));
            board.Done = Stage.JsonDeserialize(o["Done"].ToString(), int.Parse(o["DoneWip"].ToString()));

            return board;
        }

        private static string TrimJson(string json)
        {
            json = json.Replace("\\", "");
            json = json[0] == '"' ? json.Remove(0, 1) : json;
            json = json[json.Length - 1] == '"' ? json.Remove(json.Length - 1) : json;

            return json;
        }
    }
}