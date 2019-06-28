using System;
using System.Windows;

namespace DiplomApp.ViewModels.Extensions
{
    static class WindowExtensions
    {
        public static Action GetClosingWindowAction(this Window window)
        {
            return () => window.Close();
        }
        public static Action<bool> GetDialogResultWindowAction(this Window window)
        {
            return (x) => window.DialogResult = x;
        }
    }
}
