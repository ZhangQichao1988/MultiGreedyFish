using NetWorkModule.Dummy;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf;

public class FishDummy : BaseDummyData
{
    public FishDummy(Dictionary<string, MessageParser> parsers) : base(parsers)
    {   
        pbResProcesss.Add("P0_Response", typeof(ProcesserP0Res));
        pbResProcesss.Add("P1_Response", typeof(ProcesserP1Res));
        pbResProcesss.Add("P2_Response", typeof(ProcesserP2Res));
        pbResProcesss.Add("P3_Response", typeof(ProcesserP3Res));
        pbResProcesss.Add("P4_Response", typeof(ProcesserP4Res));
        pbResProcesss.Add("P5_Response", typeof(ProcesserP5Res));
        pbResProcesss.Add("P6_Response", typeof(ProcesserP6Res));
        pbResProcesss.Add("P7_Response", typeof(ProcesserP7Res));
        pbResProcesss.Add("P8_Response", typeof(ProcesserP8Res));
        pbResProcesss.Add("P9_Response", typeof(ProcesserP9Res));
        pbResProcesss.Add("P10_Response", typeof(ProcesserP10Res));
        pbResProcesss.Add("P11_Response", typeof(ProcesserP11Res));
        pbResProcesss.Add("P12_Response", typeof(ProcesserP12Res));
        pbResProcesss.Add("P13_Response", typeof(ProcesserP13Res));
        pbResProcesss.Add("P14_Response", typeof(ProcesserP14Res));
        pbResProcesss.Add("P15_Response", typeof(ProcesserP15Res));
        pbResProcesss.Add("P16_Response", typeof(ProcesserP16Res));
        pbResProcesss.Add("P18_Response", typeof(ProcesserP18Res));
        pbResProcesss.Add("P19_Response", typeof(ProcesserP19Res));

    }
}