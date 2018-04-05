using Hong.WebSocket;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Hong.Common.Extendsion;
// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace WEB_NETCORE.Controllers
{
    public class SocketController : Controller
    {
        private WebSocketHandle SocketHandle { get; set; }
        private WebSocketSessionManger SessionManager { get; set; }

        public SocketController(WebSocketHandle webSocketHandle, WebSocketSessionManger sm)
        {
            SocketHandle = webSocketHandle;
            SessionManager = sm;
        }

        public IActionResult Index()
        {
            if (SessionManager.CurrentRequestSession == null)
            {
                SessionManager.CreateNewSession();
            }

            return View();
        }

        public void Broadcast()
        {
            SessionManager.BroadcastAsync("{\"msg\":\"测试消息,时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\"}");
        }
    }
}
