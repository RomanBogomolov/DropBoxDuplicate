using DropBoxDuplicate.DataAccess;
using DropBoxDuplicate.DataAccess.Sql;
using System.Configuration;
using DropBoxDuplicate.Api.Models;
using DropBoxDuplicate.Api.Models.Validators;
using DropBoxDuplicate.Model;
using DropBoxDuplicate.Model.Validators;
using Microsoft.AspNet.Identity;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(DropBoxDuplicate.Api.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(DropBoxDuplicate.Api.App_Start.NinjectWebCommon), "Stop")]

namespace DropBoxDuplicate.Api.App_Start
{
    using System;
    using System.Web;
    using System.Web.Http;

    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using Ninject;
    using Ninject.Web.Common;

    using FluentValidation;
    using FluentValidation.WebApi;

    using DropBoxDuplicate.Api.Services;

    public static class NinjectWebCommon 
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start() 
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }
        
        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }
        
        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

                FluentValidationModelValidatorProvider.
                    Configure(GlobalConfiguration.Configuration, provider => provider.ValidatorFactory =
                        new NinjectValidatorFactory(kernel));

                RegisterServices(kernel);
                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["DBD"].ConnectionString;
            /*
             * DAL
             */
            kernel.Bind<IFileRepository>().To<FileRepository>().WithConstructorArgument(connectionString);
            kernel.Bind<IUserStore<IdentityUser, Guid>>()
                .To<IdentityUserRepository>()
                .WithConstructorArgument(connectionString);
            /*
             * Validators
             */
            kernel.Bind<IValidator<IdentityUser>>().To<IdentityUserValidator>();
            kernel.Bind<IValidator<ChangePasswordData>>().To<ChangePasswordDataValidator>();
        }        
    }
}
