using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Threading;
using GalaSoft.MvvmLight;

namespace ExchangeTracker.Presentation.Common
{
    public class MyViewModelBase : ViewModelBase, INavigation
    {
        private bool _isBusy;
        private ObservableCollection<CommandObject> _commandObjects;
        private string _title;

        public MyViewModelBase()
        {
            CommandObjects = new ObservableCollection<CommandObject>();
            Title = ResourceHelper.GetResource(GetType().Name);
        }

        public ObservableCollection<CommandObject> CommandObjects
        {
            get { return _commandObjects; }
            set { _commandObjects = value; }
        }

        public virtual string Title
        {
            get { return _title; }
            set { Set(() => Title, ref _title, value); }
        }

        public virtual bool IsBusy
        {
            get { return _isBusy; }
            set { _isBusy = value; RaisePropertyChanged("IsBusy"); }
        }

        protected void AsyncCall<T>(Func<T> asyncFunc, Action<T> callbackAction)
        {
            Task.Factory.StartNew(asyncFunc).ContinueWith(result =>
                DoAction(() => Dispatcher.CurrentDispatcher.InvokeAsync(() => callbackAction(result.Result))));
        }

        protected void DoAction(Action action)
        {
            ExceptionHelper.DoAction(action);
        }

        public virtual bool NavigateExit()
        {
            return true;
        }

        public virtual void NavigateEnter()
        {

        }

        public virtual bool RefreshOnEnter()
        {
            return false;
        }
    }
}
