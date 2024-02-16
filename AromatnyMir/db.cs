using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AromatnyMir
{
    internal class db
    {
        SqlConnection sqlConnection = new SqlConnection(@"Data Source=HOME-PC\MSSQLSERVER01;Initial Catalog=AromaMir;Integrated Security=True;TrustServerCertificate=True");

        public void openConnection()
        {
            if (sqlConnection.State == System.Data.ConnectionState.Closed) { sqlConnection.Open(); }
        }

        public void closeConnection()
        {
            if (sqlConnection.State == System.Data.ConnectionState.Open) { sqlConnection.Close(); }
        }

        public SqlConnection getConnection() { return sqlConnection; }

        // Метод для добавления заказа в базу данных
        public void AddOrder(int productId, int orderCount, DateTime orderCreateDate, DateTime orderDeliveryDate, int pickupPointId, int userId)
        {
            openConnection();
            using (SqlCommand cmd = new SqlCommand("INSERT INTO [Order] (OrderStatus, ProductArticleId, OrderCount, OrderCreateDate, OrderDeliveryDate, IdPickupPoint, UserID, OrderGetCode) VALUES (@OrderStatus, @ProductArticleId, @OrderCount, @OrderCreateDate, @OrderDeliveryDate, @IdPickupPoint, @UserID, @OrderGetCode)", getConnection()))
            {
                cmd.Parameters.AddWithValue("@OrderStatus", "В обработке");
                cmd.Parameters.AddWithValue("@ProductArticleId", productId);
                cmd.Parameters.AddWithValue("@OrderCount", orderCount);
                cmd.Parameters.AddWithValue("@OrderCreateDate", orderCreateDate);
                cmd.Parameters.AddWithValue("@OrderDeliveryDate", orderDeliveryDate);
                cmd.Parameters.AddWithValue("@IdPickupPoint", pickupPointId);
                cmd.Parameters.AddWithValue("@UserID", userId);
                cmd.Parameters.AddWithValue("@OrderGetCode", new Random().Next(100000, 999999)); // Генерируем случайный код заказа
                cmd.ExecuteNonQuery();
            }
            closeConnection();
        }

        // Метод для получения списка имен пунктов выдачи
        public List<string> GetPickupPoints()
        {
            List<string> pickupPoints = new List<string>();
            openConnection();
            using (SqlCommand cmd = new SqlCommand("SELECT CityName FROM PickupPoint", getConnection()))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        pickupPoints.Add(reader.GetString(0));
                    }
                }
            }
            closeConnection();
            return pickupPoints;
        }
    }
}
