using DXP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class GenericOptions
{
    public bool GroupEnabled(DXP.TDocumentsBarGrouping Grouping)
    {
        IClient client = DXP.GlobalVars.Client;
        DXP.IClientAPI_Interface ClientAPI = client.GetClientAPI();

        return ClientAPI.GetGroupingInDocumentsBar() == Grouping ? true : false;

    }
    public void ChangeDocGrouping(DXP.TDocumentsBarGrouping Grouping)
    {
        IClient client = DXP.GlobalVars.Client;
        DXP.IClientAPI_Interface ClientAPI = client.GetClientAPI();

        ClientAPI.SetGroupingInDocumentsBar(Grouping);
        client.BroadcastNotification(new DXP.SystemNotification(DXP.NotificationCode.SystemNotificationCode.SystemPreferencesChanged));
    }


}

