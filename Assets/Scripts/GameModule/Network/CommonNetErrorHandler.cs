using NetWorkModule;
using System.Collections;
using System.Collections.Generic;

public class CommonNetErrorHandler : IErrorCodeProcesser
{
    private HashSet<int> commonErrTable = new HashSet<int>(){
        NetWorkResponseCode.GOOD_POOL_MAX_LEVEL,
        NetWorkResponseCode.NO_ENOUGH_DIAMOND,
        NetWorkResponseCode.NO_ENOUGH_GOLD
    };

    public void Process(int errorCode)
    {
        if (commonErrTable.Contains(errorCode))
        {
            //todo l10n
            MsgBox.OpenTips("NETWORK_ERROR_CODE_" + errorCode);
        }
    }
}