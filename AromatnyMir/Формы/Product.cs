using System;
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
            try
            {
                // Проверяем, выбран ли какой-либо пункт выдачи
                if (comboBox1.SelectedItem == null)
                {
                    MessageBox.Show("Выберите пункт выдачи!");
                    return;
                }

                // Получаем строку, выбранную в комбо боксе
                string selectedPickupPoint = comboBox1.SelectedItem.ToString();

                // Разбиваем строку на части по пробелу
                string[] parts = selectedPickupPoint.Split(' ');

                // Проверяем, что строка разбилась на три части (ID, улица, номер дома)
                if (parts.Length != 3)
                {
                    MessageBox.Show("Неправильный формат строки пункта выдачи!");
                    return;
                }

                // Получаем ID пункта выдачи
                int pickupPointID = int.Parse(parts[0]);

                // Создаем заказ
                CreateOrder(pickupPointID);

                // Очищаем корзину
                cartDataGridView.Rows.Clear();

                // Обновляем количество товаров в корзине и общую сумму
                UpdateCartCount();
                CalculateTotalAmount();

                MessageBox.Show("Заказ успешно оформлен!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при оформлении заказа: " + ex.Message);
            }
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

        private void CreateOrder(int pickupPointID)
        {
            // Получаем UserID по логину
            int userID = dataBase.GetUserIDByLogin(dataTransfer.UserLogin);

            // Перебираем все товары в корзине и создаем заказ для каждого товара
            foreach (DataGridViewRow row in cartDataGridView.Rows)
            {
                int productId = (int)row.Cells["ProductIdColumn"].Value;
                int quantity = (int)row.Cells["QuantityColumn"].Value;
                decimal productPrice = (decimal)row.Cells["ProductPriceColumn"].Value;

                // Создаем соединение с базой данных
                using (SqlConnection connection = dataBase.getConnection())
                {
                    connection.Open();

                    // Создаем команду для вставки нового заказа в таблицу "Order"
                    SqlCommand cmd = new SqlCommand("INSERT INTO [Order] (OrderStatus, ProductArticleId, OrderCount, OrderCreateDate, OrderDeliveryDate, IdPickupPoint, UserID, OrderGetCode) " +
                                                    "VALUES (@OrderStatus, @ProductArticleId, @OrderCount, @OrderCreateDate, @OrderDeliveryDate, @IdPickupPoint, @UserID, @OrderGetCode)", connection);

                    // Задаем параметры для команды
                    cmd.Parameters.AddWithValue("@OrderStatus", "Новый");
                    cmd.Parameters.AddWithValue("@ProductArticleId", productId);
                    cmd.Parameters.AddWithValue("@OrderCount", quantity);
                    cmd.Parameters.AddWithValue("@OrderCreateDate", DateTime.Now);
                    cmd.Parameters.AddWithValue("@OrderDeliveryDate", DateTime.Now.AddDays(5));
                    cmd.Parameters.AddWithValue("@IdPickupPoint", pickupPointID);
                    cmd.Parameters.AddWithValue("@UserID", userID);
                    cmd.Parameters.AddWithValue("@OrderGetCode", GenerateOrderGetCode()); // Генерация уникального кода для заказа

                    // Выполняем команду
                    cmd.ExecuteNonQuery();
                }
            }
        }


        // Метод для генерации уникального кода заказа
        private int GenerateOrderGetCode()
        {
            // Здесь может быть ваша логика генерации уникального кода
            return new Random().Next(1000, 9999); // Пример: случайное число от 1000 до 9999
        }

    }
}
