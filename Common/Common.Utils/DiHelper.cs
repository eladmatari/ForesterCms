using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Utils
{
    public static class DiHelper
    {
        public static void Configure(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            Environment = serviceProvider.GetRequiredService<IHostingEnvironment>();
        }

        public static IServiceProvider ServiceProvider { get; private set; }

        public static IHostingEnvironment Environment { get; private set; }
    }
}
