using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Security.Cryptography;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace Planner
{
    enum Role
    {
        Student,
        Teacher,
        Administrator
    }
    public partial class AuthorizationPage : Form
    {
        string connectionString = "Server=MSI;Database=Planner;Trusted_Connection=True;MultipleActiveResultSets=True;";
        private Role role;
        SqlConnection conn;
        public AuthorizationPage()
        {
            InitializeComponent();
        }
       
        private void AuthorizationPage_Load(object sender, EventArgs e)
        {
            comboBox1.DataSource = Enum.GetNames(typeof(Role));
            label1.Left = (ClientRectangle.Width - label1.Width)/2;
            label2.Left = (ClientRectangle.Width - textBox1.Width) / 2;
            label3.Left = (ClientRectangle.Width - textBox1.Width) / 2;
            label4.Left = (ClientRectangle.Width - textBox1.Width) / 2;
            textBox1.Left = (ClientRectangle.Width - textBox1.Width) / 2;
            textBox2.Left = (ClientRectangle.Width - textBox1.Width) / 2;
            comboBox1.Left = (ClientRectangle.Width - textBox1.Width) / 2;
            button1.Left = (ClientRectangle.Width - textBox1.Width) / 2;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            string email = textBox1.Text.Trim();
            string password = textBox2.Text.Trim();
            role = (Role)Enum.Parse(typeof(Role), comboBox1.SelectedItem.ToString());

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Введите логин и пароль.");
                return;
            }
            conn = new SqlConnection(connectionString);
            conn.Open();
                string table = role.ToString() + "s";
                string id_name = role.ToString().ToLower() + "_id";
                string query = $"SELECT {id_name} FROM {table} WHERE email = @email AND pass = @pass";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@pass", password);

                try
                {

                    object result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        int id = Convert.ToInt32(result);

                        if (role == Role.Student)
                        {
                            StudentPage studentPage = new StudentPage(conn, id, this);
                            studentPage.Show();
                            this.Hide();
                        }
                        if (role == Role.Administrator)
                        {
                            AdministratorTabels administrator = new AdministratorTabels(conn, this);
                            administrator.Show();
                            this.Hide();
                        }
                        if (role == Role.Teacher)
                        {
                            TeacherPage teacher = new TeacherPage(conn,id, this);
                            teacher.Show();
                            this.Hide();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Неверный логин или пароль.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка подключения к базе: " + ex.Message);
                }
            } 
        }

    }
}
