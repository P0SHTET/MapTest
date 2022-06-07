using IronXL;

namespace ExcelUtils
{
    public class ExcelUtil
    {
        private string _pathFile;
        private WorkSheet _sheet;
        private WorkBook _book;
        private int _rowCount;

        public ExcelUtil()
        {
            WorkBook xlsxWorkBook = WorkBook.Create(ExcelFileFormat.XLSX);
            xlsxWorkBook.CreateWorkSheet("Sheet 1");
            xlsxWorkBook.SaveAs("NewExcelFile.xlsx");

            _book = xlsxWorkBook;
            _pathFile = xlsxWorkBook.FilePath;
            _sheet = xlsxWorkBook.WorkSheets.First();
            _rowCount = RowCount();
        }

        public ExcelUtil(string pathFile)
        {
            _pathFile = pathFile;
            _book = WorkBook.Load(_pathFile);
            _sheet = _book.WorkSheets.First();
            _rowCount = RowCount();
        }

        public void ChangeWorkBook(string pathFile)
        {
            _pathFile = pathFile;
            _book = WorkBook.Load(_pathFile);
            _sheet = _book.WorkSheets.First();
            _rowCount = RowCount();
        }

        public void SetPointsData(IEnumerable<TemperaturePointModel> pointsList)
        {
            int index = 2;
            var t = pointsList.OrderBy(x => x.Temperature);
            foreach(var point in t)
            {
                _sheet[$"A{index}"].StringValue = point.Name;
                _sheet[$"B{index}"].DoubleValue = point.Longitude;
                _sheet[$"C{index}"].DoubleValue = point.Latitude;
                _sheet[$"D{index}"].DoubleValue = point.Temperature;
                index++;
            }
            _book.SaveAs(_pathFile);
        }

        public ICollection<TemperaturePointModel> GetPointsData()
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