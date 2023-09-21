using ContactManagerApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace ContactManagerApp.Controllers
{
    public class HomeController : Controller
    {
        ApplicationContext _context;
        private IWebHostEnvironment _appEnvironment;

        public HomeController(ApplicationContext context, IWebHostEnvironment appEnvironment)
        {
            _context = context;
            _appEnvironment = appEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<IActionResult> AddFile(IFormFile uploadedFile)
        {
            if (uploadedFile != null)
            {
                string path = "/Files/" + uploadedFile.FileName;

                using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }

                string file2 = _appEnvironment.WebRootPath + path;
                var data = GetDataTabletFromCSVFile(file2);
                InsertDataIntoSQLServerUsingSQLBulkCopy(data);
            }
            return RedirectToAction("Index", "User");
        }

        static void InsertDataIntoSQLServerUsingSQLBulkCopy(DataTable csvFileData)
        {
            using (SqlConnection dbConnection = new SqlConnection("Data Source = (localdb)\\Local; Initial Catalog = expeditiondb; Integrated Security = SSPI; "))
            {
                using (SqlBulkCopy s = new SqlBulkCopy(dbConnection))
                {
                    s.DestinationTableName = "dbo.Users";
                    dbConnection.Open();
                    s.WriteToServer(csvFileData);
                    dbConnection.Close();
                }
            }
        }

        private static DataTable GetDataTabletFromCSVFile(string csv_file_path)
        {
            DataTable csvData = new DataTable();

            csvData.Columns.AddRange(new DataColumn[6] {new DataColumn("Id", typeof(int)),
            new DataColumn("UserName", typeof(string)),
            new DataColumn("DateOfBirth", typeof(DateTime)),
            new DataColumn("Married",typeof(bool)),
            new DataColumn("Phone", typeof(string)),
            new DataColumn("Salary", typeof(decimal))
            });

            try
            {
                string da = System.IO.File.ReadAllText(csv_file_path);

                foreach (string row in da.Split('\n'))
                {
                    if (!string.IsNullOrEmpty(row))
                    {
                        csvData.Rows.Add();
                        int i = 0;
                        foreach (string cell in row.Split(','))
                        {
                            csvData.Rows[csvData.Rows.Count - 1][i] = cell;
                            i++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            };
            return csvData;
        }
    }
}
