using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using WPF_Lab4.Entities;

namespace WPF_Lab4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<Car> observableCars;

        public MainWindow()
        {
            InitializeComponent();
            InitializeCarData();
        }

        private void InitializeCarData()
        {
            List<Car> myCars = new List<Car>()
            {
                new Car("E250", new Engine(1.8, 204, "CGI"), 2009),
                new Car("E350", new Engine(3.5, 292, "CGI"), 2009),
                new Car("A6", new Engine(2.5, 187, "FSI"), 2012),
                new Car("A6", new Engine(2.8, 220, "FSI"), 2012),
                new Car("A6", new Engine(3.0, 295, "TFSI"), 2012),
                new Car("A6", new Engine(2.0, 175, "TDI"), 2011),
                new Car("A6", new Engine(3.0, 309, "TDI"), 2011),
                new Car("S6", new Engine(4.0, 414, "TFSI"), 2012),
                new Car("S8", new Engine(4.0, 513, "TFSI"), 2012)
            };
            observableCars = new ObservableCollection<Car>(myCars);
            CarDataGrid.ItemsSource = observableCars;
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchTextBox.Text.ToLower();

            var filteredCars = observableCars.Where(car =>
                car.Model.ToLower().Contains(searchText) ||
                car.Motor.Model.ToLower().Contains(searchText) ||
                car.Motor.HorsePower.ToString().Contains(searchText) ||
                car.Motor.Displacement.ToString().Contains(searchText) ||
                car.Year.ToString().Contains(searchText)
            ).ToList();

            CarDataGrid.ItemsSource = new ObservableCollection<Car>(filteredCars);
        }

        private void LinqQuery_Click(object sender, RoutedEventArgs e)
        {
            var cars = observableCars
                .Where(x => x.Model == "A6")
                .GroupBy(g => g.Motor.Model == "TDI" ? "diesel" : "petrol")
                .Select(engineGroup => new
                {
                    engineType = engineGroup.Key,
                    avgHPPL = engineGroup.Average(c => c.Motor.HorsePower / c.Motor.Displacement)
                })
                .OrderByDescending(r => r.avgHPPL);

            OutputTextBox.Clear();
            foreach (var c in cars)
            {
                OutputTextBox.AppendText($"{c.engineType}: {c.avgHPPL}\n");
            }
        }

        private void QueryExpression_Click(object sender, RoutedEventArgs e)
        {
            var cars = from car in observableCars
                       where car.Model == "A6"
                       group car by car.Motor.Model == "TDI" ? "diesel" : "petrol" into engineGroup
                       select new
                       {
                           engineType = engineGroup.Key,
                           avgHPPL = engineGroup.Average(c => c.Motor.HorsePower / c.Motor.Displacement)
                       } into result
                       orderby result.avgHPPL descending
                       select result;

            OutputTextBox.Clear();
            foreach (var c in cars)
            {
                OutputTextBox.AppendText($"{c.engineType}: {c.avgHPPL}\n");
            }
        }

        //https://learn.microsoft.com/pl-pl/dotnet/desktop/wpf/windows/how-to-open-message-box?view=netdesktop-8.0
        private void DelegateQuery_Click(object sender, RoutedEventArgs e)
        {
            Func<Car, Car, int> sortArg = delegate (Car a, Car b)
            {
                return b.Motor.HorsePower.CompareTo(a.Motor.HorsePower);
            };

            Predicate<Car> filterArg = delegate (Car c)
            {
                return c.Motor.Model.Equals("TDI");
            };

            Action<Car> actionArg = delegate (Car c)
            {
                MessageBox.Show($"{c.Motor.HorsePower} - {c.Motor.Model}");
            };

            List<Car> cars = observableCars.ToList();

            cars.Sort(new Comparison<Car>(sortArg));
            cars.FindAll(filterArg).ForEach(actionArg);
        }

        public void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                Car selectedCar = (Car)e.AddedItems[0];
                observableCars.Append(selectedCar);
            }
            if (e.RemovedItems.Count > 0)
            {
                Car selectedCar = (Car)e.RemovedItems[0];
                observableCars.Remove(selectedCar);
            }
        }   
    }
}
