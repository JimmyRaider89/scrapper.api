using Microsoft.Extensions.Options;
using Scrapper.Helper;
using Scrapper.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Scrapper.Scrappers
{
    public class Google : IScrapper
    {
        private readonly HttpClient _httpClient;
        private readonly SearchEngineConfig _config;

        public Google(IHttpClientFactory clientFactory, IOptionsSnapshot<SearchEngineConfig> options)
        {
            _httpClient = clientFactory.CreateClient();
            _httpClient.DefaultRequestHeaders.Add("Accept", "*/*");
            _config = options.Get(nameof(Google));
        }

        public async Task<Ranking> ScrapeAsync(string keyWords)
        {
            var urls = new List<string>();

            // Google allows the return of 100 records in one call
            var requestUrl = string.Format(_config.Url, keyWords, _config.RecordsPerRequest);
            var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var html = response.Content.ReadAsStringAsync().Result;

                /*
                 A SEACH RESULT record in GOOGLE seems to take many forms

                - When you browse search results are wrapped in a <div class="g">
                - When calling via a non browser the same div looks like <div class="kCrYT"> 
                
                I suspect this divs id changes overtime to keep us busy - and to use the API so instead searching on /href="/url?q=

                <div class="kCrYT"><a
                    href="/url?q=https://www.landata.vic.gov.au/&amp;sa=U&amp;ved=2ahUKEwianbWRg_zqAhVVeX0KHbouBuoQFjAAegQIZRAB&amp;usg=AOvVaw249Kthl-7fQPbAlMh0Xeny">
                    <h3 class="zBAuLc">
                    <div class="BNeawe vvjwJb AP7Wnd">Landata</div>
                    </h3>
                    <div class="BNeawe UPmit AP7Wnd">https://www.landata.vic.gov.au</div>
                    </a>
                </div>
              */

                var anchorStartIndex = 0;
                do
                {
                    anchorStartIndex = html.IndexOf("<a href\x3d\x22/url?q\x3d", anchorStartIndex);
                    if (anchorStartIndex != -1)
                    {
                        var anchorClosingIndex = html.IndexOf(">", anchorStartIndex);

                        var block = html.Substring(anchorStartIndex, anchorClosingIndex - anchorStartIndex);
                        var result = RegExHelpers.FindUrl(block);
                        if (result != null)
                        {
                            urls.Add(result);
                        }
                        anchorStartIndex++;
                    }

                    if (urls.Count >= _config.MaxMatchesPerRequest)
                    {
                        break;
                    }
                } while (anchorStartIndex != -1);
            }
            else
            {
                // let this bubble up and a return a 500
                throw new Exception($"the call to {requestUrl} returned status code ${response.StatusCode}");
            }

            // Finally work out any matches
            List<int> postiions = new List<int>();
            for (int i = 0; i < urls.Count; i++)
            {
                foreach (var url in _config.MatchUrls)
                {
                    if (urls[i].Contains(url))
                    {
                        postiions.Add(i + 1);
                    }
                }
            }

            return new Ranking
            {
                Positions = postiions.ToArray(),
                SearchEngine = SearchEngine.Google,
                KeyWords = keyWords
            };
        }

    }
}
