using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LabAnd
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            chartLive();
            chartLive2();
        }
        private int xValue=0; 
        private int xValue2=0; 
        public ChartValues<int> ChartValues { get; set; }
        public void chartLive()
        {
            // Инициализация данных
            ChartValues = new ChartValues<int>();

            // Привязка данных
            SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Данные",
                    Values = ChartValues
                }
            };

            // Инициализация начального значения X
            xValue = 0;

            DataContext = this;
        }

        public ChartValues<int> ChartValues2 { get; set; }
        public void chartLive2()
        {
            // Инициализация данных
            ChartValues2 = new ChartValues<int>();

            // Привязка данных
            SeriesCollection2 = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Данные",
                    Values = ChartValues2
                }
            };

            // Инициализация начального значения X
            xValue2 = 0;
            DataContext = this;
        }
        private string PortName = ""; // имя порта
        public SerialPort port = new SerialPort();
        //Получение портов
        public string[] ports = SerialPort.GetPortNames();
        //Выбор ком-порта
        private void CheckUSBCon_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {


            if (e.AddedItems.Count > 0)
            {
                var selectedItem = e.AddedItems[0];
                if (selectedItem.ToString() == "")
                {
                    port.DataReceived -= SerialPort_DataReceived;
                    port.Close();
                    //_timer.Elapsed -= OnTimedEvent;
                    Status.Text = "Отключено";
                    PortName = Status.Text;
                }
                else if (selectedItem != null)
                {
                    // LabelCRC.Content = selectedItem;
                    PortName = selectedItem.ToString();
                    try
                    {
                        port.Close();
                        SetingPorts por = new SetingPorts();
                        por.ConectPorts(ref port, PortName);
                        port.DataReceived += SerialPort_DataReceived;
                        port.Open();
                        Status.Text = "Подключено";
                        port.DtrEnable = true;
                        port.RtsEnable = true;
                        SerialPortReader(100);
                        //MessageBox.Show("Подключено на порт: "+PortName);
                    }
                    catch (Exception)
                    {
                        try
                        {
                            _timer.Elapsed -= OnTimedEvent;
                        }
                        catch (Exception)
                        {
                        }

                        Status.Text = "Отключено";
                    }

                }

            }

        }
        private void CheckUSBCon_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SetingPorts por = new SetingPorts();
            por.ChekingPorts(ref ports, ref CheckUSBCon);
            // Установите SelectedIndex в -1 после заполнения элементов, чтобы ничего не было выбрано
            CheckUSBCon.SelectedIndex = -1;
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                int bytes = port.BytesToRead;
                char[] buffer = new char[bytes];
                port.Read(buffer, 0, bytes);
                _buffer.AddRange(buffer);
                _dataReceivedRecently = true;
                // HEX_16_INB(buffer);
            }
            catch (Exception)
            {

            }

        }
        /*
        * Таймер для орбработки сообщений
        *
        */
        public List<char> _buffer = new List<char>();
        private System.Timers.Timer _timer;
        //private int _timeoutInterval = 5;
        private int _timeoutInterval = 1;
        private bool _dataReceivedRecently;
        public void SerialPortReader(int timeoutInterval)
        {
            _timer = new System.Timers.Timer(_timeoutInterval);
            _timer.Elapsed += OnTimedEvent;
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }
        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            if (_dataReceivedRecently)
            {
                _dataReceivedRecently = false;
                return;
            }

            if (_buffer.Count > 0)
            {
                // Consider the buffer as a complete message if no data has been received recently
                char[] message = _buffer.ToArray();
                _buffer.Clear();

                // Process the complete message
                HEX_16_INB(message);
            }
        }

        public delegate void SetTextDeleg2(string data, char[] InCommand);
        public SeriesCollection SeriesCollection { get; set; }
        public SeriesCollection SeriesCollection2 { get; set; }
        public void si_Status16(string data, char[] InCommand)
        {
            AppComand(data);
            if (onServo.IsChecked == true) {
                ChartValues.Add(int.Parse(data));
                xValue++;

            }
            if (onShagDV.IsChecked == true)
            {
                ChartValues2.Add(int.Parse(data));
                xValue2++;
            }
            if (shdKW.IsChecked==true)
            {
                UpdateUI(data);
            }
        }

        private void UpdateUI(string response)
        {
            // Если получена информация о датчиках
            if (response.Contains("Dat1"))
            {
                if (response.Contains("yes"))
                {
                    leftLEDStatusLabel.Text = "Достигнута";  // Обновление метки для Dat1
                }
                else
                {
                    leftLEDStatusLabel.Text = "Не достигнута";  // Обновление метки для Dat1
                }
            }
            else if (response.Contains("Dat2"))
            {
                if (response.Contains("yes"))
                {
                    rightLEDStatusLabel.Text = "Достигнута";  // Обновление метки для Dat2
                }
                else
                {
                    rightLEDStatusLabel.Text = "Не достигнута";  // Обновление метки для Dat2
                }
            }
            // Если получена информация о скорости
            else if (response.Contains("Speed"))
            {
                // Удаляем слово "Speed" из строки
                string cleanedResponse = response.Replace("Speed", "").Trim();

                // Обновление метки скорости
                speedStatusLabel.Text = cleanedResponse;
            }
        }
        private void HEX_16_INB(char[] a)
        {
            try
            {
                //var stringRemot = new StringRemot();
                string formattedAdd = new string(a);
                // Используем Dispatcher для вызова метода в UI thread
                Dispatcher.BeginInvoke(new SetTextDeleg2(si_Status16), formattedAdd, a);
            }
            catch (Exception)
            {


            }


        }

        private void AppComand(string command)
        {
            Command.AppendText(command);
            Command.ScrollToEnd();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string test = "hello\n";
            sendCom(test);
        }

        private void sendCom(string hello)
        {
            try
            {
                port.Write(hello);

            }
            catch (Exception)
            {
                AppComand("Не конект");

            }

        }

        bool statusBut1 = false;
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (statusBut1 == true)
            {
                blueBut.Background = Brushes.Azure;
                statusBut1 = false;
                sendCom("offBlue");
                return;
            }
            else
            {
                blueBut.Background = Brushes.Blue;
                statusBut1 = true;
                sendCom("onBlue");
                return;
            }
        }
           bool statusBut2 = false;
        private void redBut_Click(object sender, RoutedEventArgs e)
        {
            if (statusBut2 == true)
            {
                redBut.Background = Brushes.Pink;
                statusBut2 = false;
                sendCom("offRed");
                return;
            }
            else
            {
                redBut.Background = Brushes.Red;
                statusBut2 = true;
                sendCom("onRed");
                return;
            }
        }
        bool statusBut3 = false;
        private void greenBut_Click(object sender, RoutedEventArgs e)
        {
            if (statusBut3 == true)
            {
                greenBut.Background = Brushes.LightGreen;
                statusBut3 = false;
                sendCom("offGreen");
                return;
            }
            else
            {
                greenBut.Background = Brushes.Green;
                statusBut3 = true;
                sendCom("onGreen");
                return;
            }
        }


        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int value = Convert.ToInt32(((Slider)sender).Value);
            string valuestr = Convert.ToString(value);
            servoText.Text= valuestr;

            sendCom("s" + valuestr);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
                int value = Convert.ToInt32(ValueSHD.Text);
                if (value<0)
                {
                    MessageBox.Show("не верное число");
                    return;
                }
                string valuestr = Convert.ToString(value);
                sendCom("s" + valuestr);
            }
            catch (Exception)
            {
                MessageBox.Show("не верное число");
            }


        }

        private void DPTSet_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int value = Convert.ToInt32(DPTValue.Text);
                if (value < 0)
                {
                    MessageBox.Show("не верное число");
                    return;
                }
                string valuestr = Convert.ToString(value);
                sendCom("s" + valuestr);
            }
            catch (Exception)
            {
                MessageBox.Show("не верное число");
            }

        }

        private void leftSHD_Click(object sender, RoutedEventArgs e)
        {
            string valuestr = "left";
            sendCom(valuestr);
        }

        private void rightSHD_Click(object sender, RoutedEventArgs e)
        {
            string valuestr = "right";
            sendCom(valuestr);
        }

        private void stopSHDCk_Click(object sender, RoutedEventArgs e)
        {
            string valuestr = "stop";
            sendCom(valuestr);
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            string valuestr = "stop";
            sendCom(valuestr);
        }

        private void MoveLeftButton_Click(object sender, RoutedEventArgs e)
        {
            string valuestr = "left";
            sendCom(valuestr);
        }

        private void MoveRightButton_Click(object sender, RoutedEventArgs e)
        {
            string valuestr = "right";
            sendCom(valuestr);
        }

        private void DPTSetFinal_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int value = Convert.ToInt32(DPTValueFinal.Text);
                if (value < 0)
                {
                    MessageBox.Show("не верное число");
                    return;
                }
                string valuestr = Convert.ToString(value);
                sendCom("s" + valuestr);
            }
            catch (Exception)
            {
                MessageBox.Show("не верное число");
            }
        }
    }
}
