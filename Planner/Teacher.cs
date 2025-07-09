using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planner
{
    internal class Teacher
    {
        public int Id { get; set; }                 
        public string Name { get; set; }

        public List<Subject> Subjects { get; set; } = new List<Subject>();

        public List<string> Groups { get; set; } = new List<string>(); 
             

        public Teacher(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
