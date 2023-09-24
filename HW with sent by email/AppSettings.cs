using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHttpServer
{
    public class AppSettings
    {
        public int Port { get; set; }
        public string Address { get; set; }
        public string StaticFilesPath { get; set; }
    }
}
