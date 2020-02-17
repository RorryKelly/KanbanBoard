using System;
using System.Threading.Tasks;

namespace KanbanBoard2
{
    public interface IWorkItem
    {
        public int Id { get; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Creator { get; }
        public int Type { get; }

        public void Create();
        public void Edit(); 
    }
}