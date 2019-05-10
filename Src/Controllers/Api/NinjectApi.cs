using Ninject;
using Ninject.Extensions.ChildKernel;
using Src.Models.Service.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Dependencies;

namespace Src.Controllers.Api
{
    public class NinjectApi : IDependencyResolver
    {
        private IKernel kernel;

        public NinjectApi() : this(new StandardKernel()) { }

        public NinjectApi(IKernel ninjectKernel, bool scope = false)
        {
            kernel = ninjectKernel;
            if (!scope)
            {
                AddBindings(kernel);
            }
        }

        private void AddBindings(IKernel kernel) { }

        private IKernel AddRequestBindings(IKernel kernel)
        {
            kernel.Bind<IUnitOfWork>().To<UnitOfWork>().InSingletonScope();
            return kernel;
        }

        public IDependencyScope BeginScope() => new NinjectApi(AddRequestBindings(new ChildKernel(kernel)), true);

        public object GetService(Type serviceType) => kernel.TryGet(serviceType);

        public IEnumerable<object> GetServices(Type serviceType) => kernel.GetAll(serviceType);

        public void Dispose() { }
    }
}