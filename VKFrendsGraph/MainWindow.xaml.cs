using System;
using System.Windows;
using System.Windows.Threading;
using System.ComponentModel;

namespace VKFrendsGraph
{
    public partial class MainWindow
    {
        // Об'єкт для роботи з потоками
        private readonly BackgroundWorker _worker = new BackgroundWorker();
        
        // Об'єкт для роботи з API
        private readonly VkApi _vk;

        public MainWindow()
        {
            InitializeComponent();

            // Підпис на подію процесу роботи потоку
            _worker.DoWork += _worker_DoWork;

            // Підпис на подію старту завершення роботи потоку
            _worker.RunWorkerCompleted += _worker_RunWorkerCompleted;
                       
            // Список алгоритмів побудови графіків
            Box.Items.Add("Tree"); 
            Box.Items.Add("LinLog");
            Box.Items.Add("KK");
            Box.Items.Add("ISOM");
            Box.Items.Add("FR");
            Box.Items.Add("EfficientSugiyama");
            Box.Items.Add("CompoundFDP");
            Box.Items.Add("Circular");
            Box.Items.Add("BoundedFR");
            _vk = new VkApi(2, this);
        }


        // Подія завершення потоку
        void _worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("Готово");
            Bar.Visibility = Visibility.Hidden;
            Bar.Value = 0;
            Bar.Maximum = 100;
            _vk.Progressflag = false;
        }

        // Подія процесу роботи потока
        void _worker_DoWork(object sender, DoWorkEventArgs e)
        {
            // Запуск функції повернення списку друзів
            string id = "";
            ID.Dispatcher.Invoke(DispatcherPriority.Normal,new Action(() => id = ID.Text));
            _vk.GetFrends(id, "");

        }

        // Подія натискання кнопки для запуску потоку
        private void GetFrends_Click(object sender, RoutedEventArgs e)
        {
            int f;
            if (int.TryParse(ID.Text, out f)) // Якщо id цифри
            {
                if (!_worker.IsBusy) // Якщо поток не зайнятий 
                {
                    _vk.Dictionary.Clear();
                    _worker.RunWorkerAsync(); // Запуск потоку
                }
                else
                {
                    MessageBox.Show("Програма вже опрацьовує"); // Поток вже працює
                }
            }
            else
            {
                MessageBox.Show("Не вірно введено ID користувача"); // Не вірно введено ID
            }   
        }
        // Подія натискання кнопки для побудови графу
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!_worker.IsBusy) // Якщо поток побудови списку друзів не активний
            {
                if (_vk.Dictionary.Count != 0) // В списку друзів більше одного запису
                {
                    if (Box.SelectedIndex != -1) // Вибрано алгоритм побудови графу
                    {
                        var form = new VisualGraph(_vk.Dictionary, this, (string)Box.SelectedItem); //(Список друзів, форма, алгоритм побудови)
                        form.Show();
                        Hide();
                    }
                    else
                    {
                        MessageBox.Show("виберіть алгоритм"); // Не вибраний алгоритм
                    }
                }
                else
                {
                    MessageBox.Show("Список пустий"); // Список друзів пустий
                }
            }
            else
            {
                MessageBox.Show("Список створюється"); // Поток побудови списку працює
            }
        }
    }
}
