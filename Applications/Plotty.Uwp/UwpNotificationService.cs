using System;
using Microsoft.Toolkit.Uwp.UI.Controls;

namespace Plotty.Uwp.Reloaded
{
    public interface INotificationService
    {
        void Show(string message, TimeSpan timeSpan);
    }

    public class UwpNotificationService : INotificationService
    {
        private readonly InAppNotification notification;

        public void Show(string message, TimeSpan timeSpan)
        {
            notification.Show(message, (int)timeSpan.TotalMilliseconds);
        }

        public UwpNotificationService(InAppNotification notification)
        {
            this.notification = notification;
        }
    }
}