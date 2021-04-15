using CG.Business.Stores;
using CG.Secrets.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CG.Secrets.Stores
{
    /// <summary>
    /// This interface represents a store for managing secrets.
    /// </summary>
    public interface ISecretStore : IStore
    {
        /// <summary>
        /// This method retrieves the value of a secret, by name.
        /// </summary>
        /// <param name="name">The name to use for the operation.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A task to perform the operation, that returns a 
        /// <see cref="Secret"/> object, if a match is found, or NULL otherwise.</returns>
        Task<Secret> GetByNameAsync(
            string name,
            CancellationToken cancellationToken = default
            );

        /// <summary>
        /// This method sets the value of a secret, by name
        /// </summary>
        /// <param name="name">The name to use for the operation.</param>
        /// <param name="value">The value to use for the operation.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A task to perform the operation, that returns a 
        /// <see cref="Secret"/> object, if a match is found, or NULL otherwise.</returns>
        Task<Secret> SetByNameAsync(
            string name,
            string value,
            CancellationToken cancellationToken = default
            );
    }
}
