using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AOP
{
    /// <summary>
    /// 静态拦截，通过装饰模式
    /// </summary>
   
    public interface IOrderService
    {
        void exec();
    }

    public class OrderService : IOrderService
    {
        public void exec(){
            Console.WriteLine("exec..........");
        }
    }

    public class OrderServiceDecorator : IOrderService
    {
        public IOrderService OrderService { get; set;}

        public OrderServiceDecorator(IOrderService orderService)
        {
            OrderService = orderService;
        }
        public void PreExec(){
            Console.WriteLine("PreExec..........");
        }

        public void PostExec(){
            Console.WriteLine("PostExec..........");
        }

        public void exec()
        {
            PreExec();
            OrderService.exec();
            PostExec();
        }
    }

    public class AOPTest
    {
        public static void Test()
        {
            IOrderService ios = new OrderService();
            OrderServiceDecorator osd = new OrderServiceDecorator(ios);
            osd.exec();

        }
    }
    

}
