using System;
using System.Collections.Generic;
using P2PQuake.JMAInformation.Quake;
using P2PQuake.JMAInformation.Quake.Convert;

namespace P2PQuake.JMAInformation.Quake.Convert
{
    public class JMAQuakeConverter
    {
        public static Core fromString(string content)
        {
            HTML2JMAQuake html2Jmaquake = new HTML2JMAQuake();
            JMAQuake quake = html2Jmaquake.Do(content);
            Core core = fromJMAQuake(quake);
            return core;
        }

        private static Core fromJMAQuake(JMAQuake jmaQuake)
        {
            Core core = new Core();

            // .expire
            core.expire = null;
            // .issue
            Issue issue   = new Issue();
            issue.source  = jmaQuake.issueOf;
            issue.time    = jmaQuake.issueTime.ToString("yyyy/MM/dd HH:mm:ss");
            // TODO: 要確認（toStringでいけるか？）
            issue.type    = jmaQuake.issueType.ToString();
            issue.correct = jmaQuake.correctType.ToString();
            core.issue    = issue;
            // .earthquake
            Earthquake earthquake = new Earthquake();
            earthquake.time       = jmaQuake.occuredTime.ToString("yyyy/MM/dd HH:mm:ss");
            {
                Hypocenter hypocenter = new Hypocenter();
                hypocenter.name       = jmaQuake.occuredPlace;
                hypocenter.latitude   = jmaQuake.occuredLatitude;
                hypocenter.longitude  = jmaQuake.occuredLongitude;
                hypocenter.depth      = jmaQuake.occuredDepth;
                hypocenter.magnitude  = jmaQuake.occuredMagnitude;
                earthquake.hypocenter = hypocenter;
            }
            earthquake.maxScale        = jmaQuake.occuredScale;
            earthquake.domesticTsunami = jmaQuake.domesticTsunamiType.ToString();
            earthquake.foreignTsunami  = jmaQuake.foreignTsunamiType.ToString();
            core.earthquake       = earthquake;
            // .points
            List<Point> points = new List<Point>();

            if (jmaQuake.observationPoints != null)
            {
                foreach (ObservationPoint observationPoint in jmaQuake.observationPoints)
                {
                    Point point = new Point();
                    point.pref = observationPoint.prefecture;
                    point.addr = observationPoint.address;
                    point.scale = observationPoint.seismicScale;
                    point.isArea = observationPoint.isArea;
                    // TODO: PointType(気象庁かそれ以外か)は捨てる？
                    points.Add(point);
                }
            }
            core.points        = points;

            return core;
        }
    }
}

