using CG.Business.Stores;
using CG.Secrets.Models;
using CG.Secrets.Repositories;
using CG.Validations;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CG.Secrets.Stores
{
    /// <summary>
    /// This class is a default implementation of the <see cref="ISecretStore"/> 
    /// interface.
    /// </summary>
    public class SecretStore : StoreBase, ISecretStore
    {
        // *******************************************************************
        // Properties.
        // *******************************************************************

        #region Properties

        /// <summary>
        /// This property contains a logger instance.
        /// </summary>
        protected ILogger<SecretStore> Logger { get; }

        /// <summary>
        /// This property contains a reference to a secret repository.
        /// </summary>
        protected ISecretRepository SecretRepository { get; set; }

        /// <summary>
        /// This property contains a reference to a cache.
        /// </summary>
        protected IDistributedCache Cache { get; set; }

        #endregion

        // *******************************************************************
        // Constructors.
        // *******************************************************************

        #region Constructors

        /// <summary>
        /// This constructor creates a new instance of the <see cref="SecretStore"/>
        /// class.
        /// </summary>
        /// <param name="logger">The logger to use with the store.</param>
        /// <param name="cache">The cache to use with the store.</param>
        /// <param name="secretRepository">The repository to use with the store.</param>
        public SecretStore(
            ILogger<SecretStore> logger,
            IDistributedCache cache,
            ISecretRepository secretRepository
            ) 
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(logger, nameof(logger))
                .ThrowIfNull(cache, nameof(cache))
                .ThrowIfNull(secretRepository, nameof(secretRepository));

            // Save the references.
            Logger = logger;
            Cache = cache;
            SecretRepository = secretRepository;
        }

        #endregion

        // *******************************************************************
        // Public methods.
        // *******************************************************************

        #region Public methods

        /// <inheritdoc/>
        public virtual async Task<Secret> GetByNameAsync(
            string name,
            CancellationToken cancellationToken = default
            )
        {
            try
            {
                // Validate the parameters before attempting to use them.
                Guard.Instance().ThrowIfNullOrEmpty(name, nameof(name));

                // Look in the cache first.
                var secret = await Cache.GetAsync<Secret>(
                    name,
                    cancellationToken
                    ).ConfigureAwait(false);

                // Did we fail to find a match?
                if (null == secret)
                {
                    // Defer to the repository.
                    secret = await SecretRepository.GetByNameAsync(
                        name,
                        cancellationToken
                        ).ConfigureAwait(false);

                    // Did we find a value?
                    if (null != secret)
                    {
                        // Put the object in the cache.
                        await Cache.SetAsync(
                            name,
                            secret,
                            cancellationToken
                            ).ConfigureAwait(false);
                    }
                }

                // Return the results.
                return secret;
            }
            catch (Exception ex)
            {
                // Provide better context for the error.
                throw new StoreException(
                    message: $"Failed to query the value of a secret, by name!",
                    innerException: ex
                    ).SetCallerInfo()
                     .SetOriginator(nameof(SecretStore))
                     .SetDateTime();
            }
        }

        // *******************************************************************

        /// <inheritdoc/>
        public virtual async Task<Secret> SetByNameAsync(
            string name,
            string value,
            CancellationToken cancellationToken = default
            )
        {
            try
            {
                // Validate the parameters before attempting to use them.
                Guard.Instance().ThrowIfNullOrEmpty(name, nameof(name));

                // Defer to the repository.
                var secret = await SecretRepository.SetByNameAsync(
                    name,
                    value,
                    cancellationToken
                    ).ConfigureAwait(false);

                // Did we create a value?
                if (null != secret)
                {
                    // Put the object in the cache.
                    await Cache.SetAsync(
                        name,
                        secret,
                        cancellationToken
                        ).ConfigureAwait(false);
                }

                // Return the results.
                return secret;
            }
            catch (Exception ex)
            {
                // Provide better context for the error.
                throw new StoreException(
                    message: $"Failed to set the value of a secret, by name!",
                    innerException: ex
                    ).SetCallerInfo()
                     .SetOriginator(nameof(SecretStore))
                     .SetDateTime();
            }
        }

        #endregion
    }
}
