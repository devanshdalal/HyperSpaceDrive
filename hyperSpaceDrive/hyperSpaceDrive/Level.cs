using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace hyperSpaceDrive
{
    public  class Level
    {
        public static int curl;
        public static int hl1;
        public static int hl2;
        public static int score;
        public static bool isNewScoreHigh = false;
        public Level(int high1,int high2){
            hl1 = high1;
            hl2 = high2;
        }
        public override string ToString()
        {
            return score + "";
        }
    }
}
