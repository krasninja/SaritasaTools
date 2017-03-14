﻿// Copyright (c) 2015-2017, Saritasa. All rights reserved.
// Licensed under the BSD license. See LICENSE file in the project root for full license information.

namespace Saritasa.Tools.EFCore
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using JetBrains.Annotations;
    using Domain;

    /// <summary>
    /// Entity framework base implementation of Unit of Work.
    /// </summary>
    /// <typeparam name="TContext">Context type should be inherited of DbContext.</typeparam>
    public class EFUnitOfWork<TContext> : IUnitOfWork where TContext : DbContext
    {
        /// <summary>
        /// Database context.
        /// </summary>
        protected TContext Context { get; private set; }

        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="context">Database context.</param>
        public EFUnitOfWork([NotNull] TContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            Context = context;
        }

        private bool disposed;

        /// <summary>
        /// Dispose object.
        /// </summary>
        /// <param name="disposing">Dispose managed resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (Context != null)
                    {
                        Context.Dispose();
                        Context = null;
                    }
                }
                disposed = true;
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc />
        public int SaveChanges()
        {
            return Context.SaveChanges();
        }

        /// <inheritdoc />
        public Task<int> SaveChangesAsync()
        {
            return Context.SaveChangesAsync();
        }

        /// <inheritdoc />
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return Context.SaveChangesAsync(cancellationToken);
        }
    }
}