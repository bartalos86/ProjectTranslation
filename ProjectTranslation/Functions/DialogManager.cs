using ProjectTranslation.ViewModels;
using System.Windows;

namespace ProjectTranslation.Functions
{
    public static class DialogManager
    {
        public static MessageBoxResult Show(string message, string title, MessageBoxButton btnLayout = MessageBoxButton.OK)
        {
            DialogWindow dialogWindow = new DialogWindow(btnLayout);
            var dataContext = (DialogWindowViewModel)dialogWindow.DataContext;
            dataContext.Message = message;
            dataContext.Title = title;

            if(dialogWindow.ShowDialog() == true)
            {
                return dataContext.Result;
            }
            else
            {
                return MessageBoxResult.None;
            }


        }

    }
}
