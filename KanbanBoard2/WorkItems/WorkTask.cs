using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KanbanBoard2.WorkItems
{
    public class WorkTask
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<WorkTask> Subtasks { get; private set; }
        public int ParentId { get; set; }
        public string CreatedBy { get; set; }
        public int Type { get; }

        public WorkTask()
        {
            Subtasks = new List<WorkTask>();
            Type = 2;
        }

        public WorkTask(string name, string description, string subtasks, int parentId, string createdby)
        {
            Name = name;
            Description = description;
            Subtasks = subtasks == String.Empty ? new List<WorkTask>() : JsonConvert.DeserializeObject<List<WorkTask>>(subtasks);
            ParentId = parentId;
            CreatedBy = createdby;
            Type = 2;
        }

        public WorkTask(int id, string name, string description, string subtasks, int parentId, string createdby)
        {
            Id = id;
            Name = name;
            Description = description;
            Subtasks = subtasks == String.Empty ? new List<WorkTask>() : JsonConvert.DeserializeObject<List<WorkTask>>(subtasks);
            ParentId = parentId;
            CreatedBy = createdby;
            Type = 2;
        }

        public void Create()
        {
            var subtasks = GetSubtasksJson();
            var reader = Database.ExecuteCommand($"INSERT INTO tasks(name, creator, description, subtasks, parent) VALUES ('{Name}', '{CreatedBy}', '{Description}', '{subtasks}', '{ParentId}') RETURNING id;");
            Id = int.Parse(reader[0].ToString());
        }

        private string GetSubtasksJson()
        {
            return JsonConvert.SerializeObject(Subtasks);
        }

        public void Edit()
        {
            var subtasks = GetSubtasksJson();
            Database.ExecuteCommand($"UPDATE tasks SET name = '{Name}', description = '{Description}', creator = '{CreatedBy}', subtasks = '{subtasks}', parent='{ParentId}' WHERE id = {Id}");
        }

        public static WorkTask GetById(int id)
        {
            var reader = Database.ExecuteCommand($"SELECT id, name, creator, description, subtasks, parent FROM tasks WHERE id = {id}");

            if (!reader.HasRows)
                throw new Exception($"No task with id={id} exists");

            return new WorkTask(int.Parse(reader[0].ToString()), reader[1].ToString(), reader[3].ToString(),
                reader[4].ToString(), int.Parse(reader[5].ToString()), reader[2].ToString());
        }

        public void AddSubTask(WorkTask subtask)
        {
            Subtasks.Add(subtask);
            subtask.ParentId = Id;
            subtask.Edit();
            Edit();
        }
    }
}
