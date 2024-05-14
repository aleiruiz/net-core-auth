using ClosedXML.Excel;

namespace XLocker.Services
{
    public interface IExcelService
    {
        MemoryStream CreateExcel<T>(List<T> list, string fileName, string[] columnNames);
    }
    public class ExcelService : IExcelService
    {

        public MemoryStream CreateExcel<T>(List<T> list, string fileName, string[] columnNames)
        {
            var workbook = new XLWorkbook();
            var ws = workbook.AddWorksheet(fileName);
            ws.Cell(1, 1).InsertTable(list);

            for (int i = 0; i < columnNames.Length; i++)
            {
                ws.Cell(1, i + 1).Value = columnNames[i];
                ws.Column(i + 1).AdjustToContents();
            }

            var ms = new MemoryStream();

            workbook.SaveAs(ms);

            return ms;
        }
    }
}
