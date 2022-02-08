using cwiczenia_4_s16324.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace cwiczenia_4_s16324.Services
{

    public interface IDatabaseService
    {
        int AddProduct(Product product);
        int RegisterProduct(Request request);
    }
    public class MockDatabaseService : IDatabaseService
    {
        
        public int AddProduct(Product product)
        {
            return 1;
        }
        public int RegisterProduct(Request request)
        {
            return 1;
        }
    }

    public class SqlServerDatabaseService : IDatabaseService {

        private IConfiguration _configuration;

        public SqlServerDatabaseService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public int AddProduct(Product product)
        {

            var res = new List<Product>();
            int RowsAffected = 0;
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("ProductionDb")))
            {
                SqlCommand com = new SqlCommand();
                com.Connection = con;
                com.CommandText = "insert into Product(Name, Description, Price) " +
                    "values(@Name, @Description, @Price)";
                com.Parameters.AddWithValue("@Name", product.Name);
                com.Parameters.AddWithValue("@Description", product.Description);
                com.Parameters.AddWithValue("@Price", product.Price);
                con.Open();
                RowsAffected = com.ExecuteNonQuery();

            }
            return RowsAffected;
        }

        public Product GetProduct(int ProductId)
        {
            Product res = null;
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("ProductionDb")))
            {
                SqlCommand com = new SqlCommand();
                com.Connection = con;
                com.Parameters.AddWithValue("@ProductId", ProductId);
                com.CommandText = "SELECT * FROM Product WHERE IdProduct=@ProductId";
                con.Open();
                SqlDataReader dr = com.ExecuteReader();
                if (dr.Read())
                {
                    res = new Product
                    {
                        IdProduct = Int32.Parse(dr["IdProduct"].ToString()),
                        Name = dr["Name"].ToString(),
                        Description = dr["Description"].ToString(),
                        Price = Decimal.Parse(dr["Price"].ToString())
                    };
                }
            }

            return res;
        }

        public Warehouse GetWarehouse(int WarehouseId)
        {
            Warehouse res = null;
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("ProductionDb")))
            {
                SqlCommand com = new SqlCommand();
                com.Connection = con;
                com.Parameters.AddWithValue("@WarehouseId", WarehouseId);
                com.CommandText = "SELECT * FROM Warehouse WHERE IdWarehouse=@WarehouseId";
                con.Open();
                SqlDataReader dr = com.ExecuteReader();
                if (dr.Read())
                {
                    res = new Warehouse
                    {
                        IdWarehouse = Int32.Parse(dr["IdWarehouse"].ToString()),
                        Name = dr["Name"].ToString(),
                        Address = dr["Address"].ToString()
                    };
                }
            }

            return res;
        }

        public Order GetOrder(int ProductId, int Amount, DateTime CreatedAt)
        {
            Order res = null;
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("ProductionDb")))
            {
                SqlCommand com = new SqlCommand();
                com.Connection = con;
                com.Parameters.AddWithValue("@ProductId", ProductId);
                com.Parameters.AddWithValue("@Amount", Amount);
                com.Parameters.AddWithValue("@CreatedAt", CreatedAt);
                com.CommandText = "SELECT * FROM \"Order\" WHERE IdProduct=@ProductId AND Amount=@Amount AND CONVERT(DATE, [CreatedAt], 103) <= @CreatedAt";
                con.Open();
                var polishFormat = new CultureInfo("pl-PL");
                SqlDataReader dr = com.ExecuteReader();
                if (dr.Read())
                {
                    if(dr["FulfilledAt"].ToString()=="" || dr["FulfilledAt"].ToString() == null)
                    {
                        res = new Order
                        {
                            IdOrder = Int32.Parse(dr["IdOrder"].ToString()),
                            IdProduct = Int32.Parse(dr["IdProduct"].ToString()),
                            Amount = Int32.Parse(dr["Amount"].ToString()),
                            CreatedAt = DateTime.ParseExact(dr["CreatedAt"].ToString(), "dd.MM.yyyy HH:mm:ss", polishFormat)
                        };
                    }
                    else
                    {
                        res = new Order
                        {
                            IdOrder = Int32.Parse(dr["IdOrder"].ToString()),
                            IdProduct = Int32.Parse(dr["IdProduct"].ToString()),
                            Amount = Int32.Parse(dr["Amount"].ToString()),
                            CreatedAt = DateTime.ParseExact(dr["CreatedAt"].ToString(), "dd.MM.yyyy HH:mm:ss", polishFormat),
                            FulfilledAt = DateTime.ParseExact(dr["FulfilledAt"].ToString(), "dd.MM.yyyy HH:mm:ss", polishFormat)
                        };
                    }
                    
                }
            }

            return res;
        }

        public ProductWarehouse GetProductWarehouse(int OrderId)
        {
            ProductWarehouse res = null;
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("ProductionDb")))
            {
                SqlCommand com = new SqlCommand();
                com.Connection = con;
                com.Parameters.AddWithValue("@OrderId", OrderId);
                com.CommandText = "SELECT * FROM Product_Warehouse WHERE IdOrder=@OrderId";
                con.Open();
                var polishFormat = new CultureInfo("pl-PL");
                SqlDataReader dr = com.ExecuteReader();
                if (dr.Read())
                {
                    res = new ProductWarehouse
                    {
                        IdProductWarehouse = Int32.Parse(dr["IdProductWarehouse"].ToString()),
                        IdWarehouse = Int32.Parse(dr["IdWarehouse"].ToString()),
                        IdProduct = Int32.Parse(dr["IdProduct"].ToString()),
                        IdOrder = Int32.Parse(dr["IdOrder"].ToString()),
                        Amount = Int32.Parse(dr["Amount"].ToString()),
                        Price = Decimal.Parse(dr["Price"].ToString()),
                        CreatedAt = DateTime.ParseExact(dr["CreatedAt"].ToString(), "dd.MM.yyyy HH:mm:ss", polishFormat)
                    };
                }
            }

            return res;
        }

        public int FulfillOrder(Request request, int OrderId, decimal Price, DateTime Timestamp)
        {
            decimal PriceDecimal = Decimal.Parse(Price.ToString());
            decimal PriceCalculated = request.Amount * PriceDecimal;
            int RowsAffected = 0;
            int IdInserted;
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("ProductionDb")))
            {
                SqlCommand com = new SqlCommand();
                com.Connection = con;
                com.Parameters.AddWithValue("@OrderId", OrderId);
                com.Parameters.AddWithValue("@Timestamp", Timestamp);
                com.CommandText = "UPDATE \"Order\" set FulfilledAt=@Timestamp WHERE IdOrder=@OrderId";
                con.Open();
                DbTransaction tran = con.BeginTransaction();
                com.Transaction = (SqlTransaction)tran;
                try
                {
                    RowsAffected = com.ExecuteNonQuery();

                    com.Parameters.Clear();
                    com.CommandText = "insert into Product_Warehouse(IdWarehouse, IdProduct, IdOrder, Amount, Price, CreatedAt) " +
                        "values(@WarehouseId, @ProductId, @OrderId, @Amount, @Price, @CreatedAt)";
                    com.Parameters.AddWithValue("@WarehouseId", request.IdWarehouse);
                    com.Parameters.AddWithValue("@ProductId", request.IdProduct);
                    com.Parameters.AddWithValue("@OrderId", OrderId);
                    com.Parameters.AddWithValue("@Amount", request.Amount);
                    com.Parameters.AddWithValue("@Price", PriceCalculated);
                    com.Parameters.AddWithValue("@CreatedAt", Timestamp);
                    RowsAffected = com.ExecuteNonQuery();
                    IdInserted = Convert.ToInt32(com.ExecuteScalar());

                    tran.Commit();
                }
                catch (SqlException e)
                {
                    //throw (e);
                    tran.Rollback();
                    return -6;
                } catch(Exception e)
                {
                    throw(e);
                    tran.Rollback();
                    return -7;
                }

            }

            return IdInserted;
        }

        public int RegisterProduct(Request request)
        {
            //get product
            Product product = GetProduct(request.IdProduct);
            Warehouse warehouse = GetWarehouse(request.IdWarehouse);
            if (product == null)
            {
                return -1;
            }
            if(warehouse == null)
            {
                return -2;
            }
            //request amount validation - has to be moved somewhere else
            if(request.Amount<0 || request.Amount == null)
            {
                return -3;
            }

            Order order = GetOrder(request.IdProduct, request.Amount, request.CreatedAt);
            if (order == null)
            {
                return -4;
            }

            DateTime Timestamp = DateTime.Now;
            ProductWarehouse pw = GetProductWarehouse(order.IdOrder);
            if (pw != null)
            {
                return -5;
            }

            return FulfillOrder(request, order.IdOrder, product.Price, Timestamp);

            //return 1;
        }

    }

}
