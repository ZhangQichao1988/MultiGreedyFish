using System;
using System.Collections.Generic;

[Serializable]
public class LanguageDataTable : BaseDataTable<LanguageDataInfo> {}


[Serializable]
public class LanguageDataInfo : IQueryById
{
    public string cn;
    public string tw;
    public string en;
    public string jp;
}