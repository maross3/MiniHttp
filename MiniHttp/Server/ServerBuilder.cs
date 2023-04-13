using MiniHttp.Global;
using MiniHttp.Mediators;

namespace MiniHttp.Server;

/// <summary>
/// Build and initialize an http server with mapped endpoints.
/// </summary>
public static class ServerBuilder
{
    private const string _SERVER_URL = "http://localhost:8080/";
    
    /// <summary>
    /// Mapped delegate pairs to build the server with
    /// </summary>
    private static readonly List<DelegatePair> _SMappedDelegatePairs = new();
    
    /// <summary>
    /// Builds an http server with the mapped delegates built in <see cref="BuildRequests"/>.
    /// </summary>
    /// <returns>The newly created http server with mapped endpoints.</returns>
    public static MiniHttpServer BuildServer()
    {
        var server = new MiniHttpServer(_SERVER_URL);
        BuildRequests();
        
        foreach (var del in _SMappedDelegatePairs)
        {
            if (del.getDelegate == null && del.postDelegate == null)
                throw new InvalidOperationException("Failed to build server due to a null delegate");
                
            if (del.type == RequestType.Get) server.MapGetRequest(del.path, del.getDelegate);
            else if (del.type == RequestType.Post) server.MapPostRequest(del.path, del.postDelegate);
        }
        
        return server;
    }
    
    /// <summary>
    /// Builds the mapped delegate pairs for the server to use. Whenever a new endpoint is added, it should be added here
    /// from the <see cref="DelegateMediator"/> class.
    /// </summary>
    /// <TODO>Make this more dynamic by creating an attribute class. append them to the mediator</TODO>
    private static void BuildRequests()
    {
        _SMappedDelegatePairs.Add(
            new DelegatePair("/test", RequestType.Get, DelegateMediator.GetRequest()));
        _SMappedDelegatePairs.Add(
            new DelegatePair("/test", RequestType.Post, DelegateMediator.PostRequest()));
    }
}