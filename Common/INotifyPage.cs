using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.UI.Xaml.Controls;

namespace GitMe.Common
{
    public enum NotifyType
    {
        StatusMessage,
        ErrorMessage
    };

    public interface INotifyPage
    {
        void NotifyUser(string strMessage, NotifyType type);
    }
}
