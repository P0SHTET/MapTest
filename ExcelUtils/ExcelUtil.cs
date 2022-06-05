using IronXL;

namespace ExcelUtils
{
    public class ExcelUtil
    {
        private string _pathFile;
        private WorkSheet _sheet;
        private int _rowCount;

        public ExcelUtil()
        {
            WorkBook xlsxWorkBook = WorkBook.Create(ExcelFileFormat.XLSX);
            xlsxWorkBook.CreateWorkSheet("Sheet 1");
            xlsxWorkBook.SaveAs("NewExcelFile.xlsx");

            _pathFile = xlsxWorkBook.FilePath;
            _sheet = xlsxWorkBook.WorkSheets.First();
            _rowCount = RowCount();
        }

        public ExcelUtil(string pathFile)
        {
            _pathFile = pathFile;
            _sheet = WorkBook.Load(_pathFile).WorkSheets.First();
            _rowCount = RowCount();
        }

        public void ChangeWorkBook(string pathFile)
        {
            _pathFile = pathFile;
            _sheet = WorkBook.Load(_pathFile).WorkSheets.First();
            _rowCount = RowCount();
        }

        public IEnumerable<TemperaturePointModel> GetPointsData()
        {
            List<TemperaturePointModel> pointsList = new List<TemperaturePointModel>();

            for(int i = 2; i <= _rowCount; i++)             
                pointsList.Add(new TemperaturePointModel()
                {
                    Name =          _sheet[$"A{i}"].StringValue,
                    Longitude =     _sheet[$"B{i}"].DoubleValue,
                    Latitude =      _sheet[$"C{i}"].DoubleValue,
                    Temperature =   _sheet[$"D{i}"].DoubleValue,

                });
           

            return pointsList;
        }

        public double GetMaxTemp()
        {
            return (double)_sheet[$"D2:D{_rowCount}"].Max();
        }

        public double GetMinTemp()
        {
            return (double)_sheet[$"D2:D{_rowCount}"].Min();
        }

        private int RowCount()
        {
            int index = 2;
            while (!(_sheet[$"B{index}"].IsEmpty ||
                     _sheet[$"C{index}"].IsEmpty ||
                     _sheet[$"D{index}"].IsEmpty))
                index++;

            return index-1;
        }

    }
}