using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TRMDesktopUI.EventModels;
using TRMDesktopUI.Library.Models;

namespace TRMDesktopUI.ViewModels
{
    public class ShellViewModel : Conductor<object>, IHandle<LogOnEvent>
    {
        private IEventAggregator _events;
        private SalesViewModel _salesVM;
        private ILoggedInUserModel _user;

        public ShellViewModel(IEventAggregator events, 
                              SalesViewModel salesVM, 
                              ILoggedInUserModel user)
        {
            _events = events;                        
            _salesVM = salesVM;
            _user = user;
            _events.SubscribeOnPublishedThread(this);

            ActivateItemAsync(IoC.Get<LoginViewModel>());
        }

        public void ExitApplication()
        {
            TryCloseAsync();
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

        public void LogOut()
        {
            _user.LogOutUser();
            ActivateItemAsync(IoC.Get<LoginViewModel>());
            NotifyOfPropertyChange(() => IsLoggedIn);
        }

        public async Task HandleAsync(LogOnEvent message, CancellationToken cancellationToken)
        {
            await ActivateItemAsync(_salesVM);
            NotifyOfPropertyChange(() => IsLoggedIn);
        }
    }
}
