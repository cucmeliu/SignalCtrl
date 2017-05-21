using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FormMain
{
    abstract class SignalBase
    {
        private SignalProp signalProp;

        internal SignalProp SignalProp
        {
            get
            {
                return signalProp;
            }

            set
            {
                signalProp = value;
            }
        }

        public abstract String getState();
        public abstract Boolean openSpecDuty(int dirFrom, int dirTo);
        public abstract Boolean closeSpecDuty();
        
    }
}
