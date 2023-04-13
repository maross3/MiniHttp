using System.Net;
using System.Text;
using MiniHttp.Utils;

namespace MiniHttp.Server;

public class MiniHttpServer
{
    // request delegates
    public delegate Task<string> ProcessGet(string path);
    public delegate Task<string> ProcessPost(string path, string body);

    // server state
    private bool _connected;
    
    /// <summary>
    /// Indicates the servers connection state.
    /// </summary>
    public bool isServerConnected => _connected;
    
    // server components
    private readonly HttpListener _listener;
    private readonly Dictionary<string, ProcessPost> _postRequestLookup = new();
    private readonly Dictionary<string, ProcessGet> _getRequestLookup = new();

    public MiniHttpServer(string url)
    {
        _listener = new HttpListener();
        _listener.Prefixes.Add(url);
    }
    
    /// <summary>
    /// Maps a post request to a delegate.
    /// </summary>
    /// <param name="path">The path of the post request.</param>
    /// <param name="handler">The delegate to invoke on request.</param>
    public void MapPostRequest(string path, ProcessPost handler) =>
        _postRequestLookup.Add(path, handler);
    
    /// <summary>
    /// Maps a get request to a delegate.
    /// </summary>
    /// <param name="path">The path of the get request.</param>
    /// <param name="handler">The delegate to invoke on request.</param>
    public void MapGetRequest(string path, ProcessGet handler) =>
        _getRequestLookup.Add(path, handler);

    /// <summary>
    /// Handles the post request.
    /// </summary>
    /// <param name="request">A post request.</param>
    /// <returns>The body of the post request.</returns>
    private async Task<string> HandlePost(HttpListenerRequest request)
    {
       if(PostPathOrError(request, out var path)) return path; 
        using var reader = new StreamReader(request.InputStream, request.ContentEncoding);
        
        var requestBody = await reader.ReadToEndAsync();
        var response = await _postRequestLookup[path].Invoke(path, requestBody);
        
        NrLogger.Log(requestBody);
        return response;
    }

    /// <summary>
    /// Handles the get request.
    /// </summary>
    /// <param name="request">A get request.</param>
    /// <returns>The body of the get request.</returns>
    private async Task<string> HandleGet(HttpListenerRequest request)
    {
        if (GetPathOrError(request, out var path)) return path;
        return await _getRequestLookup[path].Invoke(path);
    }

    /// <summary>
    /// Starts the http server.
    /// </summary>
    public async Task Start()
    {
        NrLogger.Log("starting server");
        _listener.Start();
        _connected = true;
        while (_connected)
        {
            var context = await _listener.GetContextAsync();
            var request = context.Request;
            var responseString = "Unrecognized request";
            if (request.HttpMethod == "GET") responseString = await HandleGet(context.Request);
            if (request.HttpMethod == "POST") responseString = await HandlePost(context.Request);

            await Respond(responseString, context);
        }
    }

    /// <summary>
    /// Builds the response and closes the OutputStream.
    /// </summary>
    /// <param name="responseString">The response handled by the app.</param>
    /// <param name="context">The listener's context.</param>
    private static async Task Respond(string responseString, HttpListenerContext context)
    {
        var responseBytes = Encoding.UTF8.GetBytes(responseString);
        context.Response.ContentLength64 = responseBytes.Length;
        await context.Response.OutputStream.WriteAsync(responseBytes, 0, responseBytes.Length);
        context.Response.OutputStream.Close();
    }

    /// <summary>
    /// Stop the server and update it's state.
    /// </summary>
    public void Stop()
    {
        _connected = false;
        _listener.Stop();
        _listener.Close();
    }

    // todo, use request to get request type, and then use a dictionary to get the delegate
    // delete one of these methods.
    /// <summary>
    /// Gets the path from the post request and checks if it is valid.
    /// </summary>
    /// <param name="request">The request to validate.</param>
    /// <param name="path">The path or an error message.</param>
    /// <returns>The path exists, or an error message.</returns>
    private bool PostPathOrError(HttpListenerRequest request, out string path)
    {
        path = "Unrecognized request";
        if (request.Url == null || !_postRequestLookup.ContainsKey(request.Url.AbsolutePath)) 
            return true;
        path = request.Url.AbsolutePath;
        return false;
    }
    
    /// <summary>
    /// Gets the path from the get request and checks if it is valid.
    /// </summary>
    /// <param name="request">The request to validate.</param>
    /// <param name="path">The path or an error message.</param>
    /// <returns>The path exists, or an error message.</returns> 
    private bool GetPathOrError(HttpListenerRequest request, out string path)
    {
        path = "Unrecognized request";
        if (request.Url == null || !_getRequestLookup.ContainsKey(request.Url.AbsolutePath)) 
            return true;
        path = request.Url.AbsolutePath;
        return false;
    }
}