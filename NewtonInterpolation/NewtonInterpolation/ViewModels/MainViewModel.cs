using Mvvm.Core;
using Mvvm.Core.Services;
using NewtonInterpolation.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace NewtonInterpolation.ViewModels
{
    class MainViewModel: ViewModelBase
    {
        private ObservableCollection<Point> _Table;
        public ObservableCollection<Point> Table
        {
            get { return _Table; }
        }

        private ObservableCollection<Point> _GrapthPoints;
        public ObservableCollection<Point> GrapthPoints
        {
            get { return _GrapthPoints; }
        }

        private string _Polinom;
        public String Polinom
        {
            get { return "P = " + _Polinom; }
            private set { _Polinom = value; }
        }



        //private double _InputX;
        //public String InputX {
        //    get { return _InputX.ToString(); }
        //    set { double.TryParse(value, out _InputX); }
        public String InputX { get; set; }
        public String InputY { get; set; }

        public ICommand AddPointCommand { get; private set; }
        public ICommand InterpolationCommand { get; private set; }

        //private FuncParser _Praser;

        public MainViewModel()
        {
            var commandFactory = new RelayCommandFactory();
            //_Praser = new FuncParser();

            AddPointCommand = commandFactory.CreateCommand(AddPointFunc);
            InterpolationCommand = commandFactory.CreateComamnd(o=>InterpolationFunc());
            

            _Table = new ObservableCollection<Point>();
            _Table.Add(new Point(-1.0, -0.5));
            _Table.Add(new Point(0.0, 0.0));
            _Table.Add(new Point(1.0, 0.5));
            _Table.Add(new Point(2.0, 0.86603));
            _Table.Add(new Point(3.0, 0.5));
            _Table.Add(new Point(4.0, 0.86603));

        }

        private void AddPointFunc()
        {
            double x = 0, y = 0;
            InputX = InputX.Replace('.', ',');
            InputY = InputY.Replace('.', ',');
            if (!Double.TryParse(InputX, out x) || !Double.TryParse(InputY, out y))
            {
                MessageBox.Show("Некорректная точка (" + InputX + ", " + InputY + ")", "Ошибка",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                return;
            }
            if (_Table.Any(point => point.X == x)){
                MessageBox.Show("Точка уже существует точка X =" + InputX, "Ошибка",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                return;
            }
            _Table.Add(new Point(x, y));
        }

        private void InterpolationFunc()
        {

            double[] x = new double[Table.Count];
            double[] y = new double[Table.Count];

            for (int i = 0; i < Table.Count; i++)
            {
                x[i] = Table[i].X;
                y[i] = Table[i].Y;
            }

            Dictionary<int, double> coef = Tools.Tools.Newton(Table.Count, x, y);

            StringBuilder str = new StringBuilder();
            foreach (var item in coef)
            {
                if (item.Value != 0)
                {
                    if (item.Value > 0)
                        str.AppendFormat("+{0:f4}*X^{1}", item.Value, item.Key);
                    else
                        str.AppendFormat("+(0{0:f4})*X^{1}", item.Value, item.Key); // (0-2)*X^2
                }
            }
            str.Remove(0, 1);
            _Polinom = str.ToString();
            RaisePropertyChanged("Polinom");
            _GrapthPoints = new ObservableCollection<Point>();

            var parser = new FuncParser(_Polinom);
            for (double i = x.Min() - 5; i < x.Max() + 5; i += 0.1)
                _GrapthPoints.Add(new Point(i, parser.Calculation(i)));
            RaisePropertyChanged("GrapthPoints");
        }
    }
}
