using DXP;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class GenericOptions
{
    public static readonly Logger _Log = LogManager.GetLogger(Util.SERVERNAME);

    public bool GroupEnabled(DXP.TDocumentsBarGrouping Grouping)
    {
        _Log.Trace(System.Reflection.MethodBase.GetCurrentMethod().Name);

        IClient client = DXP.GlobalVars.Client;
        DXP.IClientAPI_Interface ClientAPI = client.GetClientAPI();

        return ClientAPI.GetGroupingInDocumentsBar() == Grouping ? true : false;

    }
    public void ChangeDocGrouping(DXP.TDocumentsBarGrouping Grouping)
    {
        _Log.Trace(System.Reflection.MethodBase.GetCurrentMethod().Name);

        IClient client = DXP.GlobalVars.Client;
        DXP.IClientAPI_Interface ClientAPI = client.GetClientAPI();

        ClientAPI.SetGroupingInDocumentsBar(Grouping);
        client.BroadcastNotification(new DXP.SystemNotification(DXP.NotificationCode.SystemNotificationCode.SystemPreferencesChanged));
    }


}

