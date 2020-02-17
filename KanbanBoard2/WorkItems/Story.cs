using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using KanbanBoard2.Validators;
using Newtonsoft.Json;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace KanbanBoard2
{
    public class Story : IWorkItem
    {
        public int Id { get; private set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Creator { get; private set; }
        public int Type { get; private set; }
        
        [JsonConstructor]
        public Story(){}
        
        public Story(string name, string creator, string description)
        {
            Name = name;
            Creator = creator;
            Description = description;
            Type = 1;
        }
        
        private Story(int id, string name, string creator, string description)
        {
            Id = id;
            Name = name;
            Creator = creator;
            Description = description;
            Type = 1;
        }
        
        public void Create()
        {
            var reader = Database.ExecuteCommand($"INSERT INTO stories(name, creator, description) VALUES ('{Name}', '{Creator}', '{Description}') RETURNING id;");
            Id = int.Parse(reader[0].ToString());
        }

        public void Edit()
        {
            Database.ExecuteCommand(
                $"UPDATE stories SET name = '{Name}', creator = '{Creator}', description = '{Description}'");
        }

        public static Story GetById(int id)
        {
            var reader = Database.ExecuteCommand($"SELECT name, creator, description FROM stories WHERE id = {id}");
            return reader.HasRows 
                   ? new Story(id, reader[0].ToString(), reader[1].ToString(), reader[2].ToString())
                   : throw new Exception(id + " is not a valid Id");
        }

        public static ValidationResult IsValid(Story story)
        {
            StoryValidator validator = new StoryValidator();
            return validator.Validate(story);
        }
    }
}