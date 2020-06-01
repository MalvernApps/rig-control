using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
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

namespace serialtest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static SerialPort _serialPort;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Connect(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("connect");

            _serialPort = new SerialPort();
            _serialPort.PortName = "COM4";//Set your board COM
            _serialPort.BaudRate = 9600;
            _serialPort.StopBits = StopBits.One;
            _serialPort.Parity = Parity.None;
            _serialPort.DataBits = 8;
            _serialPort.Handshake = Handshake.None;
            _serialPort.Open();
            //while (true)
            //{
            //    string a = _serialPort.ReadExisting();
            //    Console.WriteLine(a);
            //    System.Threading.Thread.Sleep(200);
            //}

            //Thread tid1 = new Thread(new ThreadStart(Thread1));

            Thread thread1 = new Thread(MainWindow.DoWork);
            thread1.Start();
        }
        public static void Thread1()
        {
            for (int i = 1; i <= 10; i++)
            {
                Console.Write(string.Format("Thread1 {0}", i));
            }

            //while (true)
            //{
            //    Console.WriteLine("t");
            //    string a = _serialPort.ReadExisting();
            //    Console.WriteLine(a);
            //    System.Threading.Thread.Sleep(200);
            //}
        }

        private void SendData(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Send");

            //http://www.plicht.de/ekki/civ/civ-p32.html

            List<byte> b = new List<byte>();

            // http://www.plicht.de/ekki/civ/civ-p41.html
            b.Add(0xfe);
            b.Add(0xfe);
            b.Add(0x64);
            b.Add(0xe0);
            b.Add(0x06);
            b.Add(0x01);
           // b.Add(0x00);
            b.Add(0xfd);

            byte[] d = b.ToArray();
            _serialPort.Write(d, 0, d.Length );
        }

        private void SendFreq(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Send Freq");

            //http://www.plicht.de/ekki/civ/civ-p32.html

            List<byte> b = new List<byte>();

            // http://www.plicht.de/ekki/civ/civ-p41.html
            b.Add(0xfe);
            b.Add(0xfe);
            b.Add(0x64);
            b.Add(0xe0);
            b.Add(0x05);
            b.Add(0x30);
            b.Add(0x21);
            b.Add(0x80);
            b.Add(0x14);
            b.Add(0x00);
            b.Add(0xfd);

            byte[] d = b.ToArray();
            _serialPort.Write(d, 0, d.Length);
        }

        public static void DoWork()
        {
            while (true)
            {
                // Console.WriteLine("t");
                string a = _serialPort.ReadExisting();
                if ( a != string.Empty) Console.WriteLine(a);
                System.Threading.Thread.Sleep(200);
            }

            //for (int i = 0; i < 5; i++)
            //{
            //    Console.WriteLine("Working thread...");
            //    Thread.Sleep(100);
            //}
        }

        public class ThreadWork
        {
            public static void DoWork()
            {
                for (int i = 0; i < 3; i++)
                {
                    Console.WriteLine("Working thread...");
                    Thread.Sleep(100);
                }
            }
        }

        /// <summary>
        /// app is closing, sort comm port
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_serialPort != null && _serialPort.IsOpen) 
                _serialPort.Close();
        }
    }
}
