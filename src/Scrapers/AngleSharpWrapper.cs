using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Io;
using EAScraperJob.Interfaces;

namespace EAScraperJob.Scrapers
{
    public class AngleSharpWrapper : IAngleSharpWrapper
    {
        // this is too specific, it needs to return the document to the ZooplaScraper, where he can get by 
        // classname
        // this means I can query different
        public async Task<IDocument> GetSearchResults(string url, IRequester? requester = null)
        {
            if (requester == null)
            {
                requester = new DefaultHttpRequester();
            }

            var config = Configuration.Default.WithDefaultLoader().With(requester);
            var context = BrowsingContext.New(config);

            return await context.OpenAsync(url);
        }

        public async Task<IDocument> OpenAsync(string url, RequesterWrapper requester)
        {
            var config = Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(config);

            return await context.OpenAsync(url);

            //return document.GetElementsByClassName("css-1anhqz4-ListingsContainer earci3d2");
        }
    }
}
