using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MicrosoftOnline.Ads.LogAssistant;
using System.Net;

namespace LogDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                int zero = 0;
                var d = 100 / zero;
            }
            catch (Exception ex)
            {
                LogHelper.ErrorToJson(LogCategoryType.Exception, "Main", ex.Message, new
                {
                    Person = new Person
                    {
                        Age = 20,
                        Name = "SuperCody"
                    },
                    Para = new
                    {
                        Url = "http://cn.bing.com"
                    }
                }, ex);
            }
        }

        class Person
        {
            public int Age { get; set; }

            public string Name { get; set; }
        }
    }
}
