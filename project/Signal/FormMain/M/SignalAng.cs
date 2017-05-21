using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormMain
{
    class SignalAng
    {
        String lampId;

        public String LampId
        {
            get { return lampId; }
            set { lampId = value; }
        }
        int eAng;

        public int EAng
        {
            get { return eAng; }
            set { eAng = value; }
        }
        int sAng;

        public int SAng
        {
            get { return sAng; }
            set { sAng = value; }
        }
        int wAng;

        public int WAng
        {
            get { return wAng; }
            set { wAng = value; }
        }
        int nAng;

        public int NAng
        {
            get { return nAng; }
            set { nAng = value; }
        }

        public override string ToString()
        {
            return "lampID: "+ lampId 
                + "; Angle: E: " + eAng 
                + "; S: " + sAng 
                + "; W: " + wAng
                + "; N: " + NAng;
        }
    }
}
