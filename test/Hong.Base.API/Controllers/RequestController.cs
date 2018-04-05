using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Hong.Common.Extendsion;
using Hong.Base.API.TestModel;
using System.Diagnostics;

namespace Hong.Base.API.Controllers
{
    [Route("api/request")]
    public class RequestController: Controller
    {
        ILogger Log { get; set; }

        public RequestController(ILoggerFactory loggerFactory)
        {
            Log = loggerFactory.CreateLogger("RequestController");
        }

        [HttpGet]
        public void Index()
        {
            Task masterTask = new Task(() =>
            {
                for (var index = 0; index < 10; index++)
                {
                    new Task(() => TestHashCode(),TaskCreationOptions.AttachedToParent).Start();
                }
            });

            masterTask.Start();
            masterTask.Wait();
        }

        void TestHashCode()
        {
            var requestModelTest = ServiceProvider.GetRequestServices<ReqestModelTest>();
            Debug.WriteLine("----------------------Task ThtreadID:" + System.Threading.Thread.CurrentThread.ManagedThreadId + "|ReqestModelTest Hashcode:" + requestModelTest.GetHashCode());
        }
    }
}
