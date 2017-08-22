using System;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace BluetoothSPPServer
{
    public class Code
    {
        private DateTime timestamp;
        private string scanCode;

        public Code(DateTime date, string barcode)
        {
            this.timestamp = date;
            this.scanCode = barcode;
        }

        public string Timestamp
        {
            //get { return timestamp.ToShortDateString();  }
            get { return timestamp.ToString("dd.MM.yyyy HH:mm:ss"); }
            set { timestamp = DateTime.Parse(value); }
        }

        public string ScanCode
        {
            get { return scanCode; }
            set { scanCode = value; }
        }
    }

    public class Codes : ObservableCollection<Code>
    {
        public Codes()
        {
        }
    }
}
