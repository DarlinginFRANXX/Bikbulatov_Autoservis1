using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Bikbulatov_Autoservis
{
    /// <summary>
    /// Логика взаимодействия для SignUpPage.xaml
    /// </summary
    

    public partial class SignUpPage : Page
    {
        private Service _currentService = new Service();
        public SignUpPage(Service SelectedService)
        {
            InitializeComponent();
            if (SelectedService != null)
                this._currentService = SelectedService;

            DataContext = _currentService;

            var _currentClient = Бикбулатов_АвтосервисEntities.GetContext().Client.ToList();
            ComboClient.ItemsSource = _currentClient;
        }

        private ClientService _currentClientService = new ClientService();
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (ComboClient.SelectedItem == null)
                errors.AppendLine("Укажите ФИО клиента");

            if (StartDate.Text == "")
                errors.AppendLine("Укажите дату услуги");

            if (TBStart.Text == "")
                errors.AppendLine("Укажите время начала услуги");




            if (!Regex.IsMatch(TBStart.Text, @"^\d{1,2}:\d{2}$"))
            {
                errors.AppendLine("Время начала указано неверно. Используйте формат ЧЧ:ММ");
            }
            else
            {
                string[] parts = TBStart.Text.Split(':');
                bool isHourValid = int.TryParse(parts[0], out int hour);
                bool isMinValid = int.TryParse(parts[1], out int min);

                if (!isHourValid || !isMinValid || hour < 0 || hour > 23 || min < 0 || min > 59)
                {
                    errors.AppendLine("Время начала указано неверно. Часы от 0 до 23, минуты от 0 до 59.");
                }
            }
            if (!Regex.IsMatch(TBEnd.Text, @"^\d{1,2}:\d{2}$"))
            {
                errors.AppendLine("Время конца указано неверно. Используйте формат ЧЧ:ММ");
            }
            else
            {
                string[] parts = TBEnd.Text.Split(':');
                bool isHourValid = int.TryParse(parts[0], out int hour);
                bool isMinValid = int.TryParse(parts[1], out int min);

                if (!isHourValid || !isMinValid || hour < 0 || hour > 23 || min < 0 || min > 59)
                {
                    errors.AppendLine("Время конца указано неверно. Часы от 0 до 23, минуты от 0 до 59.");
                }
            }



            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            // Добавить текущие значения новой записи
            _currentClientService.ClientID = ComboClient.SelectedIndex + 1; // т.к. нумерация с 0
            _currentClientService.ServiceID = _currentService.ID;
            _currentClientService.StartTime = Convert.ToDateTime(StartDate.Text + " " + TBStart.Text);

            if (_currentClientService.ID == 0)
                Бикбулатов_АвтосервисEntities.GetContext().ClientService.Add(_currentClientService);

            // Сохранить изменения, если никаких ошибок не получилось при этом
            try
            {
                Бикбулатов_АвтосервисEntities.GetContext().SaveChanges();
                MessageBox.Show("Информация сохранена");
                manager.MainFrame.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void TBStart_TextChanged(object sender, TextChangedEventArgs e)
        {
            string s = TBStart.Text;


            if (s.Length < 3 || !s.Contains(':'))
            {
                TBEnd.Text = "";
            }
            else
            {
                string[] start = s.Split(':');

                if (start.Length != 2 || string.IsNullOrEmpty(start[1]))
                {
                    TBEnd.Text = "";
                    return;
                }

                bool isHourValid = int.TryParse(start[0], out int startHour);
                bool isMinValid = int.TryParse(start[1], out int startMin);

                if (!isHourValid || !isMinValid || startHour < 0 || startHour > 23 || startMin < 0 || startMin > 59)
                {
                    TBEnd.Text = "";
                    return;
                }

                int totalMinutes = startHour * 60 + startMin + _currentService.Duration;
                int EndHour = (totalMinutes / 60) % 24;
                int EndMin = totalMinutes % 60;

                TBEnd.Text = EndHour.ToString() + ":" + EndMin.ToString("D2");
            }

        }

        private void TBStart_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Разрешаем только цифры и двоеточие
            if (!char.IsDigit(e.Text, 0) && e.Text != ":")
            {
                e.Handled = true; // запрещаем ввод
            }
        }

        private void TBEnd_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, 0) && e.Text != ":")
            {
                e.Handled = true; // запрещаем ввод
            }
        }
    }
}
