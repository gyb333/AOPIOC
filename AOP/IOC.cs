using Castle.Core;
using Castle.Core.Resource;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.MicroKernel.SubSystems.Configuration;
using System.Web;
using Castle.Windsor.Installer;

namespace AOP
{
    //实现Controller的实例创建解析和释放等功能
    public class WindsorControllerFactory : DefaultControllerFactory
    {
        private WindsorContainer container;
        public WindsorControllerFactory()
        {
            container = new WindsorContainer(new XmlInterpreter(new ConfigResource("castle")));
            var controllerTypes = from t in Assembly.GetExecutingAssembly().GetTypes()
                                  where typeof(IController).IsAssignableFrom(t) select t;
            foreach (Type t in controllerTypes)
            {
                container.Register(Component.For(t).Named(t.FullName).LifestyleTransient());
            }
        }
        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            return (IController)container.Resolve(controllerType);
        }

    }
    //建一个基于 IWindsorInstaller 的对容器的安装类
    public class CotrollersInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Classes.FromThisAssembly().BasedOn<IController>().LifestyleTransient()
                .Configure(c => c.DependsOn()));
        }
    }
    //注册WindsorControllerFactory 控制器工厂类
    public class IOCApplication:HttpApplication
    {
        private static IWindsorContainer container;

        private static void InitContainer()
        {
            container = new WindsorContainer().Install(FromAssembly.This());
            var controllerFactory = new WindsorControllerFactory();
            ControllerBuilder.Current.SetControllerFactory(controllerFactory);
        }

        protected void Application_Start()
        {
            InitContainer();
        }
        protected void Application_End()
        {
            container.Dispose();
        }

    }

}
