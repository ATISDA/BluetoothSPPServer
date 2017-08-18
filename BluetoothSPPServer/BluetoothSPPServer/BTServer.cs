using System;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using InTheHand.Net;
using System.Net.Sockets;
using System.Text;
using System.ComponentModel;

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

        private bool deviceFound = true;
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
                if (deviceConnected != value)
                {
                    deviceConnected = value;
                    OnPropertyChanged("DeviceConnected"); // raising this event is key to have binding working properly
                }
            }
        }

        public BTServer()
        {
            BackgroundWorker worker = new BackgroundWorker();
            BC = new BluetoothClient(EP);
            //BluetoothDeviceInfo[] devices = BC.DiscoverDevices(10, true, true, false);
            //foreach (BluetoothDeviceInfo device in devices)
            //{
            //    string blueToothInfo = string.Format("- DeviceName: {0}{1}  Connected: {2}{1}  Address: {3}{1}  Last seen: {4}{1}  Last used: {5}{1}",
            //               device.DeviceName, Environment.NewLine, device.Connected, device.DeviceAddress, device.LastSeen,
            //               device.LastUsed);
            //    Console.WriteLine(blueToothInfo);
            //}

            worker.DoWork += workerDoWork;
            worker.RunWorkerAsync();

        }

        private void workerDoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            do
            {
                //Console.WriteLine(BTDevice.Connected);
                // If Scanner is connected
                BTDevice.Refresh();
                if (BTDevice.Connected)
                {
                    this.deviceFound = true;
                    Console.WriteLine("Device connected");
                    if (!BC.Connected)
                    {
                        this.deviceConnected = true;
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
                    System.Threading.Thread.Sleep(5000);
                }
                else
                {
                    this.deviceFound = false;
                    Console.WriteLine("Refresh Device Status");
                    BTDevice.Refresh();
                    System.Threading.Thread.Sleep(5000);
                }
            } while (true);
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
                    numberOfBytesRead = stream.Read(myReadBuffer, 0, myReadBuffer.Length);
                    myCompleteMessage.AppendFormat("{0}", Encoding.ASCII.GetString(myReadBuffer, 0, numberOfBytesRead));
                        
                    Console.WriteLine("You received the following message : " + myCompleteMessage);
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

        void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
