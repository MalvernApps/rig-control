using System;
using System.Collections.Generic;
using System.Diagnostics;
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

// this brilliant software has been realy useful on 14 day trial.
// https://www.eltima.com/products/serial-port-monitor/

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

            // connect up the RS232
            StartConnecting();

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

        /// <summary>
        /// connecting button has been pressed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Connect(object sender, RoutedEventArgs e)
        {            
            StartConnecting();
        }

        private void GetComPorts()
        {
            // Get a list of serial port names.
            string[] ports = SerialPort.GetPortNames();

            Console.WriteLine("The following serial ports were found:");

            // Display each port name to the console.
            foreach (string port in ports)
            {
                Console.WriteLine(port);
            }
        }

        /// <summary>
        /// make the rs232 connection setup for an icom
        /// </summary>
        private void StartConnecting()
        {
            Console.WriteLine("connecting up");

            _serialPort = new SerialPort();
            _serialPort.PortName = "COM8";//Set your board COM, look in device manager
            _serialPort.BaudRate = 19200; // use the fastest speed possible
            _serialPort.StopBits = StopBits.One;
            _serialPort.Parity = Parity.None;
            _serialPort.DataBits = 8;
            _serialPort.Handshake = Handshake.None;
            _serialPort.Open();
            _serialPort.DataReceived += _serialPort_DataReceived;

            //Thread tid1 = new Thread(new ThreadStart(Thread1));
            //Thread thread1 = new Thread(MainWindow.DoWork);
            //thread1.Start();

            // diable connect so we have no user mistakes causing a crash.
            connect.IsEnabled = false;
        }

        private void DecodePacket(List<byte> packet)
        {
            for( int x=0;x<packet.Count;x++)
            {
                Trace.WriteLine(x + ": " + packet[x].ToString("x2"));
            }

            switch( packet[4])
            {
                case 0x15:
                    // might be signal strength
                    if ( packet[5] == 0x02 && packet.Count == 9 )
                    {

                        int highlow = packet[6] & 0xF;
                        int high = packet[7] >> 4;
                        int low = packet[7] & 0xF;
                        int number = 100* highlow + 10 * high + low;
                        number /= 12;

                        // we have signal strength
                        //signalstrength.Text = packet[6].ToString();
                        signalstrength.Dispatcher.Invoke(new Action(() => { signalstrength.Text = number.ToString(); }));
                    }
                    break;

                case 0x0:
                    {
                        if (packet.Count == 10)
                        {
                            Trace.WriteLine("Frequency packet");
                            byte[] array = new byte[5];
                            array[0] = packet[4];
                            array[1] = packet[5];
                            array[2] = packet[6];
                            array[3] = packet[7];
                            array[4] = packet[8];
                            int outInt = (int) (BCD5ToInt(array));
                            double freq = outInt / 1e8;
                            Console.WriteLine(outInt);

                            //need an invoke here as we are on the wrong thread
                            signalstrength.Dispatcher.Invoke(new Action(() => { Freq.Text = freq.ToString(); }));
                        }
                    }
                    break;

            }

        }

        static List<byte> newPacket;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        private void ReCreateIcomPacket( byte [] data )
        {            
            for (int x=1;x<data.Length;x++)
            {
                if ( data[x] == 0xFe && data[x-1] == 0xFE )
                {
                    // we have a packet start
                    newPacket = new List<byte>();
                    newPacket.Add(0xFE);
                    newPacket.Add(0xFE);
                }
                else if ( data[x] == 0xfd )
                {
                    // we have a pcket end
                    newPacket.Add(0xFD);

                    // need to do something with it now
                    Console.WriteLine("Packet acquired for decoding");
                    DecodePacket(newPacket);
                }
                else
                {
                    // some error checking
                    if (newPacket == null)
                    {
                        // make a new packet
                        newPacket = new List<byte>();
                        newPacket.Add(0xFE);
                        newPacket.Add(0xFE);
                    }

                    newPacket.Add( data[x] );
                }
            }
        }

        private void _serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            // wait for final bytes to arrive as we are here pretty fast
            //Thread.Sleep(5);

            int bytestoread = _serialPort.BytesToRead;

            Trace.WriteLine("bytes available" + bytestoread );

            byte[] buffer = new byte[bytestoread];
            _serialPort.Read(buffer, 0, bytestoread);

            for(int x=0;x<bytestoread; x++)
            {
                Trace.WriteLine("x: " + buffer[x].ToString("x2"));
            }

            ReCreateIcomPacket(buffer);

            _serialPort.DiscardInBuffer();
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

            b.Add(0xfe);
            b.Add(0xfe);
            b.Add(0x64);
            b.Add(0xe0);
            b.Add(0x06);
            b.Add((byte) (modulation.SelectedIndex ));
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
                    //string a = _serialPort.ReadExisting();
                    int bytestoread = _serialPort.BytesToRead;
                    byte[] buffer = new byte[bytestoread];
                    _serialPort.Read(buffer, 0, bytestoread);

                    Console.WriteLine(bytestoread);
                }
                System.Threading.Thread.Sleep(100);
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
            for (int x = 0; x < 10; x++)
            {
                Console.WriteLine("Send");

                //http://www.plicht.de/ekki/civ/civ-p32.html

                List<byte> b = new List<byte>();

                SendHeader(b);

                b.Add(0x15);
                b.Add(0x02);    // S-meter level, return 2 btes of data                             

                SendFooter(b);

                byte[] d = b.ToArray();
                _serialPort.Write(d, 0, d.Length);

                Thread.Sleep(50);
            }
        }

        /// <summary>
        /// a simple test function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RunTest(object sender, RoutedEventArgs e)
        {
            GetComPorts();
        }
    }
}
