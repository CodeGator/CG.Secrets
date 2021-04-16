using CG.Validations;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace CG.Secrets.Stores
{
    /// <summary>
    /// This class contains extension methods related to the <see cref="IDistributedCache"/>
    /// type, for caching secrets.
    /// </summary>
    internal static partial class SecretDistributedCacheExtensions
    {
        // *******************************************************************
        // Public methods.
        // *******************************************************************

        #region Public methods

        /// <summary>
        /// This method puts an object of type <typeparamref name="T"/>
        /// into the cache, using the provided name.
        /// </summary>
        /// <typeparam name="T">The type of object to retrieve.</typeparam>
        /// <param name="cache">The cache to use for the operation.</param>
        /// <param name="name">The name of the cache entry to match.</param>
        /// <param name="obj">The object to be placed into the cache.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A task to perform the operation.</returns>
        public static async Task SetAsync<T>(
            this IDistributedCache cache,
            string name,
            T obj,
            CancellationToken cancellationToken = default
            ) where T : class
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(cache, nameof(cache))
                .ThrowIfNull(obj, nameof(obj))
                .ThrowIfNullOrEmpty(name, nameof(name));

            // Seserialize the object into JSON.
            var json = JsonSerializer.Serialize(obj);

            // Convert the string into UTF bytes.
            var bytes = Encoding.UTF8.GetBytes(json);

            // Put the entry into the cache.
            await cache.SetAsync(
                name,
                bytes,
                cancellationToken
                ).ConfigureAwait(false);
        }

        // *******************************************************************

        /// <summary>
        /// This method retrieves an object of type <typeparamref name="T"/>
        /// from the cache, provided a match is found.
        /// </summary>
        /// <typeparam name="T">The type of object to retrieve.</typeparam>
        /// <param name="cache">The cache to use for the operation.</param>
        /// <param name="name">The name of the cache entry to match.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A task to perform the operation, that returns an object 
        /// of type <typeparamref name="T"/> if a match is found, or NULL 
        /// otherwise.</returns>
        public static async Task<T> GetAsync<T>(
            this IDistributedCache cache,
            string name,
            CancellationToken cancellationToken = default
            ) where T : class
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(cache, nameof(cache))
                .ThrowIfNullOrEmpty(name, nameof(name));

            // Look for the entry in the cache.
            var buffer = await cache.GetAsync(
                name,
                cancellationToken
                ).ConfigureAwait(false);

            // Did we fail to find a match?
            if (null == buffer)
            {
                return null;
            }

            // Convert the bytes to JSON.
            var json = Encoding.UTF8.GetString(buffer);

            // Convert the json to an object of type T.
            var obj = JsonSerializer.Deserialize<T>(json);

            // Return the result.
            return obj;
        }

        #endregion
    }
}
