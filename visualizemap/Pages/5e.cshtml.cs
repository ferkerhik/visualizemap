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
    public class _5eModel : PageModel
    {
        public string jsontext;
        //public int year;
        private readonly ILogger<_5eModel> _logger;

        public _5eModel(ILogger<_5eModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            database();
        }

        public void OnPost()
        {

            //year = Int32.Parse(Request.Form["year"]);
            database();
        }
        public void database()
        {
            //string CS = "Data Source=LAPTOP-H78D00F6\\MSSQLSERVER144;Initial Catalog=SpatialDB3;Integrated Security=True";   //DB Chan
            string CS = "Data Source=LAPTOP-QG030MKA;Initial Catalog=SpatialDB3;Integrated Security=True";            //DB Pond
            SqlConnection con = new SqlConnection(CS);
            string command = "declare @bk geometry select @bk = Geom from AirPollutionPM25 where city = 'Bangkok' select distinct top 50 city, country, latitude, longitude, Geom.MakeValid().STDistance(@bk) dist from AirPollutionPM25 where city != 'Bangkok' order by dist asc";

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
