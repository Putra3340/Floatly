using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Floatly.Utils
{
    public static class Notification
    {
        public static bool IsBusy = false;
        public static event EventHandler<(string,string)> NotificationRaised;
        public static async Task ShowNotification(string message,string rescolor = "AccentPurple")
        {
            while (IsBusy)
            {
                await Task.Delay(100);
            }
            NotificationRaised?.Invoke(null, (message,rescolor));
        }

    }
}
