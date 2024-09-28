using AngleSharp.Dom;
using AngleSharp.Io;

namespace EAScraperJob.Interfaces
{
    public interface IAngleSharpWrapper     
    {
        public Task<IDocument> GetSearchResults(string url, IRequester? request = null);

        public Task<IDocument> OpenAsync(string url, RequesterWrapper requester);
    }
}
