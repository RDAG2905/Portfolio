//using ClosedXML.Excel;
using Greco2.Excel;
using Greco2.Model;
using Greco2.Models.Denuncia;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace Greco2.Controllers
{
    public class ExcelController : Controller
    {
        // GET: Excel
        public ActionResult Index()
        {
            //NorthwindEntities entities = new NorthwindEntities();
            //return View(from customer in entities.Customers.Take(10)
            //            select customer);
            return null;
        }

        //    [HttpPost]
        //    public FileResult Export()
        //    {
        //        NorthwindEntities entities = new NorthwindEntities();
        //        DataTable dt = new DataTable("Grid");
        //        dt.Columns.AddRange(new DataColumn[4] { new DataColumn("CustomerId"),
        //                                        new DataColumn("ContactName"),
        //                                        new DataColumn("City"),
        //                                        new DataColumn("Country") });

        //        var customers = from customer in entities.Customers.Take(10)
        //                        select customer;

        //        foreach (var customer in customers)
        //        {
        //            dt.Rows.Add(customer.CustomerID, customer.ContactName, customer.City, customer.Country);
        //        }

        //        using (XLWorkbook wb = new XLWorkbook())
        //        {
        //            wb.Worksheets.Add(dt);
        //            using (System.IO.MemoryStream stream = new MemoryStream())
        //            {
        //                wb.SaveAs(stream);
        //                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Grid.xlsx");
        //            }
        //        }
        //    }

        [HttpPost]
        public JsonResult GenerateSpreadsheet()
        {
            // Create temp path and file name
            var path = Server.MapPath("~/temp");
            var fileName = "Spreadsheet.xlsx";

            // Create temp path if not exits
            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }

            // Create the sample DataSet
            DataSet dataSet = new System.Data.DataSet("Hospital");
            dataSet.Tables.Add(Table());

            // Create the Excel file in temp path
            string fullPath = Path.Combine(path, fileName);
            //CreateExcelFile cef = new CreateExcelFile();
            CreateExcelFile.CreateExcelDocument(dataSet, fullPath, includeAutoFilter: true);

            // Return the Excel file name
            return Json(new { fileName = fileName, errorMessage = "" });
        }

        [HttpGet]
        [NoCache]
        public ActionResult DownloadSpreadsheet(string file)
        {
            if (Path.IsPathRooted(file))
            {
                throw new ArgumentNullException("El Path contiene una ruta Absoluta : " + file);
            }
            else
            {
                string fullPath = Path.Combine(Server.MapPath("~/temp"), file);
                return File(fullPath, "application/vnd.ms-excel", file);
            }
            
        }

        private DataTable Table()
        {
            // Sample dataset from https://www.dotnetperls.com/dataset and I added more entries

            DataTable table = new DataTable("Prescription");
            table.Columns.Add("Dosage", typeof(int));
            table.Columns.Add("Drug", typeof(string));
            table.Columns.Add("Patient", typeof(string));
            table.Columns.Add("Date", typeof(DateTime));

            table.Rows.Add(25, "Indocin", "David", DateTime.Parse("01/01/2001"));
            table.Rows.Add(50, "Enebrel", "Sam", DateTime.Parse("01/01/2002"));
            table.Rows.Add(10, "Hydralazine", "Christoff", DateTime.Parse("01/01/2003"));
            table.Rows.Add(21, "Combivent", "Janet", DateTime.Parse("01/01/2004"));
            table.Rows.Add(100, "Dilantin", "Melanie", DateTime.Parse("01/01/2005"));

            table.Rows.Add(50, "Meloxicam", "Nobert", DateTime.Parse("02/01/2001"));
            table.Rows.Add(25, "Clonazepam", "Noah", DateTime.Parse("03/01/2002"));
            table.Rows.Add(150, "Metformin", "Liam", DateTime.Parse("04/01/2003"));
            table.Rows.Add(300, "Cyclobenzaprine", "Mason", DateTime.Parse("05/01/2004"));
            table.Rows.Add(200, "Doxycycline", "Jacob", DateTime.Parse("06/01/2005"));

            table.Rows.Add(250, "Gabapentin", "William", DateTime.Parse("01/02/2001"));
            table.Rows.Add(125, "Hydrochlorothiazide", "Linda", DateTime.Parse("01/03/2002"));
            table.Rows.Add(10, "Ibuprofen", "Ethan", DateTime.Parse("01/04/2003"));
            table.Rows.Add(15, "Lexapro", "James", DateTime.Parse("01/05/2004"));
            table.Rows.Add(50, "Lorazepam", "Alexander", DateTime.Parse("01/06/2005"));

            return table;
        }


        //[HttpPost]
        //public JsonResult GenerarListaSabanaExcel()
        //{
        //    List<ListaSabanaSP> lista = new List<ListaSabanaSP>();
        //    using (NuevoDbContext context = new NuevoDbContext())
        //    {
        //        lista = context.Database
        //                .SqlQuery<ListaSabanaSP>("ExcelGetListaSabana")
        //                .ToList();
        //    }

        //    if (lista.Any())
        //    {
        //        var listaNueva = from e in lista

        //                         select new
        //                         {
        //                             e.DenunciaId,
        //                             e.Tipo_Proceso,
        //                             e.Etapa_Procesal,
        //                             e.Usuario_Creador,
        //                             e.Expediente_Id,
        //                             e.Denunciante,
        //                             e.Denunciante_Individual,
        //                             e.Nro_Documento,
        //                             e.Nro_Linea,
        //                             e.Tramite_CRM,
        //                             e.Fecha_Creacion,
        //                             e.Fecha_Notificacion,
        //                             e.Fecha_Notificacion_Gcia,
        //                             e.Organismo,
        //                             e.Localidad,
        //                             e.Provincia,
        //                             e.Region,
        //                             e.Responsable_Interno,
        //                             e.Estudio,
        //                             e.Modalidad_Gestion,
        //                             e.Sub_Tipo_Proceso,
        //                             e.Servicio,
        //                             e.Motivo_Reclamo,
        //                             e.Estado_Actual,
        //                             e.Fecha_Resultado,
        //                             e.Mediador,
        //                             e.Matricula,
        //                             e.Domicilio_Mediador,
        //                             e.Fecha_Homologacion,
        //                             e.Nro_Gestion_Coprec,
        //                             e.Honorarios_Coprec,
        //                             e.Fecha_Gestion_Honorarios,
        //                             e.Monto_Acordado,
        //                             e.Arancel,
        //                             e.Fecha_Gestion_Arancel,
        //                             e.Agenda_Coprec,
        //                             e.Denuncia_Preventiva,
        //                             e.Denuncia_Formal

        //                         };

        //        var tabla = LinqQueryToDataTable(listaNueva);

        //        var path = Server.MapPath("~/temp");

        //        var fileName = "ReporteDenuncias"
        //                       + DateTime.Now.Year.ToString()
        //                       + DateTime.Now.Month.ToString()
        //                       + DateTime.Now.Day.ToString()
        //                       + ".xlsx";

        //        if (Directory.Exists(path) == false)
        //        {
        //            Directory.CreateDirectory(path);
        //        }
        //        DataSet dataSet = new System.Data.DataSet("Denuncias");

        //        dataSet.Tables.Add(tabla);

        //        string fullPath = Path.Combine(path, fileName);

        //        CreateExcelFile.CreateExcelDocument(dataSet, fullPath, includeAutoFilter: true);

        //        return Json(new { fileName = fileName, errorMessage = "" });
        //    }
        //    else
        //    {
        //        return Json(new { fileName = "isEmpty", errorMessage = "" });
        //    }
        //}

        //public static DataTable LinqQueryToDataTable(IEnumerable<dynamic> v)
        //{
           
        //    var firstRecord = v.FirstOrDefault();
        //    if (firstRecord == null)
        //        return null;

            
        //    PropertyInfo[] infos = firstRecord.GetType().GetProperties();

           
        //    DataTable table = new DataTable();

            
        //    foreach (var info in infos)
        //    {

        //        Type propType = info.PropertyType;

        //        if (propType.IsGenericType
        //            && propType.GetGenericTypeDefinition() == typeof(Nullable<>)) //Nullable types should be handled too
        //        {
        //            table.Columns.Add(info.Name, Nullable.GetUnderlyingType(propType));
        //        }
        //        else
        //        {
        //            table.Columns.Add(info.Name, info.PropertyType);
        //        }
        //    }

            
        //    DataRow row;

        //    foreach (var record in v)
        //    {
        //        row = table.NewRow();
        //        for (int i = 0; i < table.Columns.Count; i++)
        //        {
        //            row[i] = infos[i].GetValue(record) != null ? infos[i].GetValue(record) : DBNull.Value;
        //        }

        //        table.Rows.Add(row);
        //    }

        //    //Table is ready to serve.
        //    table.AcceptChanges();

        //    return table;
        //}


        public class NoCacheAttribute : ActionFilterAttribute
        {
            public override void OnActionExecuted(ActionExecutedContext context)
            {
                context.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            }
        }


    }
}
