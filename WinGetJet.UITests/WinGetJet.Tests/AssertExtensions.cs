using Xunit;

namespace WinGetJet.Tests;

// TODO: Move this to WAF?
public static class AssertExtensions
{
    extension(Assert)
    {
        /// <summary>Repeatedly retrieves a value and verifies it with the specified assertion until the assertion succeeds or the timeout elapses.</summary>
        /// <typeparam name="T">The type of the value to verify.</typeparam>
        /// <param name="getValue">Function that retrieves the current value. It is called once per attempt.</param>
        /// <param name="assert">Assertion that verifies the value. It signals failure by throwing an exception.</param>
        /// <param name="timeout">Maximum time to retry. Default is 10 seconds.</param>
        /// <param name="interval">Delay between two attempts. Default is 250 milliseconds.</param>
        /// <param name="cancellationToken">Token to cancel the delay between attempts.</param>
        /// <returns>The value that passed the assertion.</returns>
        /// <exception cref="Exception">The exception of the last failed assertion when the timeout has elapsed.</exception>
        /// <exception cref="OperationCanceledException">The <paramref name="cancellationToken"/> was canceled.</exception>
        public static async Task<T> RetryAsync<T>(Func<Task<T>> getValue, Action<T> assert, TimeSpan? timeout = null, TimeSpan? interval = null, 
            CancellationToken cancellationToken = default)
        {
            var end = DateTime.UtcNow + (timeout ?? TimeSpan.FromSeconds(10));
            while (true)
            {
                var value = await getValue();
                try
                {
                    assert(value);
                    return value;
                }
                catch (Exception) when (DateTime.UtcNow < end)
                {
                    await Task.Delay(interval ?? TimeSpan.FromSeconds(0.25), cancellationToken);
                }
            }
        }
    }
}
