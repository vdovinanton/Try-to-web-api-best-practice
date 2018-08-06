
using AutoMapper;
using Ninject;
using Ninject.Modules;
using Ninject.Web.Common;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Dependencies;
using WebApplicationExercise.Core;
using WebApplicationExercise.Core.Interfaces;

namespace WebApplicationExercise.Utils
{
    /// <summary>
    /// Resolves Dependencies Using Ninject
    /// </summary>
    public class NinjectHttpResolver : IDependencyResolver, IDependencyScope
    {
        public IKernel Kernel { get; private set; }
        public NinjectHttpResolver(params NinjectModule[] modules)
        {
            Kernel = new StandardKernel(modules);
        }

        public NinjectHttpResolver(Assembly assembly)
        {
            Kernel = new StandardKernel();
            Kernel.Load(assembly);
        }

        public object GetService(Type serviceType)
        {
            return Kernel.TryGet(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return Kernel.GetAll(serviceType);
        }

        public void Dispose()
        {
            //Do Nothing
        }

        public IDependencyScope BeginScope()
        {
            return this;
        }
    }

    public class NinjectHttpModules
    {
        //Return Lists of Modules in the Application
        public static NinjectModule[] Modules
        {
            get
            {
                return new[] { new MainModule() };
            }
        }

        //Main Module For Application
        public class MainModule : NinjectModule
        {
            public override void Load()
            {
                //TODO: Bind to Concrete Types
                Bind<DataContext>().ToSelf().InRequestScope();
                Bind<IOrderService>().To<OrderService>();
                Bind<ICustomerService>().To<CustomerService>();
                Bind<ICurrencyService>().To<CurrencyService>();

                var mapperConfiguration = CreateConfiguration();
                Bind<MapperConfiguration>().ToConstant(mapperConfiguration).InSingletonScope();

                // This teaches Ninject how to create automapper instances say if for instance
                // MyResolver has a constructor with a parameter that needs to be injected
                Bind<IMapper>().ToMethod(ctx =>
                     new Mapper(mapperConfiguration, type => ctx.Kernel.Get(type)));
            }

            private MapperConfiguration CreateConfiguration()
            {
                var config = new MapperConfiguration(cfg =>
                {
                    // Add all profiles in current assembly
                    cfg.AddProfiles(GetType().Assembly);
                });

                return config;
            }
        }
    }


    /// <summary>
    /// Its job is to Register Ninject Modules and Resolve Dependencies
    /// </summary>
    public class NinjectHttpContainer
    {
        private static NinjectHttpResolver _resolver;

        //Register Ninject Modules
        public static void RegisterModules(NinjectModule[] modules)
        {
            _resolver = new NinjectHttpResolver(modules);
            GlobalConfiguration.Configuration.DependencyResolver = _resolver;
        }

        public static void RegisterAssembly()
        {
            _resolver = new NinjectHttpResolver(Assembly.GetExecutingAssembly());
            //This is where the actual hookup to the Web API Pipeline is done.
            GlobalConfiguration.Configuration.DependencyResolver = _resolver;
        }

        //Manually Resolve Dependencies
        public static T Resolve<T>()
        {
            return _resolver.Kernel.Get<T>();
        }
    }
}