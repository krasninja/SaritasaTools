// Copyright (c) 2015-2019, Saritasa. All rights reserved.
// Licensed under the BSD license. See LICENSE file in the project root for full license information.

using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Saritasa.Tools.Domain.Exceptions;

namespace Saritasa.Tools.EFCore
{
    /// <summary>
    /// <see cref="DbSet{TEntity}" /> extensions.
    /// </summary>
    public static class DbSetExtensions
    {
        /// <summary>
        /// Get entity by key values. Throws <see cref="NotFoundException" /> if entity not found.
        /// </summary>
        /// <param name="entities">DbSet instance.</param>
        /// <param name="keyValues">Keys.</param>
        /// <typeparam name="TEntity">Entity type.</typeparam>
        /// <returns>Entity instance.</returns>
        /// <exception cref="NotFoundException">Throws if entity not found.</exception>
        public static async Task<TEntity> GetAsync<TEntity>(this DbSet<TEntity> entities, params object[] keyValues)
            where TEntity : class
        {
            var entity = await entities.FindAsync(keyValues).ConfigureAwait(false);
            if (entity == null)
            {
                var ids = string.Join(", ", keyValues.Select(k => k.ToString()));
                throw new NotFoundException(
                    $"Cannot find entity {typeof(TEntity).Name} with identifier {ids}.");
            }
            return entity;
        }
    }
}
