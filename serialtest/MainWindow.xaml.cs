using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
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

            //uint firstInt = 987654321;
            //var array = IntToBCD5(firstInt);
            //var outInt = BCD5ToInt(array);
            //MessageBox.Show(outInt.ToString());

            //modulation.ItemsSource = EnumModulation.GetValues(e.NewValue as Type); 
        }

        public uint BCD5ToInt(byte[] bcd)
        {
            uint outInt = 0;

            for (int i = 0; i < bcd.Length; i++)
            {
                int mul = (int)Math.Pow(10, (i * 2));
                outInt += (uint)(((bcd[i] & 0xF)) * mul);
                mul = (int)Math.Pow(10, (i * 2) + 1);
                outInt += (uint)(((bcd[i] >> 4)) * mul);
            }

            return outInt;
        }

        // Convert an unsigned integer into 5 bytes of 
        public byte[] IntToBCD5(uint numericvalue, int bytesize = 5)
        {
            byte[] bcd = new byte[bytesize];
            for (int byteNo = 0; byteNo < bytesize; ++byteNo)
                bcd[byteNo] = 0;
            for (int digit = 0; digit < bytesize * 2; ++digit)
            {
                uint hexpart = numericvalue % 10;
                bcd[digit / 2] |= (byte)(hexpart << ((digit % 2) * 4));
                numericvalue /= 10;
            }
            return bcd;
        }

        private void Connect(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("connect");

            _serialPort = new SerialPort();
            _serialPort.PortName = "COM7";//Set your board COM
            _serialPort.BaudRate = 9600;
            _serialPort.StopBits = StopBits.One;
            _serialPort.Parity = Parity.None;
            _serialPort.DataBits = 8;
            _serialPort.Handshake = Handshake.None;
            _serialPort.Open();            

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
        }

        private void Signal_Strength(object sender, RoutedEventArgs e)
        {
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
            b.Add( (byte) modulation.SelectedIndex);
           // b.Add(0x00);
            b.Add(0xfd);

            byte[] d = b.ToArray();
            _serialPort.Write(d, 0, d.Length );
        }

        private void SendHeader(List<byte> b)
        {
            b.Add(0xfe);
            b.Add(0xfe);
            b.Add(0x64);
            b.Add(0xe0);
        }

        private void SendFooter(List<byte> b)
        {
            b.Add(0xfd);
        }

        private void SendFreq(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Send Freq");

            //http://www.plicht.de/ekki/civ/civ-p32.html

            List<byte> b = new List<byte>();

            SendHeader(b);

            b.Add(0x05);
            double f = double.Parse(Freq.Text);
            int fi = (int) (f*1000000);
            uint firstInt = 14250000;
            var array = IntToBCD5((uint)fi);

            for (int x = 0; x < array.Length; x++)
                b.Add(array[x]);

      
            //b.Add(0x30);
            //b.Add(0x21);
            //b.Add(0x80);
            //b.Add(0x14);
            //b.Add(0x00);

            SendFooter(b);

            byte[] d = b.ToArray();
            _serialPort.Write(d, 0, d.Length);
        }

        public static void DoWork()
        {
            while (true)
            {
                if (_serialPort.IsOpen)
                {
                    // Console.WriteLine("t");
                    string a = _serialPort.ReadExisting();
                    if (a != string.Empty) Console.WriteLine(a);
                }
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

        private void Get_Signal_Strength(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// a simple test function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RunTest(object sender, RoutedEventArgs e)
        {
            int t = (int) radio.SelectedValue;
            Console.WriteLine(radio.SelectedValue);
        }
    }
}
