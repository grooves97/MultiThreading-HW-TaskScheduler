using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
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
using TaskScheduler.DataAcces;
using TaskScheduler.Models;

namespace TaskScheduler
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            jobTypeComboBox.SelectedIndex = 0;
            repeatComboBox.SelectedIndex = 0;
            calendar.SelectedDate = DateTime.Now;

            var daemon = new Thread(new ThreadStart(TimerStart));
            daemon.IsBackground = false;
            daemon.Start();
        }

        private void MoveFile()
        {
            string pathFrom = @"Z:\Example\doc.txt";
            string pathTo = @"Z:\doc.txt";

            File.Move(pathFrom, pathTo);
        }

        private void DownloadFile()
        {
            var webClient = new WebClient();
            webClient.DownloadFile(@"ftp://videodarom:1169@videodarom.no-ip.org/Upgrade.2018.BDRip.VideoDarom.ru.avi", @"C:\Upgrade.2018.BDRip.VideoDarom.ru.mkv");
        }

        private void SendEmail()
        {
            MailAddress from = new MailAddress("anurakishev97@gmail.com", "Aslan");
            MailAddress to = new MailAddress("grooves97@gmail.com");
            MailMessage message = new MailMessage(from, to);
            message.Subject = "For Test";
            message.Body = "<h2>Just for testing</h2>";
            message.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient("smtp.mail.ru", 587);
            smtp.Credentials = new NetworkCredential("anurakishev97@gmail.com", "aslanmaster123");
            smtp.EnableSsl = true;
            smtp.Send(message);
            Console.Read();
        }

        private void TimerStart()
        {
            TimerCallback timerCallback = new TimerCallback(CheckTasks);
            Timer timer = new Timer(timerCallback, null, 0, 60000);
        }

        private void CheckTasks(object state)
        {
            using (var context = new DataContext())
            {
                var works = context.Executions.ToList();

                foreach (var work in works)
                {
                    if (work.Date <= DateTime.Now)
                    {
                        switch (work.Repeat)
                        {
                            case 0: work.Date = work.Date.AddDays(7); break;
                            case 1: work.Date = work.Date.AddMonths(1); break;
                            case 2: work.Date = work.Date.AddYears(1); break;
                        }

                        switch (work.Type)
                        {
                            case 0:
                                var threadSendMail = new Thread(SendEmail);
                                threadSendMail.Start();
                                MessageBox.Show("Задача \"Отправка Email\" выполнена");
                                break;
                            case 1:
                                var threadDownload = new Thread(DownloadFile);
                                threadDownload.Start();
                                MessageBox.Show("Задача \"Скачка файла\" выполнена");
                                break;
                            case 2:
                                var threadMove = new Thread(MoveFile);
                                threadMove.Start();
                                MessageBox.Show("Задача \"Перемещение каталога\" выполнена");
                                break;
                        }
                    }
                }
                context.SaveChanges();
            }
        }

        private void StartPlanButtonClick(object sender, RoutedEventArgs e)
        {
            var work = new Execution
            {
                Date = (DateTime)calendar.SelectedDate,
                Type = jobTypeComboBox.SelectedIndex,
                Repeat = repeatComboBox.SelectedIndex
            };

            using (var context = new DataContext())
            {
                context.Executions.Add(work);
                context.SaveChanges();
            }

            MessageBox.Show("Задача запланирована");
        }
    }
}
