using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForesterCmsServices.UI.Models
{
    public class MetaTemplate
    {
        public int Id { get; set; }
        public string Template { get; set; }
        public TemplateType Type { get; set; }

        public enum TemplateType
        {
            Title,
            Keywords,
            Description,
            Robots
        }
    }
}
