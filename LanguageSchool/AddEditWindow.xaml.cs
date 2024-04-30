using Microsoft.Win32;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LanguageSchool
{
    /// <summary>
    /// Логика взаимодействия для AddEditWindow.xaml
    /// </summary>
    public partial class AddEditWindow : Window
    {
        private Client _currentClient = new Client();
        bool CheckClient = false;
        public AddEditWindow(Client selectedClient)
        {
            InitializeComponent();
            GenderCombo.SelectedIndex = 0;
            if(selectedClient != null)
            {
                BirthdayTB.SelectedDate = Convert.ToDateTime(selectedClient.Birthday);
                _currentClient = selectedClient;
                if(_currentClient.GenderCode == "м")
                {
                    GenderCombo.SelectedIndex = 0;
                }
                else
                {
                    GenderCombo.SelectedIndex = 1;

                }
                CheckClient = true;
            }
            else
            {
                IdText.Visibility = Visibility.Hidden;
                IdTB.Visibility = Visibility.Hidden;
            }
            DataContext = _currentClient;

     
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_currentClient.ID == 0)
            {
                ShafikovLanguageEntities.GetContext().Client.Add(_currentClient);
            }
            StringBuilder errors = new StringBuilder();

            /*if (string.IsNullOrWhiteSpace(Convert.ToString(_currentClient.ID)))
                errors.AppendLine("Укажите ID клиента");*/

            if (string.IsNullOrWhiteSpace(_currentClient.LastName))
                errors.AppendLine("Укажите Фамилию");

            if (string.IsNullOrWhiteSpace(_currentClient.FirstName))
                errors.AppendLine("Укажите имя");

            if (string.IsNullOrWhiteSpace(_currentClient.Patronymic))
                errors.AppendLine("Укажите отчество");

            if (string.IsNullOrWhiteSpace(_currentClient.Email))
            {
                errors.AppendLine("Укажите email");
            }
            else
            {
                string emailPattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9]+$";
                if (!Regex.IsMatch(_currentClient.Email, emailPattern))
                    errors.AppendLine("Не корректный Email!");
            }
            if (string.IsNullOrWhiteSpace(_currentClient.Phone))
                errors.AppendLine("Укажите номер телефона");
            else
            {
                /*string ph = _currentClient.Phone.Replace("(", "").Replace("-", "").
                    Replace("+", "").Replace(")", "").Replace(" ", "");
                if (((ph[1] == '9' || ph[1] == '4' || ph[1] == '8') && ph.Length != 11)
                    || (ph[1] == '3' && ph.Length != 12))
                    errors.AppendLine("Укажите правильно номер телефона");*/
                if (_currentClient.Phone.Count(c => c == '+') == 1)
                {
                    if (!(_currentClient.Phone[0] == '+'))
                        errors.AppendLine("Некорректный номер телефона!");
                }
                if (_currentClient.Phone.Count(c => c == '+') > 1 || _currentClient.Phone.Count(c => c == '(') > 1 
                    || _currentClient.Phone.Count(c => c == ')') > 1  || _currentClient.Phone.Count(c => c == '-') > 2)
                {
                    errors.AppendLine("Не корректный номер телефона! ");
                }
            }

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            /*_currentClient
            currentProductSale.SaleDate = Convert.ToDateTime(DateSale.Text);
            currentProductSale.ProductCount = Convert.ToInt32(TBoxCountSale.Text);
            currentProductSale.ProductID = ComboProduct.SelectedIndex + 1;
            currentProductSale.AgentID = currentAgent.ID;*/

            _currentClient.Birthday = Convert.ToDateTime(BirthdayTB.Text);
            if (GenderCombo.SelectedIndex == 0)
            {
                _currentClient.GenderCode = "м";
            }
            else
            {
                _currentClient.GenderCode = "ж";
            }



            var allClient = ShafikovLanguageEntities.GetContext().Client.ToList();
            allClient = allClient.Where(p => p.ID == _currentClient.ID).ToList();

            if (allClient.Count == 0 || (CheckClient == true/* && allClient.Count <= 1)*/))
            {
                if (CheckClient == false)
                {
                    _currentClient.RegistrationDate = DateTime.Now;
                }
                if (_currentClient.ID == 0)
                { 
                    //_currentClient.Birthday = Convert.ToDateTime(BirthdayTB.Text);
                    ShafikovLanguageEntities.GetContext().Client.Add(_currentClient);
                }
                try
                {
                    ShafikovLanguageEntities.GetContext().SaveChanges();
                    MessageBox.Show("Информация сохранена");
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
            else
            {
                MessageBox.Show("Уже существует такой клиент");
            }
        }

        private void ChangePictureBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog myOpenFileDialog = new OpenFileDialog();
            if (myOpenFileDialog.ShowDialog() == true)
            {
                _currentClient.PhotoPath = myOpenFileDialog.FileName;
                PhotoImage.Source = new BitmapImage(new Uri(myOpenFileDialog.FileName));
            }
        }

        private void GenderCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void PhoneTB_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void EmailTB_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void LastNameTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string filteredText = new string(textBox.Text.Where(c => char.IsLetter(c) || c == ' ' || c == '-').ToArray());

            if (filteredText != textBox.Text)
            {
                textBox.Text = filteredText;
                textBox.SelectionStart = filteredText.Length;
            }
        }

        private void NameTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string filteredText = new string(textBox.Text.Where(c => char.IsLetter(c) || c == ' ' || c == '-').ToArray());

            if (filteredText != textBox.Text)
            {
                textBox.Text = filteredText;
                textBox.SelectionStart = filteredText.Length;
            }
        }

        private void PatronymicTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string filteredText = new string(textBox.Text.Where(c => char.IsLetter(c) || c == ' ' || c == '-').ToArray());

            if (filteredText != textBox.Text)
            {
                textBox.Text = filteredText;
                textBox.SelectionStart = filteredText.Length;
            }
        }
    }
}
