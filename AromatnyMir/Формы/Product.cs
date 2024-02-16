using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
                DataGridViewRow selectedRow = productDataGridView.SelectedRows[0];
                int productId = (int)selectedRow.Cells["dataGridViewTextBoxColumn1"].Value;
                string productName = selectedRow.Cells["dataGridViewTextBoxColumn3"].Value.ToString();
                decimal productPrice = (decimal)selectedRow.Cells["dataGridViewTextBoxColumn5"].Value;
                int quantity = (int)selectedRow.Cells["dataGridViewTextBoxColumn11"].Value;
                if (quantity <= 0)
                {
                    MessageBox.Show("Недостаточно товара на складе!");
                    return;
                }
                cartDataGridView.Rows.Add(productId, productName, productPrice, 1);
                selectedRow.Cells["dataGridViewTextBoxColumn11"].Value = quantity - 1;
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
            try
            {
                // Получаем выбранный пункт выдачи из comboBox1
                string pickupPointName = comboBox1.SelectedItem.ToString();

                // Получаем id пункта выдачи по его имени
                int pickupPointId = GetPickupPointIdByName(pickupPointName);

                // Получаем id пользователя по его логину
                int userId = GetUserIdByLogin(dataTransfer.UserLogin);

                // Проходим по каждой строке в корзине и добавляем заказы в базу данных
                foreach (DataGridViewRow row in cartDataGridView.Rows)
                {
                    int productId = (int)row.Cells["ProductIdColumn"].Value;
                    int orderCount = (int)row.Cells["QuantityColumn"].Value;
                    DateTime orderCreateDate = DateTime.Now;
                    DateTime orderDeliveryDate = DateTime.Now.AddDays(5);

                    // Вызываем метод AddOrder для добавления заказа в базу данных
                    db.AddOrder(productId, orderCount, orderCreateDate, orderDeliveryDate, pickupPointId, userId);

                    // Увеличиваем количество товара в главной таблице
                    foreach (DataGridViewRow productRow in productDataGridView.Rows)
                    {
                        if ((int)productRow.Cells["dataGridViewTextBoxColumn1"].Value == productId)
                        {
                            int quantity = Convert.ToInt32(productRow.Cells["dataGridViewTextBoxColumn11"].Value);
                            productRow.Cells["dataGridViewTextBoxColumn11"].Value = quantity + orderCount;
                            break;
                        }
                    }
                }

                // Очищаем корзину после оформления заказа
                cartDataGridView.Rows.Clear();

                // Обновляем количество товаров в корзине и общую сумму
                UpdateCartCount();
                CalculateTotalAmount();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка: " + ex.Message);
            }
        }

        // Метод для получения id пункта выдачи по его имени
        private int GetPickupPointIdByName(string pickupPointName)
        {
            int pickupPointId = 0;
            // Предполагается, что вам нужно получить id пункта выдачи из базы данных, используя его имя
            // Здесь предполагается, что у вас есть метод db.GetPickupPointIdByName, который выполняет эту задачу
            // pickupPointId = db.GetPickupPointIdByName(pickupPointName);
            return pickupPointId;
        }

        // Метод для получения id пользователя по его логину
        private int GetUserIdByLogin(string userLogin)
        {
            int userId = 0;
            // Предполагается, что вам нужно получить id пользователя из базы данных, используя его логин
            // Здесь предполагается, что у вас есть метод db.GetUserIdByLogin, который выполняет эту задачу
            // userId = db.GetUserIdByLogin(userLogin);
            return userId;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                DataGridViewRow selectedRow = cartDataGridView.SelectedRows[0];
                int productId = (int)selectedRow.Cells["ProductIdColumn"].Value;
                string productName = selectedRow.Cells["ProductNameColumn"].Value.ToString();
                decimal productPrice = (decimal)selectedRow.Cells["ProductPriceColumn"].Value;
                cartDataGridView.Rows.Remove(selectedRow);
                foreach (DataGridViewRow row in productDataGridView.Rows)
                {
                    if ((int)row.Cells["dataGridViewTextBoxColumn1"].Value == productId)
                    {
                        int quantity = Convert.ToInt32(row.Cells["dataGridViewTextBoxColumn11"].Value);
                        row.Cells["dataGridViewTextBoxColumn11"].Value = quantity + 1;
                        break;
                    }
                }
                UpdateCartCount();
                CalculateTotalAmount();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка: " + ex.Message);
            }
        }

        public List<string> GetPickupPoints()
        {
            List<string> pickupPoints = new List<string>();
            using (SqlCommand cmd = new SqlCommand("SELECT CityName FROM PickupPoint", db.getConnection()))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        pickupPoints.Add(reader.GetString(0));
                    }
                }
            }
            return pickupPoints;
        }

        // В форме Product в методе Product_Load заполните ComboBox пунктами выдачи
        private void Product_Load(object sender, EventArgs e)
        {
            this.productTableAdapter.Fill(this.aromaMirDataSet.Product);
            UserName.Text = dataTransfer.UserLogin;

            // Заполнение ComboBox пунктами выдачи
            List<string> pickupPoints = dataBase.GetPickupPoints();
            comboBox1.DataSource = pickupPoints;
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
