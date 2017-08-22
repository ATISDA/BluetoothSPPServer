using System;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using InTheHand.Net;
using System.Net.Sockets;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace BluetoothSPPServer
{
    public class BTServer : INotifyPropertyChanged
    {
        // My BT adapter
        private static BluetoothRadio  myRadio = BluetoothRadio.PrimaryRadio;
        private static BluetoothEndPoint EP = new BluetoothEndPoint(myRadio.LocalAddress, BluetoothService.BluetoothBase);
        private static BluetoothClient BC = new BluetoothClient(EP);
        // The BT device that would connect
        private static BluetoothDeviceInfo BTDevice = new BluetoothDeviceInfo(BluetoothAddress.Parse("20C38FD287AB"));
        private static NetworkStream stream = null;
        private BluetoothListener _listener = new BluetoothListener(EP);
        
        public event PropertyChangedEventHandler PropertyChanged;
        private BackgroundWorker worker;

        private Codes codes = new Codes();
        public Codes Codes
        {
            get { return this.codes; }
            set { this.codes = value; }
        }

        private bool deviceFound = false;
        /// <summary>
        /// True if the Device is found on the OS Bluetooth-List.
        /// </summary>
        public bool DeviceFound {
            get { return this.deviceFound; }
            set
            {
                if (this.deviceFound != value)
                {
                    this.deviceFound = value;
                    this.DeviceStatus = "Gerät Betriebssystem bekannt";
                    this.OnPropertyChanged("DeviceFound"); // raising this event is key to have binding working properly
                }
            }
        }

        private bool deviceConnected = false;
        /// <summary>
        /// True if the Device is connected, Authenticated and ready for Datatransmission.
        /// </summary>
        public bool DeviceConnected {
            get { return deviceConnected; }
            set
            {
                if (this.deviceConnected != value)
                {
                    this.deviceConnected = value;
                    this.DeviceStatus = "Scanner verbunden";
                    this.OnPropertyChanged("DeviceConnected"); // raising this event is key to have binding working properly
                }
            }
        }

        private string deviceStatus = "Status Unbekannt";
        public string DeviceStatus {
            get { 
                return this.deviceStatus;
            }
            set {
                this.deviceStatus = value;
                this.OnPropertyChanged("DeviceStatus");
            }
        }

        public BTServer()
        {
            worker = new BackgroundWorker();
            BC = new BluetoothClient(EP);

            worker.WorkerReportsProgress = true;
            worker.DoWork += workerDoWork;
            worker.ProgressChanged += worker_ProgressChanged;
            worker.RunWorkerAsync();

        }

        private void workerDoWork(object sender, DoWorkEventArgs e)
        {
            do
            {
                //Console.WriteLine(BTDevice.Connected);
                // If Scanner is connected
                BTDevice.Refresh();
                if (BTDevice.Connected)
                {
                    this.DeviceFound = true;
                    //this.OnPropertyChanged("DeviceFound");
                    Console.WriteLine("Device connected");
                    if (!BC.Connected)
                    {
                        this.DeviceConnected = true;
                        Console.WriteLine("BC Connected");
                        if (BluetoothSecurity.PairRequest(BTDevice.DeviceAddress, "1234"))
                        {
                            Console.WriteLine("PairRequest: OK");

                            if (BTDevice.Authenticated)
                            {
                                Console.WriteLine("Authenticated: OK");

                                //BC.SetPin("1234");

                                BC.BeginConnect(BTDevice.DeviceAddress, BluetoothService.SerialPort, new AsyncCallback(Connect), BTDevice);

                                System.Threading.Thread.Sleep(200);
                                //BC.Close();
                            }
                            else
                            {
                                Console.WriteLine("Authenticated: No");
                            }
                        }
                        else
                        {
                            Console.WriteLine("PairRequest: No");
                        }
                    }
                    this.DeviceConnected = false;
                    System.Threading.Thread.Sleep(5000);
                }
                else
                {
                    this.DeviceFound = false;
                    //this.OnPropertyChanged("DeviceFound");
                    Console.WriteLine("Refresh Device Status");
                    BTDevice.Refresh();
                    System.Threading.Thread.Sleep(5000);
                }
            } while (true);
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs code)
        {
            codes.Add((Code)code.UserState);
        }

        private async void Connect(IAsyncResult result)
        {
            Console.WriteLine(BC.Connected);
            stream = BC.GetStream();
            
            if (stream.CanRead)
            {
                byte[] myReadBuffer = new byte[1024];
                StringBuilder myCompleteMessage = new StringBuilder();
                int numberOfBytesRead;
                int counter = 0;

                // Incoming message may be larger than the buffer size. 
                do
                {
                    if (stream.DataAvailable)
                    {
                        string date;
                        string time;
                        string code;
                        DateTime timeStamp;
                        numberOfBytesRead = stream.Read(myReadBuffer, 0, myReadBuffer.Length);
                        myCompleteMessage.AppendFormat("{0}", Encoding.ASCII.GetString(myReadBuffer, 0, numberOfBytesRead));
                        Char delimiter = ',';
                        String[] parts = myCompleteMessage.ToString().Split(delimiter);
                        Console.WriteLine("You received the following message : " + myCompleteMessage);

                        date = parts[0];
                        time = parts[1];
                        code = parts[2].Trim();
                        timeStamp = Convert.ToDateTime(date + " " + time);
                        

                        worker.ReportProgress(50, new Code(timeStamp, code));
                        myCompleteMessage.Clear();
                        counter = 0;
                    }

                    counter++;                
                    System.Threading.Thread.Sleep(100);
                } while (BC.Connected && counter < 600);

                BC.Close();
                BC.Dispose();
                BC = new BluetoothClient(EP);
                Console.WriteLine("Streamschleife verlassen");
                // Print out the received message to the console.
            }
            else
            {
                Console.WriteLine("Sorry.  You cannot read from this NetworkStream.");
            }
        }

        private void OnPropertyChanged(string propName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propName));

            //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
