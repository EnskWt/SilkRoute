using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilkRoute.Settings
{
    public class MicroserviceClientSettings
    {
        public Action<HttpClient>? HttpClientConfiguration { get; set; }
    }
}
