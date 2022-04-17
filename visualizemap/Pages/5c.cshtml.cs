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
    public class _5cModel : PageModel
    {
        public string jsontext;
        //public int year;
        private readonly ILogger<_5cModel> _logger;

        public _5cModel(ILogger<_5cModel> logger)
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
            string CS = "Data Source=LAPTOP-H78D00F6\\MSSQLSERVER144;Initial Catalog=SpatialDB3;Integrated Security=True";   //DB Chan
            //string CS = "Data Source=LAPTOP-QG030MKA;Initial Catalog=SpatialDB3;Integrated Security=True";            //DB Pond
            SqlConnection con = new SqlConnection(CS);
            string command = "declare @AroundThailand geometry = 'POLYGON EMPTY' select @AroundThailand = @AroundThailand.STUnion(Geom.MakeValid()) from world where NAME = 'Thailand' select city, country, latitude, longitude from AirPollutionPM25 where country in(select Name from world where Geom.MakeValid().STTouches(@AroundThailand) = 1) and Year = 2018";

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
