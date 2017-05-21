using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormMain
{
    class SignalPos
    {
        String lampId;

        public String LampId
        {
            get { return lampId; }
            set { lampId = value; }
        }
        int compass;

        public int Compass
        {
            get { return compass; }
            set { compass = value; }
        }
        int dir;

        public int Dir
        {
            get { return dir; }
            set { dir = value; }
        }
        int x;

        public int X
        {
            get { return x; }
            set { x = value; }
        }
        int y;

        public int Y
        {
            get { return y; }
            set { y = value; }
        }

        public override string ToString()
        {
            return "Lamp: "+lampId
                + "compass: " + compass
                + "dir: " + dir
                + "X: " + x
                + "Y: " + y;
        }
    }

    class OnePoint
    {
        public OnePoint(int compass, int dir, int x, int y)
        {

        }
    }

    class OneDirPoint
    {
        Point[] points = new Point[4];  // strait, left, right, walk

        public Point[] Points
        {
            get { return points; }
            set { points = value; }
        }
    }
}
