using Scrapper.Helper;
using System;

namespace Scrapper.Scrappers
{
    public class ScrapperFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public ScrapperFactory(IServiceProvider serviceProvider)
        {
            this._serviceProvider = serviceProvider;
        }

        public IScrapper GetScraper(SearchEngine searchEngine)
        {
            if(searchEngine == SearchEngine.Google)
            {
                return (IScrapper)_serviceProvider.GetService(typeof(Google));
            }

            if (searchEngine == SearchEngine.Bing)
            {
                return (IScrapper)_serviceProvider.GetService(typeof(Bing));
            }

            throw new ArgumentOutOfRangeException("search engine not supported");
        }
    }
}
