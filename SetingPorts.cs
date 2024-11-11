using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace LabAnd
{
    internal class SetingPorts
    {
        public void ChekingPorts(ref string[] ports, ref ComboBox CheckUSBCon)
        {
            ports = SerialPort.GetPortNames();
            // Очистить список элементов перед добавлением новых
            CheckUSBCon.Items.Clear();
            CheckUSBCon.Items.Add("");

            // Добавить элементы из массива ports в список ComboBox
            foreach (var port in ports)
            {
                CheckUSBCon.Items.Add(port);
            }

            // Установить индекс выбранного элемента на первый элемент (если он доступен)
            if (CheckUSBCon.Items.Count > 0)
            {
                CheckUSBCon.SelectedIndex = 0;
            }
        }
        // port.BaudRate = 921_600;
        public void ConectPorts(ref SerialPort port, string T)
        {
            string PortName = T;
            port.PortName = PortName;
            port.BaudRate = 115_200;
            port.DataBits = 8;
            port.StopBits = StopBits.One;
        }
    }
}
