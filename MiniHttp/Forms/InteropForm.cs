using MiniHttp.Mediators;
using MiniHttp.Server;

namespace MiniHttp.Forms;

public partial class InteropForm : Form
{
    private readonly MiniHttpServer _server;
    public InteropForm()
    {
        InitializeComponent();
        DelegateMediator.mainForm = this;
        _server = ServerBuilder.BuildServer();
        
#pragma warning disable CS4014
        _server.Start();
#pragma warning restore CS4014
        
        Closing += (sender, args) => CleanUp();
    }
    
    internal async Task<string> PostRequest(string path, string body) =>
        await Task.FromResult("Successfully posted");
    
    internal Task<string> TestGet(string path) =>
        Task.FromResult("Test get request processed successfully");
    private void CleanUp() =>
        _server.Stop();
}