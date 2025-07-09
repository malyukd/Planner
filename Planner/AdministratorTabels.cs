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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar;
using System.Security.Cryptography;

namespace Planner
{
    public partial class AdministratorTabels : Form
    {

        bool closing = false;
        AuthorizationPage authorizationPage;
        SqlConnection conn;
        DataTable dataTable;
        private Label cellTooltipLabel;
        private bool exc = false;
        public AdministratorTabels(SqlConnection c, AuthorizationPage ap)
        {
            InitializeComponent();
            this.conn = c;
            button1.Left = (ClientRectangle.Width - button1.Width) / 2;
            this.authorizationPage = ap;
            cellTooltipLabel = new Label();
            cellTooltipLabel.AutoSize = true;
            cellTooltipLabel.BackColor = Color.White;
            cellTooltipLabel.BorderStyle = BorderStyle.FixedSingle;
            cellTooltipLabel.Font = new Font("Segoe UI", 10);
            cellTooltipLabel.Visible = false;
            cellTooltipLabel.Padding = new Padding(4);
            this.Controls.Add(cellTooltipLabel);

        }
        private void LoadTableNames()
        {


            DataTable schema = conn.GetSchema("Tables");

            foreach (DataRow row in schema.Rows)
            {
                string tableName = row[2].ToString();
                comboBox1.Items.Add(tableName);
            }

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedTable = comboBox1.SelectedItem.ToString();
            LoadTableData(selectedTable);
            decorateDataGridView(dataGridView1);
        }
        private void LoadTableData(string tableName)
        {
            string query = $"SELECT * FROM [{tableName}]";


            using (SqlCommand command = new SqlCommand(query, conn))
            {

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    dataGridView1.DataSource = dataTable;

                }

            }

           
            if (tableName != "Groups" && tableName != "TeacherSubject" && tableName != "TeacherGroup" && tableName != "StudentAssignment")
            {
                dataGridView1.Columns[0].ReadOnly = true;
                exc = false;
            }
            else
            {

                
                exc = true;
            }
        }

        private void AdministratorTabels_Load(object sender, EventArgs e)
        {
            LoadTableNames();
            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            comboBox1.SelectedIndex = 0;
            decorateDataGridView(dataGridView1);
        }
        private void decorateDataGridView(DataGridView dgv)
        {
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgv.AllowUserToAddRows = true;
            dgv.AllowUserToResizeColumns = false;
            dgv.AllowUserToResizeRows = false;
            dgv.RowHeadersVisible = true;



            dgv.RowsDefaultCellStyle.BackColor = Color.White;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgv.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2; // или EditOnEnter
            dataGridView1.ReadOnly = false;


            // Мягкий цвет выделения
            dgv.DefaultCellStyle.SelectionBackColor = Color.LightSteelBlue;
            dgv.DefaultCellStyle.SelectionForeColor = Color.Black;

            // Отключаем выделение заголовков
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = dgv.ColumnHeadersDefaultCellStyle.BackColor;
            dgv.ColumnHeadersDefaultCellStyle.SelectionForeColor = dgv.ColumnHeadersDefaultCellStyle.ForeColor;

            dgv.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            dgv.GridColor = Color.LightGray;

            foreach (DataGridViewColumn column in dgv.Columns)
            {
                if (column.ValueType == typeof(int) || column.ValueType == typeof(DateTime))
                {
                    column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            closing = true;
            this.Close();
            authorizationPage.Show();
        }

        private void AdministratorTabels_FormClosing(object sender, FormClosingEventArgs e)
        {
            conn.Close();
            if (!closing)
                Application.Exit();
        }

        private void HideCellTooltip(object sender, DataGridViewCellEventArgs e)
        {
            cellTooltipLabel.SendToBack();
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


        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null)
                return;

            int row = dataGridView1.CurrentRow.Index;

            

            string query = $"DELETE FROM {comboBox1.SelectedItem.ToString()} WHERE ";
            if (!exc || dataGridView1.ColumnCount == 1)
                query += dataGridView1.Columns[0].Name + "=@" + dataGridView1.Columns[0].Name;
            else
            {
                query += dataGridView1.Columns[0].Name + "=@" + dataGridView1.Columns[0].Name + " AND " + dataGridView1.Columns[1].Name + "=@" + dataGridView1.Columns[1].Name;
            }
            try
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@" + dataGridView1.Columns[0].Name, dataGridView1.Rows[row].Cells[0].Value);
                    var k = dataGridView1.Rows[row].Cells[0].Value;
                    if (exc && dataGridView1.ColumnCount != 1)
                        cmd.Parameters.AddWithValue("@" + dataGridView1.Columns[1].Name, dataGridView1.Rows[row].Cells[1].Value);
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Запись удалена");
                        dataGridView1.Rows.Remove(dataGridView1.Rows[row]);
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

        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            int row = e.RowIndex;
            var value = dataGridView1.Rows[row].Cells[0].Value.ToString();
            if (value.Equals(""))
            {
                if (!exc)
                    dataGridView1.Rows[row].Cells[0].Value = (int)dataGridView1.Rows[dataGridView1.RowCount - 2].Cells[0].Value + 1;
            }
         
            if (exc&&e.RowIndex != dataGridView1.NewRowIndex)
            {
                e.Cancel = true;
            }
            if (comboBox1.SelectedItem.ToString().Equals("StudentAssignment")&&e.RowIndex != dataGridView1.NewRowIndex)
            {
                if (e.ColumnIndex == 0 || e.ColumnIndex == 1)
                {
                    e.Cancel = true;
                }
            }

        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            int row = e.RowIndex;

            string query = $"SELECT  COUNT(*) FROM {comboBox1.SelectedItem.ToString()} WHERE ";
            if (!exc|| comboBox1.SelectedItem.ToString().Equals("Groups"))
                query += dataGridView1.Columns[0].Name + "=@" + dataGridView1.Columns[0].Name;
            else
            {
                query += dataGridView1.Columns[0].Name + "=@" + dataGridView1.Columns[0].Name + " AND " + dataGridView1.Columns[1].Name + "=@" + dataGridView1.Columns[1].Name;
            }
            try
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@" + dataGridView1.Columns[0].Name, dataGridView1.Rows[row].Cells[0].Value);
                    if (exc)
                        cmd.Parameters.AddWithValue("@" + dataGridView1.Columns[1].Name, dataGridView1.Rows[row].Cells[1].Value);
                    object res = cmd.ExecuteScalar();
                    if ((int)res != 0)
                    {
                        string query2 = $"UPDATE {comboBox1.SelectedItem.ToString()} SET ";
                        query2 += dataGridView1.Columns[e.ColumnIndex].Name + "=@" + dataGridView1.Columns[e.ColumnIndex].Name;
                        query2 += " WHERE ";
                        if (!exc)
                            query2 += dataGridView1.Columns[0].Name + "=@" + dataGridView1.Columns[0].Name;
                        else
                        {
                            query2 += dataGridView1.Columns[0].Name + "=@" + dataGridView1.Columns[0].Name + " AND " + dataGridView1.Columns[1].Name + "=@" + dataGridView1.Columns[1].Name;
                        }
                        using (SqlCommand cmd2 = new SqlCommand(query2, conn))
                        {
                            cmd2.Parameters.AddWithValue("@" + dataGridView1.Columns[0].Name, dataGridView1.Rows[row].Cells[0].Value);
                            if (exc)
                                cmd2.Parameters.AddWithValue("@" + dataGridView1.Columns[1].Name, dataGridView1.Rows[row].Cells[1].Value);
                            cmd2.Parameters.AddWithValue("@" + dataGridView1.Columns[e.ColumnIndex].Name, dataGridView1.Rows[row].Cells[e.ColumnIndex].Value);
                            int rowsAffected = cmd2.ExecuteNonQuery();

                            if (rowsAffected < 0)
                                MessageBox.Show("Произошла ошибка");
                        }
                    }
                    else
                    {
                        bool insert = true;
                        for (int i = 0; i < dataGridView1.Columns.Count; i++)
                        {
                            if (dataGridView1.Rows[row].Cells[i].Value.ToString().Equals(""))
                                insert = false;
                        }
                        if (!insert)
                            return;
                        string query2 = $"INSERT INTO {comboBox1.SelectedItem.ToString()} (";
                        for (int i = exc ? 0 : 1; i < dataGridView1.Columns.Count; i++)
                        {
                            query2 += dataGridView1.Columns[i].Name;
                            if (i == dataGridView1.Columns.Count - 1)
                                query2 += ") VALUES (";
                            else
                                query2 += ", ";
                        }
                        for (int i = exc ? 0 : 1; i < dataGridView1.Columns.Count; i++)
                        {
                            query2 += "@" + dataGridView1.Columns[i].Name;
                            if (i == dataGridView1.Columns.Count - 1)
                                query2 += ")";
                            else
                                query2 += ", ";
                        }
                        using (SqlCommand cmd2 = new SqlCommand(query2, conn))
                        {

                            for (int i = exc ? 0 : 1; i < dataGridView1.Columns.Count; i++)
                            {
                                cmd2.Parameters.AddWithValue("@" + dataGridView1.Columns[i].Name, dataGridView1.Rows[row].Cells[i].Value);
                            }
                            int rowsAffected = cmd2.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                if (!exc)
                                {
                                    MessageBox.Show("Запись добавлена");
                                    string query3 = $"SELECT TOP 1 " + dataGridView1.Columns[0].Name +
                                        $" FROM {comboBox1.SelectedItem.ToString()} " +
                                        $" ORDER BY " + dataGridView1.Columns[0].Name +
                                        $" DESC";
                                    using (SqlCommand cmd3 = new SqlCommand(query3, conn))
                                    {
                                        int result = Convert.ToInt32(cmd3.ExecuteScalar());
                                        dataGridView1.Rows[row].Cells[0].Value = result;
                                    }
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

        private void dataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (dataGridView1.Rows[e.RowIndex].IsNewRow) return;
            string newValue = e.FormattedValue.ToString();

            if (string.IsNullOrWhiteSpace(newValue)&&e.RowIndex != dataGridView1.NewRowIndex)
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
    }

}
