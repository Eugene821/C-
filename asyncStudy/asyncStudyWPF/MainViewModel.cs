using asyncStudy;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace asyncStudyWPF
{
    class MainViewModel : INotifyPropertyChanged
    {
        private CancellationTokenSource _cts;
        private bool _isBusy; // 작업 중복 방지를 위한 플래그

        private string _outputText;
        public string OutputText
        {
            get => _outputText;
            set
            {
                _outputText = value;
                OnPropertyChanged(nameof(OutputText));
            }
        }

        public ICommand GetUrlCommand { get; }
        public ICommand MGetUrlCommand { get; }

        private readonly GetUrlContent _getUrlContent;
        public MainViewModel()
        {
            _getUrlContent = new GetUrlContent();

            GetUrlCommand = new RelayCommand(() => GetUrl(), CanExecuteGetUrl);
            MGetUrlCommand = new RelayCommand(() => _GetUrl());
        }

        public async Task GetUrl()
        {
            if (_cts != null)
            {
                _cts?.Cancel();
                _cts?.Dispose();
            }

            _cts = new CancellationTokenSource();

            try
            {
                OutputText = "Fetching content length...\n";
                int length = await _getUrlContent.GetUrlContentLengthAsync(_cts.Token);
                OutputText += $"Content Length: {length}\n";
            }
            catch (OperationCanceledException) //스레드에서 실행 중인 작업을 취소할 때 해당 스레드에서 throw되는 예외
            {
                OutputText = "Operation was cancelled.\n";
            }
            finally
            {
                _cts?.Dispose();
                _cts = null;
            }
        }

        public async Task _GetUrl()
        {
            if (_isBusy)
                return; // 이미 작업 중이면 중복 실행을 방지

            _isBusy = true; // 작업 시작
            OnCommandCanExecuteChanged(); // 명령 상태 업데이트

            try
            {
                OutputText = "Fetching content length...\n";
                int length = await _getUrlContent.GetUrlContentLengthAsync_cts();
                OutputText += $"Content Length: {length}\n";
            }
            catch (OperationCanceledException)
            {
                OutputText = "Operation was cancelled.\n";
            }
            finally
            {
                _isBusy = false; // 작업 종료
                OnCommandCanExecuteChanged(); // 명령 상태 업데이트
            }
        }

        private bool CanExecuteGetUrl()
        {
            return _cts == null || _cts.IsCancellationRequested;
        }

        private void OnCommandCanExecuteChanged()
        {
            (GetUrlCommand as RelayCommand)?.RaiseCanExecuteChanged(); // RelayCommand가 다시 실행 가능 여부를 확인
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
