using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace LanguageSchool
{
    /// <summary>
    /// Логика взаимодействия для ClientPage.xaml
    /// </summary>
    public partial class ClientPage : Page
    {
        int CountRecords;
        int CountPage;
        int CurrentPage = 0;
        int CountInPage = 10;

        List<Client> CurrentPageList = new List<Client>();
        List<Client> TableList;

        List<int> SelectPriority = new List<int>();
        public ClientPage()
        {
            InitializeComponent();
            var currentClient = ShafikovLanguageEntities.GetContext().Client.ToList();
            SchoolListView.ItemsSource = currentClient;
            strCount.SelectedIndex = 0;
            TBCount.Text = Convert.ToString(currentClient.Count);
            UpdatePage();
        }

        private void UpdatePage()
        {
            var currentClient = ShafikovLanguageEntities.GetContext().Client.ToList();

            TBAllRecords.Text = ShafikovLanguageEntities.GetContext().Client.ToList().Count().ToString();
            TBCount.Text = currentClient.Count().ToString();

            SchoolListView.ItemsSource = currentClient;
            TableList = currentClient;

            if (strCount.SelectedIndex == 0)
            {
                CountInPage = 10;
            }
            else if (strCount.SelectedIndex == 1)
            {
                CountInPage = 50;
            }
            else if (strCount.SelectedIndex == 2)
            {
                CountInPage = 200;
            }
            else if (strCount.SelectedIndex == 3)
            {
                CountInPage = 0;
            }

            ChangePage(0, 0);
        }
        private void ChangePage(int direction, int? selectedPage)
        {
            CurrentPageList.Clear();
            CountRecords = TableList.Count;

            if (CountInPage > 0)
            {
                TBAllRecords.Visibility = Visibility.Visible;
                TBCount.Visibility = Visibility.Visible;
                PageListBox.Visibility = Visibility.Visible;
                RightDirButton.Visibility = Visibility.Visible;
                LeftDirButton.Visibility = Visibility.Visible;

                if (CountRecords % CountInPage > 0)
                {
                    CountPage = CountRecords / CountInPage + 1;
                }
                else
                {
                    CountPage = CountRecords / CountInPage;
                }

                Boolean Ifupdate = true;

                int min;

                if (selectedPage.HasValue)
                {
                    if (selectedPage >= 0 && selectedPage <= CountPage)
                    {
                        CurrentPage = (int)selectedPage;
                        min = CurrentPage * CountInPage + CountInPage < CountRecords ? CurrentPage * CountInPage + CountInPage : CountRecords;
                        for (int i = CurrentPage * CountInPage; i < min; i++)
                        {
                            CurrentPageList.Add(TableList[i]);
                        }
                    }

                }
                else
                {
                    switch (direction)
                    {
                        case 1:
                            if (CurrentPage > 0)
                            {
                                CurrentPage--;
                                min = CurrentPage * CountInPage + CountInPage < CountRecords ? CurrentPage * CountInPage + CountInPage : CountRecords;
                                for (int i = CurrentPage * CountInPage; i < min; i++)
                                {
                                    CurrentPageList.Add(TableList[i]);
                                }
                            }
                            else
                            {
                                Ifupdate = false;
                            }
                            break;

                        case 2:
                            if (CurrentPage < CountPage - 1)
                            {
                                CurrentPage++;
                                min = CurrentPage * CountInPage + CountInPage < CountRecords ? CurrentPage * CountInPage + CountInPage : CountRecords;
                                for (int i = CurrentPage * CountInPage; i < min; i++)
                                {
                                    CurrentPageList.Add(TableList[i]);
                                }
                            }
                            else
                            {
                                Ifupdate = false;
                            }
                            break;
                    }
                }
                if (Ifupdate)
                {
                    PageListBox.Items.Clear();

                    for (int i = 1; i <= CountPage; i++)
                    {
                        PageListBox.Items.Add(i);
                    }
                    PageListBox.SelectedIndex = CurrentPage;

                    min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage * 10 + 10 : CountRecords;
                    //TBCount.Text = min.ToString();
                    TBAllRecords.Text = " из " + CountRecords.ToString();

                    SchoolListView.ItemsSource = CurrentPageList;
                    SchoolListView.Items.Refresh();
                }
            }
            else
            {
                TBAllRecords.Visibility = Visibility.Hidden;
                TBCount.Visibility = Visibility.Hidden;
                PageListBox.Visibility = Visibility.Hidden;
                RightDirButton.Visibility = Visibility.Hidden;
                LeftDirButton.Visibility = Visibility.Hidden;
            }
        }

        private void PageListBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ChangePage(0, Convert.ToInt32(PageListBox.SelectedItem.ToString()) - 1);
        }

        private void RightDirButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePage(2, null);
        }

        private void LeftDirButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePage(1, null);
        }

        private void strCount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdatePage();
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            var currentClient = (sender as Button).DataContext as Client;
            //var currentClientService = ShafikovLanguageEntities.GetContext().ClientService.ToList();
            //currentClientService = ShafikovLanguageEntities.Where(p => p.ServiceID == currentService.ID).ToList();
            if (currentClient.LastVisitDate == "Нет")
            {
                if (MessageBox.Show("Вы точно хотите выполнить удаление?", "Внимание!",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        ShafikovLanguageEntities.GetContext().Client.Remove(currentClient);
                        ShafikovLanguageEntities.GetContext().SaveChanges();
                        //выводим в листвью измененную табл сервис
                        SchoolListView.ItemsSource = ShafikovLanguageEntities.GetContext().Client.ToList();
                        //Чтобы применить фильтры и поиск, если они были на форме изначально
                        UpdatePage();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString());
                    }
                }
            }
            else
            {
                MessageBox.Show("Невозможно выполнить удаление, так как клиент проявляет активность");
            }
        }

        /*private void DeleteMenu_Click(object sender, RoutedEventArgs e)
        {
            var currentClient = (sender as Button).DataContext as Client;
            //var currentClientService = ShafikovLanguageEntities.GetContext().ClientService.ToList();
            //currentClientService = ShafikovLanguageEntities.Where(p => p.ServiceID == currentService.ID).ToList();
            if(currentClient.LastVisitDate == "Нет")
            {
                if (MessageBox.Show("Вы точно хотите выполнить удаление?", "Внимание!",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        ShafikovLanguageEntities.GetContext().Client.Remove(currentClient);
                        ShafikovLanguageEntities.GetContext().SaveChanges();
                        //выводим в листвью измененную табл сервис
                        SchoolListView.ItemsSource = ShafikovLanguageEntities.GetContext().Client.ToList();
                        //Чтобы применить фильтры и поиск, если они были на форме изначально
                        UpdatePage();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString());
                    }
                }
            }
            else
            {
                MessageBox.Show("Невозможно выполнить удаление, так как клиент проявляет активность");
            }
        }*/

    }
}
