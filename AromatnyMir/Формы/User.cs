using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AromatnyMir.Формы;
using System.Data.SqlClient;
using System.Security.Cryptography.X509Certificates;


namespace AromatnyMir
{
    public partial class Form1 : Form
    {
        db dataBase = new db();

        DataTransfer dataTransfer = new DataTransfer();
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string login = loginTextBox.Text;
            string password = passwordTextBox.Text;

            dataBase.openConnection();
            SqlCommand cmd = new SqlCommand("SELECT * FROM [User] WHERE UserLogin = @login AND UserPassword = @password", dataBase.getConnection());
            cmd.Parameters.AddWithValue("@login", login);
            cmd.Parameters.AddWithValue("@password", password);
            int count = (int)cmd.ExecuteScalar();
            dataBase.closeConnection();

            if (count > 0)
            {
                MessageBox.Show("Авторизация прошла успешно!");

                dataTransfer.UserLogin = login;

                this.Hide();
                Product product = new Product(dataTransfer);
                product.ShowDialog();
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль!");
            }
        }

    }
}