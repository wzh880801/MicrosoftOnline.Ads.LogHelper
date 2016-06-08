# MicrosoftOnline.Ads.LogAssistant
Common Log Handler for C#

#Uasge
```C#
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MicrosoftOnline.Ads.LogAssistant;

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
                LogHelper.Error(LogCategoryType.Exception, "Main", ex.Message, new
                {
                    Person = new Person
                    {
                        Age = 20,
                        Name = "SuperCody"
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
```

#Log caught
##Common
```C#
{
	2016-04-27 14:24:58.594	Exception	[Error - Main]	Message = Attempted to divide by zero.
	TrackingId = a82af3a0-253c-4569-af80-14c014a51bcc
	Parameters[<>f__AnonymousType0`1[LogDemo.Program+Person]] = 
	{ 
		Person[LogDemo.Program+Person] = 
		{ 
			Age[System.Int32] = 20, 
			Name[System.String] = SuperCody 
		} 
	}
	Exception[System.DivideByZeroException] = 
	{ 
		Message[System.String] = Attempted to divide by zero., 
		Data[System.Collections.IDictionary] = System.Collections.ListDictionaryInternal, 
		TargetSite[System.Reflection.MethodBase] = <IngoredType>, 
		StackTrace[System.String] =    at LogDemo.Program.Main(String[] args) in D:\Git\MicrosoftOnline.Ads.LogHelper\example\LogDemo\Program.cs:line 17, 
		Source[System.String] = LogDemo, 
		HResult[System.Int32] = -2147352558 
	}
}
```
###Json
```js
{
  "trackingId": "5f1ab81f-1a27-4e16-abc7-ba1f2822f275", 
  "logDateTime": "2016-06-08T19:27:54.4434924Z", 
  "level": 2, 
  "category": 6, 
  "entry": "Main", 
  "message": "Attempted to divide by zero.", 
  "parameters": {
    "Person": {
      "Age": 20, 
      "Name": "SuperCody"
    }, 
    "Para": {
      "Url": "http://cn.bing.com"
    }
  }, 
  "exception": {
    "ClassName": "System.DivideByZeroException", 
    "Message": "Attempted to divide by zero.", 
    "Data": null, 
    "InnerException": null, 
    "HelpURL": null, 
    "StackTraceString": "   at LogDemo.Program.Main(String[] args) in D:\Git\MicrosoftOnline.Ads.LogHelper\example\LogDemo\Program.cs:line 18", 
    "RemoteStackTraceString": null, 
    "RemoteStackIndex": 0, 
    "ExceptionMethod": "8
Main
LogDemo, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
LogDemo.Program
Void Main(System.String[])", 
    "HResult": -2147352558, 
    "Source": "LogDemo", 
    "WatsonBuckets": null
  }, 
  "sensitiveProperties": null, 
  "sensitiveString": "******"
}
```


