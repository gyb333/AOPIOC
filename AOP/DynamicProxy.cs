using Microsoft.Practices.Unity.InterceptionExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using Microsoft.Practices.EnterpriseLibrary.PolicyInjection;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;

namespace AOP
{
    //定义特性方便使用
    public class LogHandlerAttribute : HandlerAttribute
    {
        public string LogInfo { get; set; }
        public override ICallHandler CreateHandler(IUnityContainer container)
        {
            return new LogHandler() { Order = 1, LogInfo = this.LogInfo };
        }
    }
    //注册对需要的Handler拦截请求
    public class LogHandler : ICallHandler
    {
        public int Order { get; set; }
        public string LogInfo { get; set; }

        public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
        {
            Console.WriteLine("LogInfo内容" + LogInfo);
            //0.解析参数
            var arrInputs = input.Inputs;
            if (arrInputs.Count > 0)
            {
                var oUserTest1 = arrInputs[0] ;
            }
            //1.执行方法之前的拦截
            Console.WriteLine("方法执行前拦截到了");
            //2.执行方法
            var messagereturn = getNext()(input, getNext);

            //3.执行方法之后的拦截
            Console.WriteLine("方法执行后拦截到了");
            return messagereturn;
        }
    }
    //用户定义接口和实现
    public interface IOperation
    {
        void operation();
    }

    public class Operation : MarshalByRefObject, IOperation
    {
        private static Operation single = null;
        public static Operation GetInstance()
        {
            if (single == null)
            {
                IConfigurationSource configurationSource = ConfigurationSourceFactory.Create();
                PolicyInjector policyInjector = new PolicyInjector(configurationSource);
                PolicyInjection.SetPolicyInjector(policyInjector);

                single = PolicyInjection.Create<Operation>();
            }
               
            return single;
        }

        [LogHandler(LogInfo ="operation的日志为Test")]
        public void operation()
        {
            Console.WriteLine("operation");
        }
    }

    public class DynamicProxyTest
    {
        public static void Test()
        {
            IOperation o = Operation.GetInstance();
            o.operation();
        }
    }
}
