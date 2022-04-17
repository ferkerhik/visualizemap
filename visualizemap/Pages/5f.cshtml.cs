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
    public class _5fModel : PageModel
    {
        public string jsontext;
        public int year;
        private readonly ILogger<_5fModel> _logger;

        public _5fModel(ILogger<_5fModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            database();
        }

        public void OnPost()
        {

            year = Int32.Parse(Request.Form["year"]);
            database();
        }
        public void database()
        {
            //string CS = "Data Source=LAPTOP-H78D00F6\\MSSQLSERVER144;Initial Catalog=SpatialDB3;Integrated Security=True";   //DB Chan
            string CS = "Data Source=LAPTOP-QG030MKA;Initial Catalog=SpatialDB3;Integrated Security=True";            //DB Pond
            SqlConnection con = new SqlConnection(CS);
            string command = "select city, country, latitude, longitude from AirPollutionPM25 where wbinc16_text = 'Low income' and Year = '" + year + "'";

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
