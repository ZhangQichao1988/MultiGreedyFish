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
    }
}