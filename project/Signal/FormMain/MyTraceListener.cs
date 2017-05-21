using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormMain
{
    class MyTraceListener: TraceListener
    {
        public override void Write(string message)
        {
            File.AppendAllText(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "log\\SingalTracelog.log", message);
        }
        
        public override void WriteLine(string message)
        {
            File.AppendAllText(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "log\\SingalTracelog.log", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + message + Environment.NewLine);
        }

        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
        {
            base.TraceData(eventCache, source, eventType, id, data);
        }
    }
}
