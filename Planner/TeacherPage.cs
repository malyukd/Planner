using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Planner
{
    public partial class TeacherPage : Form
    {
        AuthorizationPage authorizationPage;
        SqlConnection conn;
        Teacher currT = null;
        int id;
        private Label cellTooltipLabel;
        private bool closing = false;
        public TeacherPage(SqlConnection c, int id, AuthorizationPage ap)
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

        private void TeacherPage_Load(object sender, EventArgs e)
        {
            string query = $"SELECT name FROM Teachers WHERE teacher_id = {id}";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {

                try
                {

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Получаем имя и группу
                            currT = new Teacher(id, reader["name"].ToString());
                            // Отображаем данные на форме
                            label2.Text = currT.Name;


                        }
                        else
                        {
                            MessageBox.Show("Данные студента не найдены.");
                        }
                    }

                }
                catch (SqlException ex)
                {
                    MessageBox.Show($"Ошибка базы данных: {ex.Message}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}");
                }
                LoadSubjects();
                LoadGroups();
                LoadStudents();
                LoadExams();
                LoadAssignments();
                LoadBuildings();
                LoadTypes();
                LoadTeachers();
            }

            decorateDataGridView(dataGridView1);
            decorateDataGridView(dataGridView2);
            decorateDataGridView(dataGridView3);
            decorateDataGridView(dataGridView4);
            decorateDataGridView(dataGridView5);
            decorateDataGridView(dataGridView6);
            dataGridView4.AllowUserToAddRows = true;
            dataGridView5.AllowUserToAddRows = true;
        }

        private void TeacherPage_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!closing)
                Application.Exit();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            closing = true;
            this.Close();
            authorizationPage.Show();
        }
        private void decorateDataGridView(DataGridView dgv)
        {
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgv.AllowUserToResizeColumns = false;
            dgv.AllowUserToResizeRows = false;
            dgv.RowHeadersVisible = false;
            dgv.AllowUserToAddRows = false;

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


            string query = $"SELECT Subjects.subject_id, title FROM Subjects " +
            $"JOIN TeacherSubject ON TeacherSubject.subject_id = Subjects.subject_id " +
            $"WHERE teacher_id = {currT.Id}";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {


                using (SqlDataReader reader = cmd.ExecuteReader())
                {

                    while (reader.Read())
                    {

                        int id = Convert.ToInt32(reader["subject_id"]);
                        string title = reader["title"].ToString();
                        currT.Subjects.Add(new Subject(id, title));
                    }
                    dataGridView1.DataSource = null;
                    dataGridView1.DataSource = currT.Subjects;
                    dataGridView1.ReadOnly = true;
                    dataGridView1.Columns[0].Visible = false;
                    reader.Close();
                }
            }


        }
        private void LoadGroups()
        {

            string query = $"SELECT group_id FROM TeacherGroup " +
            $"WHERE teacher_id = {currT.Id}";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {


                using (SqlDataReader reader = cmd.ExecuteReader())
                {

                    while (reader.Read())
                    {


                        string id = reader["group_id"].ToString();
                        currT.Groups.Add(id);
                    }
                    dataGridView2.DataSource = null;
                    dataGridView2.Columns.Add("Group", "Group");

                    foreach (string s in currT.Groups)
                    {
                        dataGridView2.Rows.Add(s);
                    }
                    dataGridView2.ReadOnly = true;
                    comboBox1.DataSource = currT.Groups;
                    comboBox2.DataSource = currT.Groups;
                    comboBox3.DataSource = currT.Groups;
                    reader.Close();
                }
            }


        }
        private void LoadStudents()
        {
            string query = $"SELECT name AS Student FROM Students " +
                $"WHERE group_id = '{comboBox1.SelectedItem}'";


            SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);

            dataGridView3.DataSource = dataTable;


        }
        private void LoadAssignments()
        {
            string query = $"SELECT assignment_id, Assignments.title AS Title, subject_id as Subject" +
                $", description AS Description, [date] AS Date, " +
                $"due_date AS [Due date] FROM Assignments " +
                $"WHERE group_id = '{comboBox2.SelectedItem}' AND teacher_id={currT.Id}";

            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);


                dataGridView4.DataSource = dataTable;

                dataGridView4.ReadOnly = false;
                FillSubject(dataGridView4);
                dataGridView4.Columns["assignment_id"].Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }
        private void FillSubject(DataGridView dvg)
        {
            try
            {
                string oldColumnName = dvg.Columns[2].Name;

                List<int> oldValues = new List<int>();
                foreach (DataGridViewRow row in dvg.Rows)
                {
                    if (!row.IsNewRow)
                        oldValues.Add(Convert.ToInt32(row.Cells[oldColumnName].Value));
                }


                int columnIndex = 2;
                dvg.Columns.Remove(dvg.Columns[oldColumnName]);

                DataGridViewComboBoxColumn comboCol = new DataGridViewComboBoxColumn();
                comboCol.Name = oldColumnName;
                comboCol.HeaderText = oldColumnName;
                comboCol.DataSource = currT.Subjects;
                comboCol.DisplayMember = "Title";
                comboCol.ValueMember = "Id";
                comboCol.DataPropertyName = oldColumnName;
                comboCol.FlatStyle = FlatStyle.Flat;
                comboCol.Width = 120;
                dvg.Columns.Insert(columnIndex, comboCol);
                for (int i = 0; i < oldValues.Count; i++)
                {
                    dvg.Rows[i].Cells[columnIndex].Value = oldValues[i];
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
            }
        private void LoadExams()
        {
            string query = $"SELECT exam_id, Exams.title AS Title, subject_id, " +
                $"type_id AS Type, date AS Date, time AS Time, " +
                $"building_id AS Building  FROM Exams " +
                $"WHERE group_id = '{comboBox3.SelectedItem}' AND teacher_id={currT.Id}";

            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                dataGridView5.DataSource = dataTable;
                FillSubject(dataGridView5);
                FillTypes(dataGridView5);
                FillBuildings(dataGridView5);
                dataGridView5.Columns["exam_id"].Visible = false;
                dataGridView5.ReadOnly = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }

        }
        private void FillTypes(DataGridView dvg)
        {
            string oldColumnName = dvg.Columns[3].Name;

            List<int> oldValues = new List<int>();
            foreach (DataGridViewRow row in dvg.Rows)
            {
                if (!row.IsNewRow)
                    oldValues.Add(Convert.ToInt32(row.Cells[oldColumnName].Value));
            }


            int columnIndex = 3;
            dvg.Columns.Remove(dvg.Columns[oldColumnName]);

            DataGridViewComboBoxColumn comboCol = new DataGridViewComboBoxColumn();
            comboCol.Name = oldColumnName;
            comboCol.HeaderText = oldColumnName;
            comboCol.DataSource = ExamType.ETypes;
            comboCol.DisplayMember = "Name";
            comboCol.ValueMember = "Id";
            comboCol.DataPropertyName = oldColumnName;
            comboCol.FlatStyle = FlatStyle.Flat;
            comboCol.Width = 120;
            dvg.Columns.Insert(columnIndex, comboCol);
            for (int i = 0; i < oldValues.Count; i++)
            {
                dvg.Rows[i].Cells[columnIndex].Value = oldValues[i];
            }
        }
        private void FillBuildings(DataGridView dvg)
        {
            string oldColumnName = dvg.Columns[6].Name;

            List<int> oldValues = new List<int>();
            foreach (DataGridViewRow row in dvg.Rows)
            {
                if (!row.IsNewRow)
                    oldValues.Add(Convert.ToInt32(row.Cells[oldColumnName].Value));
            }


            int columnIndex = 6;
            dvg.Columns.Remove(dvg.Columns[oldColumnName]);

            DataGridViewComboBoxColumn comboCol = new DataGridViewComboBoxColumn();
            comboCol.Name = oldColumnName;
            comboCol.HeaderText = oldColumnName;
            comboCol.DataSource = Building.Buildings;
            comboCol.DisplayMember = "Address";
            comboCol.ValueMember = "Id";
            comboCol.DataPropertyName = oldColumnName;
            comboCol.FlatStyle = FlatStyle.Flat;
            comboCol.Width = 120;
            dvg.Columns.Insert(columnIndex, comboCol);
            for (int i = 0; i < oldValues.Count; i++)
            {
                dvg.Rows[i].Cells[columnIndex].Value = oldValues[i];
            }
        }
        private void LoadTypes()
        {
            if (ExamType.ETypes.Count == 0)
            {

                string query = $"SELECT type_id, name FROM ExamTypes ORDER BY type_id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {


                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            int id = Convert.ToInt32(reader["type_id"]);
                            string name = reader["name"].ToString();
                            ExamType.ETypes.Add(new ExamType(id, name));
                        }

                    }
                }

            }
        }
        private void LoadBuildings()
        {
            if (Building.Buildings.Count == 0)
            {

                string query = $"SELECT building_id, address FROM Buildings ORDER BY building_id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {


                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            int id = Convert.ToInt32(reader["building_id"]);
                            string name = reader["address"].ToString();
                            Building.Buildings.Add(new Building(id, name));
                        }

                    }
                }

            }
        }
        private void LoadTeachers()
        {
            string query = $"SELECT name AS Teacher FROM Teachers ";


            SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);

            dataGridView6.DataSource = dataTable;
            dataGridView6.ReadOnly = true;

        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadStudents();
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadExams();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadAssignments();
        }

        private void dataGridView4_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            int row = e.RowIndex;

            try
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Assignments WHERE assignment_id = @assignment_id", conn))
                {
                    if (dataGridView4.Rows[row].Cells["assignment_id"].Value.ToString().Equals(""))
                        dataGridView4.Rows[row].Cells["assignment_id"].Value = -1;


                    cmd.Parameters.AddWithValue("@assignment_id", dataGridView4.Rows[row].Cells["assignment_id"].Value);
                    object res = cmd.ExecuteScalar();
                    if (res != null)
                    {
                        var comboCell = dataGridView4.Rows[row].Cells[2] as DataGridViewComboBoxCell;
                        string query = "UPDATE Assignments " +
                            "SET title = @tittle, " +
                            "description = @description," +
                            "date = @date," +
                            "due_date = @due_date," +
                            "subject_id=@subject_id " +
                            "WHERE assignment_id = @assignment_id";
                        using (SqlCommand cmd2 = new SqlCommand(query, conn))
                        {
                            cmd2.Parameters.AddWithValue("@assignment_id", Convert.ToInt32(dataGridView4.Rows[row].Cells["assignment_id"].Value));
                            cmd2.Parameters.AddWithValue("@tittle", dataGridView4.Rows[row].Cells["Title"].Value);
                            cmd2.Parameters.AddWithValue("@description", dataGridView4.Rows[row].Cells["Description"].Value);
                            cmd2.Parameters.AddWithValue("@date", dataGridView4.Rows[row].Cells["Date"].Value);
                            cmd2.Parameters.AddWithValue("@due_date", dataGridView4.Rows[row].Cells["Due date"].Value);
                            cmd2.Parameters.AddWithValue("@subject_id", comboCell.Value);
                            cmd2.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        bool insert = true;
                        for (int i = 0; i < dataGridView4.Columns.Count; i++)
                        {
                            if (dataGridView4.Rows[row].Cells[i].Value.ToString().Equals(""))
                                insert = false;
                        }
                        if (!insert)
                            return;
                        var comboCell = dataGridView4.Rows[row].Cells[2] as DataGridViewComboBoxCell;
                        using (SqlCommand cmd2 = new SqlCommand("INSERT INTO Assignments(title, description, date, due_date, teacher_id, subject_id, group_id) " +
                            "VALUES (@title, @description, @date, @due_date, @teacher_id, @subject_id, @group_id)", conn))
                        {
                            cmd2.Parameters.AddWithValue("@teacher_id", currT.Id);
                            cmd2.Parameters.AddWithValue("@title", dataGridView4.Rows[row].Cells["Title"].Value);
                            cmd2.Parameters.AddWithValue("@description", dataGridView4.Rows[row].Cells["Description"].Value);
                            cmd2.Parameters.AddWithValue("@date", dataGridView4.Rows[row].Cells["Date"].Value);
                            cmd2.Parameters.AddWithValue("@due_date", dataGridView4.Rows[row].Cells["Due date"].Value);
                            cmd2.Parameters.AddWithValue("@subject_id", comboCell.Value);
                            cmd2.Parameters.AddWithValue("@group_id", comboBox2.SelectedItem);
                            int rowsAffected = cmd2.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Запись добавлена");
                                string query3 = $"SELECT TOP 1 assignment_id" +
                                    $" FROM Assignments" +
                                    $" ORDER BY assignment_id" +
                                    $" DESC";
                                using (SqlCommand cmd3 = new SqlCommand(query3, conn))
                                {
                                    int result = Convert.ToInt32(cmd3.ExecuteScalar());
                                    dataGridView4.Rows[row].Cells[0].Value = result;
                                }

                            }
                            else
                                MessageBox.Show("Произошла ошибка");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }

        }

        private void dataGridView5_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            int row = e.RowIndex;

            try
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Exams WHERE exam_id = @exam_id", conn))
                {
                    if (dataGridView5.Rows[row].Cells[0].Value.ToString().Equals(""))
                        dataGridView5.Rows[row].Cells[0].Value = -1;


                    cmd.Parameters.AddWithValue("@exam_id", dataGridView5.Rows[row].Cells[0].Value);
                    object res = cmd.ExecuteScalar();
                    if (res != null)
                    {
                        var comboCell = dataGridView5.Rows[row].Cells[2] as DataGridViewComboBoxCell;
                        var comboCell2 = dataGridView5.Rows[row].Cells[3] as DataGridViewComboBoxCell;
                        var comboCell3 = dataGridView5.Rows[row].Cells[6] as DataGridViewComboBoxCell;
                        string query = "UPDATE Exams " +
                            "SET title = @title, " +
                            "date = @date," +
                            "time = @time," +
                            "subject_id=@subject_id," +
                            "building_id=@building_id, " +
                            "type_id=@type_id " +
                            "WHERE exam_id = @exam_id";
                        using (SqlCommand cmd2 = new SqlCommand(query, conn))
                        {
                            cmd2.Parameters.AddWithValue("@exam_id", dataGridView5.Rows[row].Cells[0].Value);
                            cmd2.Parameters.AddWithValue("@title", dataGridView5.Rows[row].Cells["Title"].Value);
                            cmd2.Parameters.AddWithValue("@subject_id", comboCell.Value);
                            cmd2.Parameters.AddWithValue("@date", dataGridView5.Rows[row].Cells["Date"].Value);
                            cmd2.Parameters.AddWithValue("@time", dataGridView5.Rows[row].Cells["Time"].Value);
                            cmd2.Parameters.AddWithValue("@type_id", comboCell2.Value);
                            cmd2.Parameters.AddWithValue("@building_id", comboCell3.Value);
                            int r = cmd2.ExecuteNonQuery();
                            if (r < 0)
                                MessageBox.Show("Произошла ошибка");
                        }
                    }
                    else
                    {
                        bool insert = true;
                        for (int i = 0; i < dataGridView5.Columns.Count; i++)
                        {
                            if (dataGridView5.Rows[row].Cells[i].Value.ToString().Equals(""))
                                insert = false;
                        }
                        if (!insert)
                            return;
                        var comboCell = dataGridView5.Rows[row].Cells[2] as DataGridViewComboBoxCell;
                        var comboCell2 = dataGridView5.Rows[row].Cells[3] as DataGridViewComboBoxCell;
                        var comboCell3 = dataGridView5.Rows[row].Cells[6] as DataGridViewComboBoxCell;
                        using (SqlCommand cmd2 = new SqlCommand("INSERT INTO Exams(subject_id, title, date, time, [type_id], teacher_id, building_id, group_id) " +
                            "VALUES (@subject_id, @title, @date, @time, @type_id, @teacher_id, @building_id, @group_id)", conn))
                        {
                            cmd2.Parameters.AddWithValue("@teacher_id", currT.Id);
                            cmd2.Parameters.AddWithValue("@title", dataGridView5.Rows[row].Cells["Title"].Value);
                            cmd2.Parameters.AddWithValue("@subject_id", comboCell.Value);
                            cmd2.Parameters.AddWithValue("@date", dataGridView5.Rows[row].Cells["Date"].Value);
                            cmd2.Parameters.AddWithValue("@time", dataGridView5.Rows[row].Cells["Time"].Value);
                            cmd2.Parameters.AddWithValue("@type_id", comboCell2.Value);
                            cmd2.Parameters.AddWithValue("@building_id", comboCell3.Value);
                            cmd2.Parameters.AddWithValue("@group_id", comboBox3.SelectedItem);
                            int rowsAffected = cmd2.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                    MessageBox.Show("Запись добавлена");
                                    string query3 = $"SELECT TOP 1 exam_id" +
                                        $" FROM Exams" +
                                        $" ORDER BY exam_id" +
                                        $" DESC";
                                    using (SqlCommand cmd3 = new SqlCommand(query3, conn))
                                    {
                                        int result = Convert.ToInt32(cmd3.ExecuteScalar());
                                        dataGridView5.Rows[row].Cells[0].Value = result;
                                    }
                                
                            }
                            else
                                MessageBox.Show("Произошла ошибка");
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int row = dataGridView4.CurrentRow.Index;
            string query = $"DELETE FROM Assignments WHERE assignment_id=@assignment_id";
            try
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@assignment_id", dataGridView4.Rows[row].Cells[0].Value);
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Запись удалена");
                        dataGridView4.Rows.Remove(dataGridView4.Rows[row]);
                    }
                    else
                        MessageBox.Show("Произошла ошибка");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            int row = dataGridView5.CurrentRow.Index;
            string query = $"DELETE FROM Exams WHERE exam_id=@exam_id";
            try
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@exam_id", dataGridView5.Rows[row].Cells[0].Value);
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Запись удалена");
                        dataGridView5.Rows.Remove(dataGridView5.Rows[row]);
                    }
                    else
                        MessageBox.Show("Произошла ошибка");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void dataGridView4_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (dataGridView4.Rows[e.RowIndex].IsNewRow) return;
            string newValue = e.FormattedValue.ToString();

            if (string.IsNullOrWhiteSpace(newValue))
            {
                MessageBox.Show("Поле не может пустым");
                e.Cancel = true;
            }
        }

        private void dataGridView5_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (dataGridView5.Rows[e.RowIndex].IsNewRow) return;
            string newValue = e.FormattedValue.ToString();

            if (string.IsNullOrWhiteSpace(newValue))
            {
                MessageBox.Show("Поле не может пустым");
                e.Cancel = true;
            }
        }

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show($"Ошибка ввода данных в ячейку: {e.Exception.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            e.ThrowException = false;
        }

        private void dataGridView2_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show($"Ошибка ввода данных в ячейку: {e.Exception.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            
            e.ThrowException = false;
        }

        private void dataGridView3_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show($"Ошибка ввода данных в ячейку: {e.Exception.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            
            e.ThrowException = false;
        }

        private void dataGridView4_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show($"Ошибка ввода данных в ячейку: {e.Exception.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            
            e.ThrowException = false;
        }

        private void dataGridView5_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show($"Ошибка ввода данных в ячейку: {e.Exception.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        
            e.ThrowException = false;
        }
    }
}

