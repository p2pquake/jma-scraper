using System;
using System.Collections.Generic;
using System.Text;

namespace P2PQuake.JMAInformation.Quake.Convert
{
    /// <summary>発表の種類</summary>
    enum IssueType
    {
        /// <summary>不明</summary>
        Unknown = 0,
        /// <summary>震度速報</summary>
        ScalePrompt,
        /// <summary>震源に関する情報</summary>
        Destination,
        /// <summary>震度・震源に関する情報</summary>
        ScaleAndDestination,
        /// <summary>各地の震度に関する情報</summary>
        DetailScale,
        /// <summary>遠地地震に関する情報</summary>
        Foreign,
        /// <summary>その他の情報</summary>
        Other
    }

    /// <summary>訂正の有無</summary>
    enum CorrectType
    {
        /// <summary>なし</summary>
        None = 0,
        /// <summary>不明</summary>
        Unknown,
        /// <summary>震度</summary>
        ScaleOnly,
        /// <summary>震源</summary>
        DestinationOnly,
        /// <summary>震度・震源</summary>
        ScaleAndDestination,
    }

    /// <summary>日本への津波の有無</summary>
    enum DomesticTsunamiType
    {
        /// <summary>不明</summary>
        Unknown = 0,
        /// <summary>調査中</summary>
        Checking,
        /// <summary>なし</summary>
        None,
        /// <summary>若干の海面変動（被害の心配なし）</summary>
        NonEffective,
        /// <summary>津波注意報</summary>
        Watch,
        /// <summary>津波予報（種類はいずれか不明）</summary>
        Warning
    }

    /// <summary>海外での津波の有無</summary>
    enum ForeignTsunamiType
    {
        /// <summary>不明</summary>
        Unknown = 0,
        /// <summary>なし</summary>
        None,
        /// <summary>調査中</summary>
        Checking,

        /// <summary>震源の近傍: 小さな津波の可能性（被害をもたらす津波の心配なし）</summary>
        NonEffectiveNearby,
        /// <summary>震源の近傍: 津波の可能性</summary>
        WarningNearby,

        /// <summary>太平洋: 津波の可能性</summary>
        WarningPacific,
        /// <summary>太平洋: 広域で津波の可能性</summary>
        WarningPacificWide,

        /// <summary>インド洋: 津波の可能性</summary>
        WarningIndian,
        /// <summary>インド洋: 広域で津波の可能性</summary>
        WarningIndianWide,

        /// <summary>一般的に，この規模では津波の可能性</summary>
        Potential
    }

    /// <summary>観測点の種類</summary>
    enum ObservationPointType
    {
        /// <summary>不明</summary>
        Unknown = 0,

        /// <summary>気象庁</summary>
        JMA,
        /// <summary>地方公共団体</summary>
        Local,
        /// <summary>(独立行政法人)防災科学技術研究所</summary>
        NIED,
        /// <summary>気象庁以外</summary>
        LocalOrNIED
    }

    /// <summary>観測点の情報</summary>
    struct ObservationPoint
    {
        /// <summary>(IssueType.ScaleAndDestination) True: 地域, False: 市町村</summary>
        public bool isArea;

        /// <summary>震度(10倍, "弱"は-5, 推定は+1)</summary>
        public int seismicScale;
        /// <summary>都道府県 (不明な場合は空文字列)</summary>
        public string prefecture;
        /// <summary>観測点位置</summary>
        public string address;
        /// <summary>種類</summary>
        public ObservationPointType pointType;
    }
}

