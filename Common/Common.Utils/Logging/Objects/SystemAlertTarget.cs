using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Utils.Logging.Objects
{
    public class SystemAlertTarget
    {
        public string Email { get; set; }
        public int Id { get; set; }
        public bool IsActive { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Type { get; set; }
    }
}
