using MiniHttp.Forms;
using MiniHttp.Server;

namespace MiniHttp.Mediators;

public static class DelegateMediator
{
    public static InteropForm mainForm;
    
    public static MiniHttpServer.ProcessGet GetRequest() =>
        mainForm.TestGet;
    
    public static MiniHttpServer.ProcessPost PostRequest() =>
        mainForm.PostRequest;
}