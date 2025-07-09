using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planner
{
    internal class Status
    {
        static public List<Status> Statuses { get; set; } = new List<Status>();
        public int Id { get; set; }
        public string Name { get; set; }
        public Status(int id, string title)
        {
            Id = id;
            Name = title;
        }
    }
}
