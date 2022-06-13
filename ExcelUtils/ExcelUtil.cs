using IronXL;

namespace ExcelUtils
{
    public class ExcelUtil
    {
        //полное имя выбранного файла
        private string _pathFile;

        //имя директории, в которой содержится таблица
        private string _pathDirectory;

        //лист таблицы, в котором содержатся данные
        private WorkSheet _sheet;

        //Excel-книга, представляющая весь файл
        private WorkBook _book;

        //количество строк в таблице
        private int _rowCount;

        public ExcelUtil()
        {
            WorkBook xlsxWorkBook = WorkBook.Create(ExcelFileFormat.XLSX);
            xlsxWorkBook.CreateWorkSheet("Sheet 1");

            _pathFile = $"{AppDomain.CurrentDomain.BaseDirectory}Tables\\NewExcelFile.xlsx";

            if(File.Exists(_pathFile))
                File.Create(_pathFile);
            xlsxWorkBook.SaveAs(_pathFile);

            _book = xlsxWorkBook;
            _pathDirectory = Path.GetDirectoryName(_pathFile);
            _sheet = xlsxWorkBook.WorkSheets.First();
            _rowCount = RowCount();
        }

        public ExcelUtil(string pathFile)
        {
            _pathFile = pathFile;
            _pathDirectory = Path.GetDirectoryName(_pathFile);
            _book = WorkBook.Load(_pathFile);
            _sheet = _book.WorkSheets.First();
            _rowCount = RowCount();
        }

        public void ChangeWorkBook(string pathFile)
        {
            _pathFile = pathFile;
            _pathDirectory = Path.GetDirectoryName(_pathFile);
            _book = WorkBook.Load(_pathFile);
            _sheet = _book.WorkSheets.First();
            _rowCount = RowCount();
        }

        public void SetPointsData(ICollection<TemperaturePointModel> pointsList)
        {
            _sheet[$"A2:D{_rowCount}"].ClearContents();
            int index = 2;
            var t = pointsList.OrderBy(x => x.Temperature).Reverse();
            foreach(var point in t)
            {
                _sheet[$"A{index}"].StringValue = point.Name;
                _sheet[$"B{index}"].DoubleValue = point.Longitude;
                _sheet[$"C{index}"].DoubleValue = point.Latitude;
                if(point.Temperature != null)
                    _sheet[$"D{index}"].DoubleValue = (double)point.Temperature;
                index++;
            }
            _rowCount = index;
            _book.SaveAs(_pathFile);

        }

        public ICollection<TemperaturePointModel> GetPointsData()
        {
            List<TemperaturePointModel> pointsList = new();
            double? temp;
            for (int i = 2; i <= _rowCount; i++)
            {
                if (_sheet[$"D{i}"].IsEmpty)
                    temp = null;
                else
                    temp = _sheet[$"D{i}"].DoubleValue;
                pointsList.Add(new TemperaturePointModel()
                {
                    Name = _sheet[$"A{i}"].StringValue,
                    Longitude = _sheet[$"B{i}"].DoubleValue,
                    Latitude = _sheet[$"C{i}"].DoubleValue,
                    Temperature = temp,

                });
            }
            return pointsList;
        }

        public (double[] dates, double[] values) GetPointDataForMonth(string pointName)
        {
            var fileName = $"{_pathDirectory}\\{pointName}.xlsx";
            if (!File.Exists(fileName)) throw new Exception($"Файла с полным именем {fileName} не существует");
            var book = WorkBook.Load(fileName);
            var sheet = book.WorkSheets.First();
            var data = sheet.ToMultiDimensionalArray();
            int rowCount = data.GetLength(0);

            double[] dates = new double[rowCount];
            double[] values = new double[rowCount];

            for (int i = 0; i < rowCount; i++)
            {
                dates[i] = data[i][0].DateTimeValue.Value.ToOADate();
                values[i] = data[i][1].DoubleValue;
            }

            return (dates, values);
        }

        public double GetMaxTemp()
        {
            if (_rowCount < 2) return 100;
            return (double)_sheet[$"D2:D{_rowCount}"].Max();
            
        }

        public double GetMinTemp()
        {
            _sheet[""].
            if (_rowCount < 2) return -100;
            return (double)_sheet[$"D2:D{_rowCount}"].Min();            
        }

        private int RowCount()
        {
            int index = 2;
            while (!(_sheet[$"B{index}"].IsEmpty ||
                     _sheet[$"C{index}"].IsEmpty ))
                index++;

            return index-1;
        }

    }
}