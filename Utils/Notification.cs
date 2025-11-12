using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Floatly.Utils
{
    public static class Notification
    {
        private static readonly Queue<(string message, string color)> NotificationQueue = new();
        private static readonly object _lock = new();
        public static event EventHandler<(string, string)> NotificationRaised;
        public static bool IsBusy = false;

        public static void ShowNotification(string message, string color = "AccentPurple")
        {
            lock (_lock)
            {
                NotificationQueue.Enqueue((message, color));
            }
        }

        public static async Task BackgroundNotificationWorker(CancellationToken token = default)
        {
            while (!token.IsCancellationRequested)
            {
                (string message, string color) item;

                lock (_lock)
                {
                    if (NotificationQueue.Count == 0)
                    {
                        item = (null, null);
                    }
                    else
                    {
                        item = NotificationQueue.Dequeue();
                    }
                }

                if (item.message == null)
                {
                    await Task.Delay(200, token); // idle wait
                    continue;
                }

                // wait for current notification to finish
                while (Volatile.Read(ref IsBusy))
                    await Task.Delay(100, token);

                IsBusy = true;
                try
                {
                    NotificationRaised?.Invoke(null, item);
                }
                finally
                {
                    // UI should reset IsBusy = false when animation ends
                }

                await Task.Delay(100, token); // slight delay before next
            }
        }
    }

}
