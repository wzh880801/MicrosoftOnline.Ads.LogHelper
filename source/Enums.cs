using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicrosoftOnline.Ads.LogAssistant
{
    public enum LogCategoryType
    {
        Unknown = 0,
        System,
        Javascript,
        Database,
        Cache,
        BrowserClient,
        Exception,
        Localization,
        Logic,
        Supporting,
        Email
    }

    public enum LogLevel
    {
        Info,
        Warn,
        Error
    }
}
