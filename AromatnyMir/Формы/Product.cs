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
    public partial class Product : Form
    {
        db dataBase = new db();
        DataTransfer dataTransfer;

        // Конструктор, принимающий аргументы
        public Product(DataTransfer dataTransfer)
        {
            InitializeComponent();
            this.dataTransfer = dataTransfer;
        }

        private void productBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.productBindingSource.EndEdit();
            this.tableAdapterManager.UpdateAll(this.aromaMirDataSet);
        }

        private void Product_Load(object sender, EventArgs e)
        {
            this.productTableAdapter.Fill(this.aromaMirDataSet.Product);
            UserName.Text = dataTransfer.UserLogin;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            try
            {
                int productId = (int)productDataGridView.SelectedRows[0].Cells["dataGridViewTextBoxColumn1"].Value;
                List<int> productList = dataTransfer.ProductIds.ToList(); // Преобразуем массив в список для удобства добавления элементов
                productList.Add(productId); // Добавляем идентификатор товара в список
                dataTransfer.ProductIds = productList.ToArray(); // Преобразуем список обратно в массив

                Cart cart = new Cart(dataTransfer);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка: " + ex.Message);
            }
        }
    }
}
