﻿// Copyright (c) 2015-2019, Saritasa. All rights reserved.
// Licensed under the BSD license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

// ReSharper disable once CheckNamespace
namespace Saritasa.Tools.Common.Pagination
{
    /// <summary>
    /// Class contains Entity Framework related static methods for
    /// <see cref="PagedList{T}" /> and is intended to simplify instantiation and better API.
    /// </summary>
    public static class EfPagedListFactory
    {
        /// <summary>
        /// Creates paged list from queryable source and query source by page and page size.
        /// The calling will evaluate query automatically.
        /// </summary>
        /// <typeparam name="T">Item type.</typeparam>
        /// <param name="source">Queryable enumerable.</param>
        /// <param name="page">Current page.</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>Paged list.</returns>
        public static async Task<PagedList<T>> FromSourceAsync<T>(
            IQueryable<T> source,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var offset = GetOffset(page, pageSize);
            return new PagedList<T>(
                await source.Skip(offset).Take(pageSize).ToListAsync(cancellationToken),
                page,
                pageSize,
                await source.CountAsync());
        }

        internal static int GetOffset(int page, int pageSize)
        {
            if (page < PagedList<object>.FirstPage)
            {
                throw new ArgumentOutOfRangeException(nameof(page));
            }
            if (pageSize < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(pageSize));
            }

            return (page - 1) * pageSize;
        }
    }
}
