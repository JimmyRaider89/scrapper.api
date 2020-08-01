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

            var requestUrl = string.Format(_config.Url, keyWords, _config.Count);
            var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            // Google allows the return of 100+ records
            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var html = response.Content.ReadAsStringAsync().Result;
                //await System.IO.File.ReadAllTextAsync("c:\\temp\\google.html");

                /*
                 A SEACH RESULT record in GOOGLE looks like below.  This will ignore any ADs and "People Also Ask" sections

                 <div class="g">
                   <!--m-->
                   <div class="rc" data-hveid="CDQQAA" data-ved="2ahUKEwjRwIrA8vnqAhXWWisKHazbBFYQFSgAMAV6BAg0EAA">
                      <div class="r">
                         <a href="https://www.nswlrs.com.au/Access-Titling-Information" ping="/url?sa=t&amp;source=web&amp;rct=j&amp;url=https://www.nswlrs.com.au/Access-Titling-Information&amp;ved=2ahUKEwjRwIrA8vnqAhXWWisKHazbBFYQFjAFegQINBAB">
                            <br>
                            <h3 class="LC20lb DKV0Md">Access Titling Information - NSW Land Registry Services</h3>
                            <div class="TbwUpd NJjxre"><cite class="iUh30 bc tjvcx">www.nswlrs.com.au<span class="eipWBe"> › Access-Titling-Information</span></cite></div>
                         </a>
                        ... [TRIMMED FOR BREVITY]
              */

                var openingDivIndex = 0;
                do
                {
                    openingDivIndex = html.IndexOf("<div class=\"r\">", openingDivIndex);
                    if (openingDivIndex != -1)
                    {
                        // find the first anchor tag
                        var anchorStartIndex = html.IndexOf("<a href=\"", openingDivIndex);
                        var anchorClosingIndex = html.IndexOf(">", anchorStartIndex);

                        var block = html.Substring(anchorStartIndex, anchorClosingIndex - anchorStartIndex);
                        var result = RegExHelpers.FindUrl(block);
                        if (result != null)
                        {
                            urls.Add(result);
                        }
                        openingDivIndex++;
                    }
                } while (openingDivIndex != -1);
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
                Postiions = postiions.ToArray(),
                SearchEngine = SearchEngine.Google,
                KeyWords = keyWords
            };
        }

    }
}
