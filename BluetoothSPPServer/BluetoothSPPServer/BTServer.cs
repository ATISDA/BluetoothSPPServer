using System;
using System.IO.Ports;
using System.Threading;

namespace BluetoothSPPServer
{
    public class BTServer
    {
        public BTServer()
        {
            // Get a list of serial port names.
            ListCOMPorts();


            SerialPort inPort3 = new SerialPort("COM3");
            SerialPort inPort4 = new SerialPort("COM4");
            inPort3.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
            inPort4.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

            inPort3.Open();
            inPort4.Open();
            //string mess = inPort.ReadLine();
            //inPort.Close();

            //Console.WriteLine(mess);
        }

        private void ListCOMPorts()
        {
            Console.WriteLine("The following serial ports were found:");
            string[] ports = SerialPort.GetPortNames();
            foreach (string port in ports)
            {
                Console.WriteLine(port);
            }
        }

        private static void DataReceivedHandler(
                        object sender,
                        SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string indata = sp.ReadExisting();
            Console.WriteLine("Data Received:");
            Console.Write(indata);
        }
    }
}
