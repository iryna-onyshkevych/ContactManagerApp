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

                string filePath = _appEnvironment.WebRootPath + path;
                var data = GetDataTabletFromCSVFile(filePath);
                InsertDataIntoSQLServerUsingSQLBulkCopy(data);
            }

            return RedirectToAction("Index", "User");
        }

        static void InsertDataIntoSQLServerUsingSQLBulkCopy(DataTable fileData)
        {

            using (SqlConnection dbConnection = new SqlConnection("Data Source = ENTER; Initial Catalog = ENTER; Integrated Security = SSPI;"))
            {
                using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(dbConnection))
                {
                    sqlBulkCopy.DestinationTableName = "dbo.Users";
                    dbConnection.Open();
                    sqlBulkCopy.WriteToServer(fileData);
                    dbConnection.Close();
                }
            }
        }

        private static DataTable GetDataTabletFromCSVFile(string filePath)
        {
            DataTable data = new DataTable();

            data.Columns.AddRange(new DataColumn[6] {new DataColumn("Id", typeof(int)),
            new DataColumn("UserName", typeof(string)),
            new DataColumn("DateOfBirth", typeof(DateTime)),
            new DataColumn("Married",typeof(bool)),
            new DataColumn("Phone", typeof(string)),
            new DataColumn("Salary", typeof(decimal))
            });

            try
            {
                string text = System.IO.File.ReadAllText(filePath);

                foreach (string row in text.Split('\n'))
                {
                    if (!string.IsNullOrEmpty(row))
                    {
                        data.Rows.Add();
                        int i = 0;
                        foreach (string cell in row.Split(','))
                        {
                            data.Rows[data.Rows.Count - 1][i] = cell;
                            i++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            };
            return data;
        }
    }
}
