using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planner
{
    internal class Student
    {
        public int Id { get; set; }                 
        public string Name { get; set; }            
        public string Group { get; set; }           

        public List<Assignment> Assignments { get; set; } = new List<Assignment>(); 
        public BindingList<Note> Notes { get; set; } = new BindingList<Note>();               

        public Student(int id, string name, string group)
        {
            Id = id;
            Name = name;
            Group = group;
        }
        public void AddNote(Note note)
        {
            if (note != null)
                Notes.Add(note);
        }


        public bool RemoveNoteById(int noteId)
        {
            var note = Notes.FirstOrDefault(n => n.Id == noteId);
            if (note != null)
            {
                Notes.Remove(note);
                return true;
            }
            return false;
        }


        public bool RemoveNoteAt(int index)
        {
            if (index >= 0 && index < Notes.Count)
            {
                Notes.RemoveAt(index);
                return true;
            }
            return false;
        }


        public bool UpdateNote(Note updatedNote)
        {
            if (updatedNote == null) return false;

            var existingNote = Notes.FirstOrDefault(n => n.Id == updatedNote.Id);
            if (existingNote != null)
            {
                existingNote.Title = updatedNote.Title;
                existingNote.Text = updatedNote.Text;
                existingNote.Date = updatedNote.Date;
                existingNote.Subject_id = updatedNote.Subject_id;
                return true;
            }
            return false;
        }


        public bool UpdateNoteAt(int index, Note newNote)
        {
            if (index >= 0 && index < Notes.Count && newNote != null)
            {
                Notes[index] = newNote;
                return true;
            }
            return false;
        }

        public bool HasNote(int noteId)
        {
            return Notes.Any(n => n.Id == noteId);
        }
    }
}
