using OfficeOpenXml;
using System.Collections.Generic;

namespace NewPracticeProject.Services
{
    public interface IExcelExportService
    {
        byte[] GenerateExcel(IEnumerable<object> data, string worksheetName);
    }

    public class ExcelExportService : IExcelExportService
    {
        public byte[] GenerateExcel(IEnumerable<object> data, string worksheetName)
        {
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add(worksheetName);

            // Customize Excel worksheet based on data type
            if (data != null && data.Any())
            {
                var properties = data.First().GetType().GetProperties();

                // Customize the header row
                for (int i = 0; i < properties.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = properties[i].Name;
                }

                // Populate data rows
                int row = 2;
                foreach (var item in data)
                {
                    var values = properties.Select(prop => prop.GetValue(item)).ToArray();
                    for (int i = 0; i < values.Length; i++)
                    {
                        worksheet.Cells[row, i + 1].Value = values[i];
                    }
                    row++;
                }
            }

            return package.GetAsByteArray();
        }
    }
}
