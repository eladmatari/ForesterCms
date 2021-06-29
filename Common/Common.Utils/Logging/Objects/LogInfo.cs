using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Utils.Logging.Objects
{
    public class LogInfo
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Exception { get; set; }

        public string Service { get; set; }

        public string Method { get; set; }

        public string Step { get; set; }

        public string Url { get; set; }

        public string Request { get; set; }

        public string Response { get; set; }

        public string PhoneNumber { get; set; }

        public string Server { get; set; }

        public string ClientIp { get; set; }

        public bool IsAlert { get; set; }

        public string MoreInfo { get; set; }

        public bool IsInserted { get; set; }
    }
}
