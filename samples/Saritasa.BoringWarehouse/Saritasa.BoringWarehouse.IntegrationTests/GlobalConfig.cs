﻿using Autofac;
using NUnit.Framework;
using Saritasa.BoringWarehouse.Domain;
using Saritasa.BoringWarehouse.Domain.Users.Commands;
using Saritasa.BoringWarehouse.Domain.Users.Entities;
using Saritasa.BoringWarehouse.Infrastructure;
using Saritasa.Tools.Messages.Abstractions;

namespace Saritasa.BoringWarehouse.IntegrationTests
{
    /// <summary>
    /// Global configuration for all tests.
    /// </summary>
    [SetUpFixture]
    public class GlobalConfig
    {
        /// <summary>
        /// Autofac container.
        /// </summary>
        public static IContainer Container { get; private set; }

        /// <summary>
        /// User id of administrator.
        /// </summary>
        public static int AdminId { get; private set; }

        private ICommandPipeline commandPipeline;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var builder = CommonDIConfig.CreateBuilder();
            Container = builder.Build();

            commandPipeline = Container.Resolve<ICommandPipeline>();
            using (var uow = Container.Resolve<IAppUnitOfWork>())
            {
                CreateAdmin(uow);
            }
        }

        private void CreateAdmin(IAppUnitOfWork uow)
        {
            var command = new CreateUserCommand
            {
                FirstName = "Admin",
                LastName = "Admin",
                Email = "admin@example.com",
                Password = "secret",
                Role = UserRole.Admin
            };
            commandPipeline.Handle(command);

            AdminId = command.UserId;
        }
    }
}