using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planner
{
    internal class Subject
    {
        static public List<Subject> Subjects { get; set; } = new List<Subject>();   
        public int Id { get; set; }               
        public string Title { get; set; }       
        public Subject(int id, string title)
        {
            Id = id;
            Title = title;
           
        }
    }
}
