using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planner
{
    internal class Note
    {
        public int Id { get; set; }               
        public string Title { get; set; }       
        public string Text { get; set; }          
        public string Date { get; set; }       
        public int Subject_id { get; set; }      
        public bool FromDB { get; set; }

        public Note(int id, string title, string text, string date, int subject, bool fromDB)
        {
            Id = id;
            Title = title;
            Text = text;
            Date = date;
            Subject_id = subject;
            FromDB = fromDB;
        }
    }
}
