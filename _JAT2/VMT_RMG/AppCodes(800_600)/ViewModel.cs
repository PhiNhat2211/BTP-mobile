using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Input;

namespace VMT_RMG_800by600
{
    public class ViewModel : INotifyPropertyChanged
    {
        private static readonly ViewModel _theOnly = null;
        public static ViewModel Singleton
        {
            get { return _theOnly; }
        }

        private bool _isPlaying = false;
        private RelayCommand _playCommand = null;

        static ViewModel()
        {
            _theOnly = new ViewModel();
        }

        private ViewModel()
        {
            isPlaying = false;
        }

        public bool isPlaying
        {
            get { return _isPlaying; }
            set
            {
                _isPlaying = value;
                OnPropertyChanged("isPlaying");
            }
        }

        public ICommand PlayCommand
        {
            get
            {
                return _playCommand ?? new RelayCommand((x) =>
                {
                    var buttonType = x.ToString();

                    if (null != buttonType)
                    {
                        if (buttonType.Contains("Play"))
                        {
                            isPlaying = false;
                        }
                        else if (buttonType.Contains("Stop"))
                        {
                            isPlaying = true;
                        }
                    }
                });
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Predicate<object> _canExecute;
        private readonly Action<object> _execute;

        public event EventHandler CanExecuteChanged;

        public RelayCommand(Action<object> execute) : this(execute, null) { }

        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {

            if (_canExecute == null)
            {
                return true;
            }

            return _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, EventArgs.Empty);
            }
        }
    }
}
