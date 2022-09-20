using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testMysql
{
    class Program
    {
        static void Main(string[] args)
        {
            //Database=dbname;Data Source=192.168.1.1;Port=3306;User Id=root;Password=****;Charset=utf8;TreatTinyAsBoolean=false;
            string connstr = "data source=localhost;database=test_tb;user id=root;password=123456;pooling=false;charset=utf8";//pooling代表是否使用连接池
            MySqlConnection conn = new MySqlConnection(connstr);
            if(conn.State==ConnectionState.Open)  
            {  
                conn.Close();  
            }
            try
            {
                conn.Open();
                string sql3 = "insert into tb_emp1 (name,turbidity) values ('XXX','2.1')";
                MySqlCommand cmd3 = new MySqlCommand(sql3, conn);
                int s = cmd3.ExecuteNonQuery();
                if (s == 0)
                    Console.WriteLine("false");
                else
                    Console.WriteLine("success");
                conn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }  
            finally 
            {  
              conn.Close();  
            }  
            Console.ReadLine();















            //string sql = "select * from tb_emp1";
            //MySqlCommand cmd = new MySqlCommand(sql, conn);
            //conn.Open();
            //MySqlDataReader reader = cmd.ExecuteReader();
            //Console.WriteLine("id\t姓名\t浊度\t余氯\t电导率");


            //while (reader.Read())
            //{
            //    Console.Write(reader.GetInt32("id") + "\t");
            //    if (reader.IsDBNull(1))
            //    {
            //        Console.Write("空\t");
            //    }
            //    else
            //    {
            //        Console.Write(reader.GetString("name") + "\t");
            //    }
            //    if (reader.IsDBNull(2))
            //    {
            //        Console.Write("空\n");
            //    }
            //    else
            //    {
            //        Console.Write(reader.GetFloat("turbidity") + "\t");
            //    }

            //    if (reader.IsDBNull(3))
            //    {
            //        Console.Write("空\n");
            //    }
            //    else
            //    {
            //        Console.Write(reader.GetFloat("residualchlorine") + "\t");
            //    }


            //    if (reader.IsDBNull(4))
            //    {
            //        Console.Write("空\n");
            //    }
            //    else
            //    {
            //        Console.Write(reader.GetFloat("conductivity") + "\n");
            //    }
            //}

            //conn.Close();


            //string sql2 = "select conductivity from tb_emp1 where id=1";
            //MySqlCommand cmd2 = new MySqlCommand(sql2, conn);
            //conn.Open();
            //string names = cmd2.ExecuteScalar().ToString();
            //Console.WriteLine(names);
            //conn.Close();




        }
    }
}
