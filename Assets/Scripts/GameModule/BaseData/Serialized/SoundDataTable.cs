using System;
using System.Collections.Generic;

[Serializable]
public class SoundDataTable : BaseDataTable<SoundData> {}


[Serializable]
public class SoundData : IQueryById
{
    public enum SceneType
    { 
        Common = 0,
        Sim          = 1,
        Battle       = 2,

    };
    public int scene;
}