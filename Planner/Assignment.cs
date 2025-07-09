using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planner
{
   
    internal class Assignment
    {
        public int Id { get; set; }            
        public string Title { get; set; }     
        public int Status_id { get; set; }
        public Assignment(int id, string title, int status)
        {
            Id = id;
            Title = title;
            Status_id = status;
        }
    }
    
}
