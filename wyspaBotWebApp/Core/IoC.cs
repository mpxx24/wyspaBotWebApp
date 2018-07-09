using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;

namespace wyspaBotWebApp.Core {
    public static class IoC {
        private static IContainer container;

        private static ContainerBuilder builder;

        public static T Resolve<T>() {
            return container.Resolve<T>();
        }

        public static void Initialize(Module[] modules) {
            builder = new ContainerBuilder();

            if (modules != null) {
                foreach (var module in modules) {
                    builder.RegisterModule(module);
                }
            }
            else {
                return;
            }

            container = builder.Build();

            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}