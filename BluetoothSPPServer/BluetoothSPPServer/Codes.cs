using System;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace BluetoothSPPServer
{
    public class Code : INotifyPropertyChanged
    {
        private DateTime timestamp;
        private string scanCode;

        public Code(DateTime date, string barcode)
        {
            this.timestamp = date;
            this.scanCode = barcode;
        }

        public DateTime Timestamp
        {
            get { return timestamp;  }
            set { timestamp = value; }
        }

        public string ScanCode
        {
            get { return scanCode; }
            set { scanCode = value; }
        }

        //INotifzPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class Codes : ObservableCollection<Code>
    {
        public Codes()
        {
            Add(new Code(new DateTime(1000), "45353"));
            Add(new Code(new DateTime(25424524), "45554323353"));
            Add(new Code(new DateTime(243693), "2588888"));
        }
    }
}
