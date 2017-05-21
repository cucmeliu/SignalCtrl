using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FormMain
{
    class Option
    {
        Boolean autoConn;

        public Boolean AutoConn
        {
            get { return autoConn; }
            set { autoConn = value; }
        }
        String sigSwitchPath;

        public String SigSwitchPath
        {
            get { return sigSwitchPath; }
            set { sigSwitchPath = value; }
        }
        private String xtAddress;

        public String XtAddress
        {
            get { return xtAddress; }
            set { xtAddress = value; }
        }

     


        public override string ToString()
        {
            return "Option: autoconn= " + autoConn + "; sigPath=" + sigSwitchPath;
        }
    }
}
