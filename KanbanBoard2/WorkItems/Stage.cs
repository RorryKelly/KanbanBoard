using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KanbanBoard2.WorkItems
{
    public class Stage : List<IWorkItem>
    {
        public Stage() : base()
        {
        }

        public Stage(int capacity) : base(capacity)
        {
        }

        public void Add(IWorkItem item)
        {
            if(Capacity != Count || Capacity == 0)
                base.Add(item);
        }

        internal static Stage JsonDeserialize(string stageJson, int stageWip)
        {
            var o = JArray.Parse(stageJson);
            Stage stage = new Stage(stageWip);

            foreach(var item in o)
            {
                if ((int)item["Type"] == 1)
                    stage.Add(JsonConvert.DeserializeObject<Story>(item.ToString()));
            }

            return stage;
        }
    }
}
