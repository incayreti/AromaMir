using System;
using System.Windows.Forms;

namespace AromatnyMir.Формы
{
    public partial class Product : Form
    {
        db dataBase = new db();
        DataTransfer dataTransfer;

        public Product(DataTransfer dataTransfer)
        {
            InitializeComponent();
            this.dataTransfer = dataTransfer;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            try
            {
                // Получаем выбранный продукт из DataGridView с продуктами
                DataGridViewRow selectedRow = productDataGridView.SelectedRows[0];

                // Получаем необходимую информацию о продукте
                int productId = (int)selectedRow.Cells["dataGridViewTextBoxColumn1"].Value;
                string productName = selectedRow.Cells["dataGridViewTextBoxColumn3"].Value.ToString();
                decimal productPrice = (decimal)selectedRow.Cells["dataGridViewTextBoxColumn5"].Value;

                // Проверяем, что количество товара больше нуля
                int quantity = (int)selectedRow.Cells["dataGridViewTextBoxColumn11"].Value;
                if (quantity <= 0)
                {
                    MessageBox.Show("Недостаточно товара на складе!");
                    return;
                }

                // Добавляем продукт в корзину DataGridView
                cartDataGridView.Rows.Add(productId, productName, productPrice, 1);

                // Уменьшаем количество товара в главной таблице
                selectedRow.Cells["dataGridViewTextBoxColumn11"].Value = quantity - 1;

                // Обновляем количество товаров в корзине и общую сумму
                UpdateCartCount();
                CalculateTotalAmount();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка: " + ex.Message);
            }
        }


        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                // Получаем выбранный продукт из корзины DataGridView
                DataGridViewRow selectedRow = cartDataGridView.SelectedRows[0];

                // Получаем информацию о продукте в корзине
                int productId = (int)selectedRow.Cells["ProductIdColumn"].Value;
                string productName = selectedRow.Cells["ProductNameColumn"].Value.ToString();
                decimal productPrice = (decimal)selectedRow.Cells["ProductPriceColumn"].Value;

                // Удаляем выбранный продукт из корзины DataGridView
                cartDataGridView.Rows.Remove(selectedRow);

                // Увеличиваем количество товара в главной таблице
                foreach (DataGridViewRow row in productDataGridView.Rows)
                {
                    if ((int)row.Cells["dataGridViewTextBoxColumn1"].Value == productId)
                    {
                        int quantity = Convert.ToInt32(row.Cells["dataGridViewTextBoxColumn11"].Value);
                        row.Cells["dataGridViewTextBoxColumn11"].Value = quantity + 1;
                        break;
                    }
                }

                // Обновляем количество товаров в корзине и общую сумму
                UpdateCartCount();
                CalculateTotalAmount();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка: " + ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }


        private void Product_Load(object sender, EventArgs e)
        {
            this.productTableAdapter.Fill(this.aromaMirDataSet.Product);
            UserName.Text = dataTransfer.UserLogin;
        }

        private void UpdateCartCount()
        {
            cartCount.Text = cartDataGridView.Rows.Count.ToString();
        }

        private void CalculateTotalAmount()
        {
            decimal totalAmount = 0;
            foreach (DataGridViewRow row in cartDataGridView.Rows)
            {
                decimal itemPrice = (decimal)row.Cells["ProductPriceColumn"].Value;
                int itemQuantity = (int)row.Cells["QuantityColumn"].Value;
                totalAmount += itemPrice * itemQuantity;
            }
            totalAmountLabel.Text = totalAmount.ToString();
        }
    }
}
