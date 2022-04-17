using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace visualizemap.Pages
{
    public class sql_queryModel : PageModel
    {
        public string jsontext;
        public void OnGet()
        {
            database();
        }

        public void database()
        {
            string CS = "Data Source=LAPTOP-H78D00F6\\MSSQLSERVER144;Initial Catalog=SpatialDB3;Integrated Security=True";   //DB Chan
            //string CS = "Data Source=LAPTOP-QG030MKA;Initial Catalog=SpatialDB3;Integrated Security=True";            //DB Pond
            SqlConnection con = new SqlConnection(CS);
            //string command = "select latitude,longitude,color_pm25 from AirPolluTionPM25 where year = '" + year + "'";//5(a)
            //string command = "select latitude,longitude,color_pm25 from AirPolluTionPM25";
            string command = "select country,city,pm25 from AirPollutionPM25 where pm25 >50 and Year = 2015 order by pm25 asc";

            SqlDataAdapter da = new SqlDataAdapter(command, con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            string[] columnNames = (from dc in dt.Columns.Cast<DataColumn>()
                                    select dc.ColumnName).ToArray();
            Console.WriteLine(dt.DataSet);

            Console.WriteLine("Test2");

            jsontext = JsonConvert.SerializeObject(dt);
            Console.WriteLine(jsontext);
        }
    }
}
