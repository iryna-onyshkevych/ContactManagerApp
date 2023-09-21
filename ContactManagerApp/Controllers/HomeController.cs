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

            return View(_context.Files.ToList());
        }

        public IActionResult Privacy()
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

            return RedirectToAction("Index");
        }
        static void InsertDataIntoSQLServerUsingSQLBulkCopy(DataTable csvFileData)
        {


            using (SqlConnection dbConnection = new SqlConnection("Data Source = (localdb)\\Local; Initial Catalog = expeditiondb; Integrated Security = SSPI; "))
            {
                using (SqlBulkCopy s = new SqlBulkCopy(dbConnection))
                {
                    //Set the database table name.
                    s.DestinationTableName = "dbo.UsersData";
                    dbConnection.Open();
                    s.WriteToServer(csvFileData);
                    dbConnection.Close();

                }
            }
        }

        private static DataTable GetDataTabletFromCSVFile(string csv_file_path)
        {
            DataTable csvData = new DataTable();

            csvData.Columns.AddRange(new DataColumn[5] { new DataColumn("Name", typeof(string)),
            new DataColumn("Date of Birth", typeof(DateTime)),
            new DataColumn("Married",typeof(bool)),
            new DataColumn("Phone", typeof(string)),
            new DataColumn("Salary", typeof(decimal))
});


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


            //System.IO.File.ReadAllText(csv_file_path);
            //try

            //{
            //    using (TextFieldParser csvReader = new TextFieldParser(csv_file_path))
            //    {
            //        csvReader.SetDelimiters(new string[] { "," });
            //        csvReader.HasFieldsEnclosedInQuotes = true;
            //        string[] colFields = csvReader.ReadFields();



            //            foreach (string column in colFields)
            //        {
            //            DataColumn datecolumn = new DataColumn(column);
            //            datecolumn.AllowDBNull = true;
            //            csvData.Columns.Add(datecolumn);
            //        }
            //        while (!csvReader.EndOfData)
            //        {
            //            string[] fieldData = csvReader.ReadFields();
            //            //Making empty value as null
            //            for (int i = 0; i < fieldData.Length; i++)
            //            {
            //                if (fieldData[i] == "")
            //                {
            //                    fieldData[i] = null;
            //                }
            //            }
            //            csvData.Rows.Add(fieldData);
            //        }
            //    }

            //catch (Exception ex)
            //{
            //    return null;
            //}
            return csvData;
        }
    }
}
