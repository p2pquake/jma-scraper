using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace P2PQuake.JMAInformation.Quake.Convert
{
    class JMAQuake
    {
        // 発表の要素
        /// <summary>発表元</summary>
        public string issueOf;
        /// <summary>発表時刻</summary>
        public DateTime issueTime;
        /// <summary>発表種類</summary>
        public IssueType issueType;
        /// <summary>訂正報かどうか</summary>
        public CorrectType correctType;

        // 地震の要素
        /// <summary>発生時刻</summary>
        public DateTime occuredTime;
        /// <summary>震源地</summary>
        public string occuredPlace;
        /// <summary>緯度(北緯:+, 南緯:-) [度] （取得失敗時は -200）</summary>
        public double occuredLatitude;
        /// <summary>経度(東経:+, 西経:-) [度] （取得失敗時は -200）</summary>
        public double occuredLongitude;
        /// <summary>深さ [km]</summary>
        public int occuredDepth;
        /// <summary>マグニチュード</summary>
        public double occuredMagnitude;

        /// <summary>最大震度(10倍, "弱"は-5)</summary>
        public int occuredScale;

        // 津波の要素
        /// <summary>日本への津波の影響</summary>
        public DomesticTsunamiType domesticTsunamiType;
        /// <summary>海外への津波の影響</summary>
        public ForeignTsunamiType foreignTsunamiType;

        // 各地の震度
        public IList<ObservationPoint> observationPoints;

        public JMAQuake() : this(-1)
        {

        }

        public JMAQuake(int observationPointDefaultCapacity)
        {
            issueOf = "";
            issueTime = DateTime.MinValue;
            issueType = IssueType.Unknown;
            correctType = CorrectType.Unknown;

            occuredTime = DateTime.MinValue;
            occuredPlace = "";
            occuredLatitude = -200;
            occuredLongitude = -200;
            occuredDepth = -1;
            occuredMagnitude = -1;

            occuredScale = -1;

            domesticTsunamiType = DomesticTsunamiType.Unknown;
            foreignTsunamiType = ForeignTsunamiType.Unknown;

            if (observationPointDefaultCapacity >= 0)
                    observationPoints = new List<ObservationPoint>(observationPointDefaultCapacity); 
            else
                    observationPoints = new List<ObservationPoint>();
        }
    }
}
