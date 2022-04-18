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
using System.Text;
using System.Configuration;
using ClosedXML.Excel;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;

namespace visualizemap.Pages
{
    public class ReadDataModel : PageModel
    {
        private IHostingEnvironment _environment;
        private string _cs = "Data Source=LAPTOP-H78D00F6\\MSSQLSERVER144;Initial Catalog=SpatialDB3;Integrated Security=True"; // DB Chan
        //private string _cs = "Data Source=LAPTOP-H78D00F6\\MSSQLSERVER144;Initial Catalog=SpatialDB3;Integrated Security=True"; // DB Pond

        //public FileResult OnReadData() {
            
        //}

        public ReadDataModel(IHostingEnvironment environment)
        {
            _environment = environment;
        }

        public void OnPostUpload(IFormFile file) {
            string path = Path.Combine(_environment.WebRootPath, "uploads");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string fileName = Path.GetFileName(file.FileName);

            using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
            {
                file.CopyTo(stream);
            }

            string excelPath = Path.Combine(path, fileName);

            var workbook = new XLWorkbook(excelPath);
            var ws1 = workbook.Worksheet(1);
            var usedRows = ws1.RowsUsed().Skip(1);

            string sql = @"insert into AirPollutionPM25 (
                country, city, Year, pm25, latitude, longitude, population, wbinc16_text, Region, conc_pm25, color_pm25, Geom)
                values (@country, @city, @year, @pm25, @latitude, @longitude, @population, @wbinc16_text, @Region, @conc_pm25, @color_pm25, @Geom)
            ";

            using (SqlConnection con = new SqlConnection(_cs))
            {
                con.Open();

                foreach (var dataRow in usedRows)
                {
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@country", dataRow.Cell(1).GetValue<string>());
                        cmd.Parameters.AddWithValue("@city", dataRow.Cell(2).GetValue<string>());
                        cmd.Parameters.AddWithValue("@year", dataRow.Cell(3).GetValue<int>());
                        cmd.Parameters.AddWithValue("@pm25", dataRow.Cell(4).GetValue<double>());
                        cmd.Parameters.AddWithValue("@latitude", dataRow.Cell(5).GetValue<double>());
                        cmd.Parameters.AddWithValue("@longitude", dataRow.Cell(6).GetValue<double>());
                        cmd.Parameters.AddWithValue("@population", dataRow.Cell(7).GetValue<double>());
                        cmd.Parameters.AddWithValue("@wbinc16_text", dataRow.Cell(8).GetValue<string>());
                        cmd.Parameters.AddWithValue("@Region", dataRow.Cell(9).GetValue<string>());
                        cmd.Parameters.AddWithValue("@conc_pm25", dataRow.Cell(10).GetValue<string>());
                        cmd.Parameters.AddWithValue("@color_pm25", dataRow.Cell(11).GetValue<string>());
                        cmd.Parameters.AddWithValue("@Geom", DBNull.Value);
                        
                        cmd.ExecuteNonQuery();
                    }
                }

                sql = @"update AirPollutionPM25
                set Geom = geometry::STGeomFromText('POINT(' + convert(nvarchar(255), longitude) + ' ' + convert(nvarchar(255), latitude) + ')', 4326)
                ";

                using (SqlCommand cmd = new SqlCommand(sql, con)) {
                    cmd.ExecuteNonQuery();
                }

                con.Close();
            }
        }
    }
}
