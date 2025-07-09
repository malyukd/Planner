using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planner
{
    internal class Building
    {
        static public List<Building> Buildings { get; set; } = new List<Building>();
        public int Id { get; set; }
        public string Address { get; set; }
        public Building(int id, string title)
        {
            Id = id;
            Address = title;
        }
    }
}
