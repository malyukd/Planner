using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planner
{
    internal class ExamType
    {
            static public List<ExamType> ETypes { get; set; } = new List<ExamType>();
            public int Id { get; set; }
            public string Name { get; set; }
            public ExamType(int id, string title)
            {
                Id = id;
                Name = title;
            }
        
    }
}
