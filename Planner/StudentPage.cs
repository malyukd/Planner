using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using static Azure.Core.HttpHeader;

namespace Planner
{
    public partial class StudentPage : Form
    {
        AuthorizationPage authorizationPage;
        SqlConnection conn;
 
        Student currSt = null;
        int id;
        private Label cellTooltipLabel;
        Note currNote = null;
        int for_note_id = 6000;
        List<int[]> assignStatus = new List<int[]>();
        private bool closing = false;

        public StudentPage(SqlConnection c, int id, AuthorizationPage ap)
        {
            InitializeComponent();
            this.conn = c;
            this.id = id;
            cellTooltipLabel = new Label();
            cellTooltipLabel.AutoSize = true;
            cellTooltipLabel.BackColor = Color.White;
            cellTooltipLabel.BorderStyle = BorderStyle.FixedSingle;
            cellTooltipLabel.Font = new Font("Segoe UI", 10);
            cellTooltipLabel.Visible = false;
            cellTooltipLabel.Padding = new Padding(4);
            this.Controls.Add(cellTooltipLabel);
            button4.Left = (ClientRectangle.Width - button4.Width) / 2;
            this.authorizationPage = ap;
        }

        private void HideCellTooltip(object sender, DataGridViewCellEventArgs e)
        {
            cellTooltipLabel.Visible = false;
        }
        private void ShowCellTooltip(object sender, DataGridViewCellEventArgs e)
        {
            var dgv = sender as DataGridView;
            int rowIndex = e.RowIndex;
            int columnIndex = e.ColumnIndex;
            if (rowIndex >= 0 && columnIndex >= 0)
            {
                var cell = dgv.Rows[rowIndex].Cells[columnIndex];
                if (cell.Value != null)
                {
                    string text = cell.Value.ToString();

                    var cellRect = dgv.GetCellDisplayRectangle(columnIndex, rowIndex, true);
                    using (Graphics g = dgv.CreateGraphics())
                    {
                        SizeF textSize = g.MeasureString(text, dgv.DefaultCellStyle.Font);
                        if (textSize.Width > cellRect.Width)
                        {
                            cellTooltipLabel.Text = text;

                            var cellScreenLocation = dgv.PointToScreen(cellRect.Location);
                            var formLocation = this.PointToClient(cellScreenLocation);

                            cellTooltipLabel.Location = new Point(formLocation.X + 2, formLocation.Y + cellRect.Height + 2);
                            cellTooltipLabel.BringToFront();
                            cellTooltipLabel.Visible = true;
                        }
                        else
                        {
                            cellTooltipLabel.Visible = false;
                        }
                    }
                }
                else
                {
                    cellTooltipLabel.Visible = false;
                }
            }
        }

        private void StudentPage_Load(object sender, EventArgs e)
        {

            string query = $"SELECT name, group_id FROM Students WHERE student_id = {id}";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@studentId", id);

                try
                {

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Получаем имя и группу
                            currSt = new Student(id, reader["name"].ToString(), reader["group_id"].ToString());
                            // Отображаем данные на форме
                            label2.Text = currSt.Name;
                            label4.Text = currSt.Group;
                            tabPage2.Text = currSt.Group;
                           

                        }
                        else
                        {
                            MessageBox.Show("Данные студента не найдены.");
                        }
                    }
                    LoadTeachers();
                    LoadGroup();
                    LoadAssignments();
                    LoadExams();
                    LoadSubjects();
                    LoadNotes();
                    LoadStatuses();
                    LoadAssignStatuses();
                }
                catch (SqlException ex)
                {
                    MessageBox.Show($"Ошибка базы данных: {ex.Message}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}");
                }
            }

            decorateDataGridView(dataGridView1);
            decorateDataGridView(dataGridView2);
            decorateDataGridView(dataGridView3);
            decorateDataGridView(dataGridView4);
            decorateDataGridView(dataGridView5);

        }

        private void LoadTeachers()
        {
            string query = $"SELECT name AS Teacher, title AS Subject FROM Teachers " +
                $"Join TeacherSubject ON Teachers.teacher_id = TeacherSubject.teacher_id " +
                $"Join TeacherGroup ON Teachers.teacher_id = TeacherGroup.teacher_id " +
                $"Join Subjects ON TeacherSubject.subject_id = Subjects.subject_id " +
                $"WHERE group_id = '{currSt.Group}'";


            SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);

            dataGridView1.DataSource = dataTable;

        }
        private void LoadGroup()
        {
            string query = $"SELECT name AS Student FROM Students " +
                $"WHERE group_id = '{currSt.Group}'";


            SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);

            dataGridView2.DataSource = dataTable;

        }
        private void LoadAssignments()
        {
            string query = $"SELECT assignment_id, Assignments.title AS Title, Subjects.title " +
                $"AS Subject, description AS Description, [date] AS Date, " +
                $"due_date AS [Due date] FROM Assignments " +
                $"JOIN Subjects ON Assignments.subject_id = Subjects.subject_id " +
                $"WHERE group_id = '{currSt.Group}'";


            SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);

            dataGridView3.DataSource = dataTable;
            dataGridView3.Columns["assignment_id"].Visible = false;

        }
        private void LoadExams()
        {
            string query = $"SELECT Exams.title AS Title, Subjects.title AS Subject, " +
                $"Teachers.name AS Teacher, ExamTypes.name AS Type, date AS Date, time AS Time, " +
                $"Buildings.address AS Building  FROM Exams " +
                $"JOIN Subjects ON Subjects.subject_id = Exams.subject_id " +
                $"JOIN ExamTypes ON ExamTypes.type_id = Exams.type_id " +
                $"JOIN Teachers ON Teachers.teacher_id = Exams.teacher_id " +
                $"JOIN Buildings ON Buildings.building_id = Exams.building_id " +
                $"WHERE group_id = '{currSt.Group}'";


            SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);

            dataGridView4.DataSource = dataTable;

        }
        private void decorateDataGridView(DataGridView dgv)
        {
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToResizeColumns = false;
            dgv.AllowUserToResizeRows = false;
            dgv.RowHeadersVisible = false;

            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;

            dgv.RowsDefaultCellStyle.BackColor = Color.White;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgv.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.GridColor = Color.LightGray;

            // Мягкий цвет выделения
            dgv.DefaultCellStyle.SelectionBackColor = Color.LightSteelBlue;
            dgv.DefaultCellStyle.SelectionForeColor = Color.Black;

            // Отключаем выделение заголовков
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = dgv.ColumnHeadersDefaultCellStyle.BackColor;
            dgv.ColumnHeadersDefaultCellStyle.SelectionForeColor = dgv.ColumnHeadersDefaultCellStyle.ForeColor;
        }
        private void LoadSubjects()
        {
            if (Subject.Subjects.Count == 0)
            {

                string query = $"SELECT subject_id, title FROM Subjects";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {


                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        Subject b = new Subject(0, "Предмет не выбран");
                        while (reader.Read())
                        {
                            // Получаем имя и группу
                            int id = Convert.ToInt32(reader["subject_id"]);
                            string title = reader["title"].ToString();
                            Subject.Subjects.Add(new Subject(id, title));
                        }
                        comboBox1.DataSource = Subject.Subjects;
                        comboBox1.DisplayMember = "Title";
                        comboBox1.ValueMember = "Id";
                    }
                }

            }
        }
        private void LoadStatuses()
        {
            if (Status.Statuses.Count == 0)
            {

                string query = $"SELECT status_id, name FROM AssignmentStatus ORDER BY status_id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {


                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            int id = Convert.ToInt32(reader["status_id"]);
                            string name = reader["name"].ToString();
                            Status.Statuses.Add(new Status(id, name));
                        }

                    }
                }

            }
        }
        private void LoadAssignStatuses()
        {
            string query = $"SELECT assignment_id, status_id FROM StudentAssignment " +
                $"WHERE student_id = {currSt.Id}";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {


                using (SqlDataReader reader = cmd.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        assignStatus.Add(new int[] { Convert.ToInt32(reader["assignment_id"]), Convert.ToInt32(reader["status_id"]) });
                    }

                }
            }

            DataGridViewComboBoxColumn comboColumn = new DataGridViewComboBoxColumn();
            comboColumn.HeaderText = "Status";
            comboColumn.Name = "StatusColumn";
            comboColumn.DataSource = Status.Statuses;
            comboColumn.DisplayMember = "Name";
            comboColumn.ValueMember = "Id";
            comboColumn.FlatStyle = FlatStyle.Flat;
            comboColumn.Width = 120;
            dataGridView3.Columns.Add(comboColumn);
            dataGridView3.Columns["Title"].ReadOnly = true;
            dataGridView3.Columns["Subject"].ReadOnly = true;
            dataGridView3.Columns["Description"].ReadOnly = true;
            dataGridView3.Columns["Date"].ReadOnly = true;
            dataGridView3.Columns["Due date"].ReadOnly = true;
            dataGridView3.DataBindingComplete += (s, e) =>
            {
                for (int i = 0; i < dataGridView3.Rows.Count; i++)
                {
                    int id = Convert.ToInt32(dataGridView3.Rows[i].Cells["assignment_id"].Value);
                    bool inDB = false;
                    foreach (int[] arr in assignStatus)
                    {
                        if (arr[0] == id)
                        {
                            var comboCell = dataGridView3.Rows[i].Cells["StatusColumn"] as DataGridViewComboBoxCell;
                            comboCell.Value = arr[1];
                            inDB = true;
                        }
                    }
                    if (!inDB)
                    {
                        var comboCell = dataGridView3.Rows[i].Cells["StatusColumn"] as DataGridViewComboBoxCell;
                        comboCell.Value = 1;
                    }

                }
            };

        }
        private void LoadNotes()
        {

            string query = $"SELECT note_id,Notes.title as title,text,date, Subjects.subject_id FROM Notes " +
                $"JOIN Subjects ON Subjects.subject_id = Notes.subject_id " +
                $"WHERE student_id = {currSt.Id}";


            using (SqlCommand cmd = new SqlCommand(query, conn))
            {

                using (SqlDataReader reader = cmd.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        // Получаем имя и группу
                        var note = new Note(
                        id: Convert.ToInt32(reader["note_id"]),
                        title: reader["title"].ToString(),
                        text: reader["text"].ToString(),
                        date: reader["date"].ToString().Split(' ')[0],
                        subject: Convert.ToInt32(reader["subject_id"]),
                        fromDB: true
                        );

                        currSt.Notes.Add(note);

                    }

                }
            }
            
            dataGridView5.DataSource = null;
            dataGridView5.DataSource = currSt.Notes;
            dataGridView5.Columns["Id"].Visible = false;
            dataGridView5.Columns["Text"].Visible = false;
            dataGridView5.Columns["Subject_id"].Visible = false;
            dataGridView5.Columns["FromDB"].Visible = false;
            if (currSt.Notes.Count != 0)
            {
                currNote = currSt.Notes[0];
                textBox1.Text = currNote.Title;
                richTextBox1.Text = currNote.Text;
                comboBox1.SelectedIndex = currNote.Subject_id - 1;
                dataGridView5.Rows[0].Selected = true;
            }
        }
        private void dataGridView5_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int row = e.RowIndex;
            if (e.RowIndex >= 0)
            {
                currNote = (Note)dataGridView5.Rows[e.RowIndex].DataBoundItem;
                textBox1.Text = currNote.Title;
                richTextBox1.Text = currNote.Text;
                comboBox1.SelectedIndex = currNote.Subject_id - 1;
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (currNote.FromDB)
                {



                    string query = "DELETE FROM Notes WHERE note_id = @id";

                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@id", currNote.Id);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected < 0)
                        {
                            MessageBox.Show("Произошла ошибка");
                            return;
                        }
                    }

                }
                currSt.RemoveNoteById(currNote.Id);
                MessageBox.Show("Заметка удалена");
                
                if (dataGridView5.Rows.Count > 0) {
                    dataGridView5.Rows[0].Selected=true;
                    currNote = (Note)dataGridView5.Rows[0].DataBoundItem;
                    textBox1.Text = currNote.Title;
                    richTextBox1.Text = currNote.Text;
                    comboBox1.SelectedIndex = currNote.Subject_id - 1;
                }
                else { 
                currNote = null;
                textBox1.Text = "";
                richTextBox1.Text = "";
                comboBox1.SelectedIndex = 0;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Нет заметок для удаления");
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            currNote = null;
            textBox1.Text = "";
            richTextBox1.Text = "";
            comboBox1.SelectedIndex = 0;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (currNote == null)
            {

                if (!string.IsNullOrWhiteSpace(textBox1.Text)||!string.IsNullOrWhiteSpace(comboBox1.SelectedItem.ToString()))
                {
                    currNote = new Note(for_note_id++, textBox1.Text.Trim(), richTextBox1.Text, DateTime.Today.ToString().Split()[0], comboBox1.SelectedIndex+1, false);
                    currSt.AddNote(currNote);
                    MessageBox.Show("Заметка добавлена");
                    dataGridView5.Rows[dataGridView5.RowCount-1].Selected = true;
                }
                else
                    MessageBox.Show("Заполните все поля");
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(textBox1.Text) || !string.IsNullOrWhiteSpace(comboBox1.SelectedItem.ToString()))
                {
                    currNote.Title = textBox1.Text.Trim();
                    currNote.Text = richTextBox1.Text;
                    currNote.Date = DateTime.Today.ToString().Split()[0];
                    currNote.Subject_id = comboBox1.SelectedIndex+1;
                    currSt.UpdateNote(currNote);
                    if (currNote.FromDB)
                    {



                        string query = @"UPDATE Notes 
                                            SET title = @title, 
                                            text = @text, 
                                            date = @date,
                                            subject_id = @subject 
                                            WHERE note_id = @id";

                        using (SqlCommand command = new SqlCommand(query, conn))
                        {

                            command.Parameters.AddWithValue("@id", currNote.Id);
                            command.Parameters.AddWithValue("@title", currNote.Title);
                            command.Parameters.AddWithValue("@text", currNote.Text);
                            command.Parameters.AddWithValue("@date", DateTime.Today);
                            command.Parameters.AddWithValue("@subject", currNote.Subject_id);

                            int rowsAffected = command.ExecuteNonQuery();

                            if (rowsAffected < 0)
                            {
                                MessageBox.Show("Произошла ошибка");
                                return;
                            }
                        }

                    }
                    MessageBox.Show("Заметка изменена");
                }
                else
                    MessageBox.Show("Заполните все поля");
            }
        }
        private void StudentPage_FormClosing(object sender, FormClosingEventArgs e)
        {

            foreach (var note in currSt.Notes)
            {
                if (!note.FromDB)
                {
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO Notes (title, text, date, student_id, subject_id) VALUES (@title, @text, @date, @student_id, @subject_id)", conn))
                    {
                        cmd.Parameters.AddWithValue("@title", note.Title);
                        cmd.Parameters.AddWithValue("@text", note.Text);
                        cmd.Parameters.AddWithValue("@date", DateTime.Today);
                        cmd.Parameters.AddWithValue("@student_id", currSt.Id);
                        cmd.Parameters.AddWithValue("@subject_id", note.Subject_id);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            currSt.Notes.Clear();

            if (!closing)
                Application.Exit();
      
        }

        private void button4_Click(object sender, EventArgs e)
        {
            closing = true;
            this.Close();
          
            authorizationPage.Show();
        }

        private void dataGridView3_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            int row = e.RowIndex;


            using (SqlCommand cmd = new SqlCommand("SELECT * FROM StudentAssignment WHERE student_id = @student_id AND assignment_id = @assignment_id", conn))
            {

                cmd.Parameters.AddWithValue("@student_id", currSt.Id);
                cmd.Parameters.AddWithValue("@assignment_id", Convert.ToInt32(dataGridView3.Rows[row].Cells["assignment_id"].Value));
                var comboCell = dataGridView3.Rows[row].Cells["StatusColumn"] as DataGridViewComboBoxCell;
                cmd.Parameters.AddWithValue("@status_id", comboCell.Value);
                object res = cmd.ExecuteScalar();
                if (res != null)
                {
                    using (SqlCommand cmd2 = new SqlCommand("UPDATE StudentAssignment SET status_id = @status_id WHERE student_id = @student_id AND assignment_id = @assignment_id", conn))
                    {
                        cmd2.Parameters.AddWithValue("@student_id", currSt.Id);
                        cmd2.Parameters.AddWithValue("@assignment_id", Convert.ToInt32(dataGridView3.Rows[row].Cells["assignment_id"].Value));
                        cmd2.Parameters.AddWithValue("@status_id", comboCell.Value);
                        cmd2.ExecuteNonQuery();
                    }
                }
                else
                {
                    using (SqlCommand cmd2 = new SqlCommand("INSERT INTO StudentAssignment (student_id, assignment_id, status_id) VALUES (@student_id, @assignment_id, @status_id)", conn))
                    {
                        cmd2.Parameters.AddWithValue("@student_id", currSt.Id);
                        cmd2.Parameters.AddWithValue("@assignment_id", Convert.ToInt32(dataGridView3.Rows[row].Cells["assignment_id"].Value));
                        cmd2.Parameters.AddWithValue("@status_id", comboCell.Value);
                        cmd2.ExecuteNonQuery(); ;
                    }
                }
            }

        }
    }
}
