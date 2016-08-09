using System;
using System.Collections.Generic;

using P2PQuake.JMAInformation.Quake.Convert;

namespace P2PQuake.JMAInformation.Quake
{
    public class Core
    {
        public string      expire;
        public Issue       issue;
        public Earthquake  earthquake;
        public List<Point> points;

        public static Core CreateFromString(string content)
        {
            return JMAQuakeConverter.fromString(content);
        }
    }
}

