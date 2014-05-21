using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Listener.Models
{
    public class IPNListenerModels
    {
    }

    public enum PriceType
    {
        Free,
        Priced,
        TermBased
    }

    public class AppInfo
    {
        public string AppId { get; set; }
        public PriceType priceType { get; set; }
        

    }

    public class LiveApp
    {
        public string ID { get; set; }
        public string Title { get; set; }
    }

    public class GetLiveAppsResult
    {
        public string Message { get; set; }
        public string User { get; set; }
        public List<LiveApp> LiveApps { get; set; }
    }
}