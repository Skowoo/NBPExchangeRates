using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeRatesLibrary
{
    public class DocumentDownloadedEvent : EventArgs
    {
        public int filesDone { get; init; }

        public int totalFilesToDownload { get; init; }

        public DocumentDownloadedEvent(int done, int total) 
        { 
            totalFilesToDownload = total;
            filesDone = done;
        }
    }
}
