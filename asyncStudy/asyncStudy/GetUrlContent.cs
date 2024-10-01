using System.Diagnostics;
using System.Threading.Tasks;

namespace asyncStudy
{
    public class GetUrlContent
    {
        private CancellationTokenSource _cts;
        

        /// <summary>
        /// ModelView에서 CancellationToken 쓰기
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetUrlContentLengthAsync(CancellationToken ct)
        {
            Debug.WriteLine("1");

            using var client = new HttpClient();

            Debug.WriteLine("2");

            try
            {
                var res = await client.GetStringAsync("https://learn.microsoft.com/dotnet", ct);

                Debug.WriteLine("3");

                DoIndependentWork();

                Debug.WriteLine("4");

                return res.Length;
            }
            catch (TaskCanceledException)
            {
                Debug.WriteLine("Task was cancelled.");
                throw new OperationCanceledException();
            }
        }

        /// <summary>
        /// Model에서 CancellationToken 쓰기
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetUrlContentLengthAsync_cts()
        {
            if (_cts != null)
            {
                _cts.Cancel(); // 이전 작업 취소
                _cts.Dispose();
            }

            _cts = new CancellationTokenSource();

            Debug.WriteLine("1");

            using var client = new HttpClient();

            Debug.WriteLine("2");

            try
            {
                var res = await client.GetStringAsync("https://learn.microsoft.com/dotnet", _cts.Token);

                Debug.WriteLine("3");

                DoIndependentWork();

                Debug.WriteLine("4");

                return res.Length;
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine("Operation was cancelled.");
                throw new OperationCanceledException();
            }
            finally
            {
                _cts?.Dispose();
                _cts = null;
            }
        }

        void DoIndependentWork()
        {
            Debug.WriteLine("Working...");
            
        }

    }
}
