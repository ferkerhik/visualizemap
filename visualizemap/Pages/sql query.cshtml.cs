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

namespace visualizemap.Pages
{
    public class sql_queryModel : PageModel
    {
        private string _cs = "Data Source=LAPTOP-H78D00F6\\MSSQLSERVER144;Initial Catalog=SpatialDB3;Integrated Security=True"; // DB Chan
        //private string _cs = "Data Source=LAPTOP-H78D00F6\\MSSQLSERVER144;Initial Catalog=SpatialDB3;Integrated Security=True"; // DB Pond

        public string table;
        private DataTable _dt;
        public string type, country, year, color;

        public void OnGet()
        {
            string queryType = Request.Query["query-type"];
            type = queryType;

            country = Request.Query["country"];
            year = Request.Query["year"];
            color = Request.Query["color_pm25"];

            if (queryType == "1")
            {
                _dt = this._getQuery1();
                table = _renderHTML();
            }
            else if (queryType == "2") {
                _dt = this._getQuery2();
                table = _renderHTML();
            }
            else if (queryType == "3") {
                _dt = this._getQuery3(country);
                table = _renderHTML();
            }
            else if (queryType == "4") {
                _dt = this._getQuery4(year, color);
                table = _renderHTML();
            }
        }

        private string _renderHTML() {
            StringBuilder html = new StringBuilder();

            //Table start.
            html.Append("<table border = '1'>");

            //Building the Header row.
            html.Append("<tr>");
            foreach (DataColumn column in _dt.Columns)
            {
                html.Append("<th>");
                html.Append(column.ColumnName);
                html.Append("</th>");
            }
            html.Append("</tr>");

            //Building the Data rows.
            foreach (DataRow row in _dt.Rows)
            {
                html.Append("<tr>");
                foreach (DataColumn column in _dt.Columns)
                {
                    html.Append("<td>");
                    html.Append(row[column.ColumnName]);
                    html.Append("</td>");
                }
                html.Append("</tr>");
            }

            //Table end.
            html.Append("</table>");

            return html.ToString();
        }

        private DataTable _getQuery1() {
            using (SqlConnection con = new SqlConnection(_cs))
            {
                using (SqlCommand cmd = new SqlCommand("select country,city,pm25 from AirPollutionPM25 where pm25 >50 and Year = 2015 order by pm25 asc"))
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter())
                    {
                        cmd.Connection = con;
                        sda.SelectCommand = cmd;
                        using (DataTable dt = new DataTable("query"))
                        {
                            sda.Fill(dt);
                            return dt;
                        }
                    }
                }
            }
        }

        private DataTable _getQuery2()
        {
            using (SqlConnection con = new SqlConnection(_cs))
            {
                using (SqlCommand cmd = new SqlCommand("select country,AVG(pm25) average_pm from AirPollutionPM25 group by country order by average_pm desc"))
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter())
                    {
                        cmd.Connection = con;
                        sda.SelectCommand = cmd;
                        using (DataTable dt = new DataTable("query"))
                        {
                            sda.Fill(dt);
                            return dt;
                        }
                    }
                }
            }
        }

        private DataTable _getQuery3(string country)
        {
            using (SqlConnection con = new SqlConnection(_cs))
            {
                using (SqlCommand cmd = new SqlCommand("select country,city,year,pm25 from AirPollutionPM25 where country = '"+country+"'"))
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter())
                    {
                        cmd.Connection = con;
                        sda.SelectCommand = cmd;
                        using (DataTable dt = new DataTable("query"))
                        {
                            sda.Fill(dt);
                            return dt;
                        }
                    }
                }
            }
        }

        private DataTable _getQuery4(string year, string color)
        {
            using (SqlConnection con = new SqlConnection(_cs))
            {
                using (SqlCommand cmd = new SqlCommand("select SUM(population) as Allpopulation from AirPollutionPM25 where Year = '" + year+"' and color_pm25 = '"+color+"'"))
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter())
                    {
                        cmd.Connection = con;
                        sda.SelectCommand = cmd;
                        using (DataTable dt = new DataTable("query"))
                        {
                            sda.Fill(dt);
                            return dt;
                        }
                    }
                }
            }
        }

        public FileResult OnPostExport()
        {
            DataTable dt = null;

            switch (Request.Form["type"]) {
                case "1":
                    dt = this._getQuery1();
                    break;
                case "2":
                    dt = this._getQuery2();
                    break;
                case "3":
                    dt = this._getQuery3(Request.Form["country"]);
                    break;
                case "4":
                    dt = this._getQuery4(Request.Form["year"], Request.Form["color_pm25"]);
                    break;
                default:
                    return null;
            }

            XLWorkbook wb = new XLWorkbook();
            wb.Worksheets.Add(dt);

            using (MemoryStream stream = new MemoryStream())
            {
                wb.SaveAs(stream);
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Grid.xlsx");
            }
        }
    }
}
