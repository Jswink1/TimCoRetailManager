using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TRMDesktopUI.EventModels;
using TRMDesktopUI.Library.Api;
using TRMDesktopUI.Library.Models;

namespace TRMDesktopUI.ViewModels
{
    public class ShellViewModel : Conductor<object>, IHandle<LogOnEvent>
    {
        private IEventAggregator _events;
        private SalesViewModel _salesVM;
        private ILoggedInUserModel _user;
        private readonly IAPIHelper _apiHelper;

        public ShellViewModel(IEventAggregator events,
                              SalesViewModel salesVM,
                              ILoggedInUserModel user,
                              IAPIHelper apiHelper)
        {
            _events = events;                        
            _salesVM = salesVM;
            _user = user;
            _apiHelper = apiHelper;

            // Subscribe the ShellView to broadcasted events
            _events.SubscribeOnPublishedThread(this);

            // Open the LoginView on startup
            ActivateItemAsync(IoC.Get<LoginViewModel>());
        }

        public async Task HandleAsync(LogOnEvent message, CancellationToken cancellationToken)
        {
            // Open the SalesView when the LogOn Event is broadcasted
            await ActivateItemAsync(_salesVM);
            // Display the User Management menu item
            NotifyOfPropertyChange(() => IsLoggedIn);
        }        

        public bool IsLoggedIn
        {
            get 
            {
                bool enableAccountMenu = false;

                if (string.IsNullOrWhiteSpace(_user.Token) == false)
                {
                    enableAccountMenu = true;
                }

                return enableAccountMenu;
            }
        }

        public void UserManagement()
        {
            ActivateItemAsync(IoC.Get<UserDisplayViewModel>());
        }

        public void LogOut()
        {
            _user.ResetUserModel();
            _apiHelper.LogOutUser();
            ActivateItemAsync(IoC.Get<LoginViewModel>());
            NotifyOfPropertyChange(() => IsLoggedIn);
        }

        public void ExitApplication()
        {
            TryCloseAsync();
        }
    }
}
