using System.Windows;

public static class Notification
{
    private static readonly Queue<(string message, string color)> _queue = new();
    private static readonly SemaphoreSlim _signal = new(0);

    public static event EventHandler<(string, string)> NotificationRaised;

    public static void ShowNotification(string message, string color = "AccentPurple")
    {
        lock (_queue)
        {
            _queue.Enqueue((message, color));
        }
        _signal.Release();
    }

    public static async Task BackgroundNotificationWorker(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            await _signal.WaitAsync(token);

            (string msg, string color) item;
            lock (_queue)
            {
                item = _queue.Dequeue();
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                NotificationRaised?.Invoke(null, item);
            });
            await Task.Delay(4000);
        }
    }
}
