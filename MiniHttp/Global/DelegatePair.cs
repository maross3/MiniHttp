using MiniHttp.Server;

namespace MiniHttp.Global;
public class DelegatePair
{
    public string path;
    public RequestType type;
    public readonly MiniHttpServer.ProcessGet? getDelegate;
    public readonly MiniHttpServer.ProcessPost? postDelegate;
    
    public DelegatePair(string path, RequestType type, Server.MiniHttpServer.ProcessGet getDelegate, MiniHttpServer.ProcessPost postDelegate)
    {
        this.path = path;
        this.type = type;
        this.getDelegate = getDelegate;
        this.postDelegate = postDelegate;
    }

    public DelegatePair(string path, RequestType type, MiniHttpServer.ProcessGet getDelegate)
    {
        this.path = path;
        this.type = type;
        this.getDelegate = getDelegate;
    }
    
    public DelegatePair(string path, RequestType type, MiniHttpServer.ProcessPost postDelegate)
    {
        this.path = path;
        this.type = type;
        this.postDelegate = postDelegate;
    }
}
