using Common.Utils.Standard;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForesterCmsServices.UI.Models
{
    public class DatasourceResult : ContentResult
    {
        public DatasourceResult(string key, object value = null) : base()
        {
            Content = $"datasource.{key} = {JsonHelper.Serialize(value)}";
            ContentType = "application/javascript";
        }
    }
}
