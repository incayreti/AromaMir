using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AromatnyMir.Формы
{
    public partial class Cart : Form
    {
        /*
        db dataBase = new db();
        DataTransfer dataTransfer;

        // Конструктор, принимающий аргументы
        public Cart(DataTransfer dataTransfer)
        {
            InitializeComponent();
            this.dataTransfer = dataTransfer; // Установка переданного объекта dataTransfer
        }

        private void FillDataGridView()
        {
            dataGridView1.Rows.Clear();

            if (dataTransfer.ProductIds != null) // Проверяем наличие данных перед выводом
            {
                foreach (int productId in dataTransfer.ProductIds)
                {
                    dataGridView1.Rows.Add(productId);
                }
            }
        }
        */

        private void Cart_Load(object sender, EventArgs e)
        {
            //FillDataGridView();
        }
    }
}
