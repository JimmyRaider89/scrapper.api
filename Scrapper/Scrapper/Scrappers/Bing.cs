using Microsoft.Extensions.Options;
using Scrapper.Helper;
using Scrapper.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Scrapper.Scrappers
{
    public class Bing : IScrapper
    {
        private readonly HttpClient _httpClient;
        private readonly SearchEngineConfig _config;

        public Bing(IHttpClientFactory clientFactory, IOptionsSnapshot<SearchEngineConfig> options)
        {
            _httpClient = clientFactory.CreateClient();
            _httpClient.DefaultRequestHeaders.Add("Accept", "*/*");
            _config = options.Get(nameof(Bing));
        }

        public async Task<Ranking> ScrapeAsync(string keyWords)
        {
            var urls = new List<string>();

            do
            {
                var requestUrl = string.Format(_config.Url, keyWords, _config.RecordsPerRequest, urls.Count);
                var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var html = response.Content.ReadAsStringAsync().Result;

                    //strip all the junk so we're just left with the <main> search result tag and it's contents
                    // if not continue on but by doing this we reduce the string size
                    var mainTagIndex = html.IndexOf("<main");

                    if (mainTagIndex != -1)
                    {
                        html = html.Substring(mainTagIndex, html.IndexOf("</main>") - mainTagIndex);
                    }

                    /*
                     A SEARCH RESULT record in BING looks like below.  This will ignore any ADs and "People Ask Block" sections
                     Ideally if I had the time I would of used a html parser.

                    <li class="b_algo" data-bm="13">
                       <h2><a href="https://infotrackgo.com.au/property" h="ID=SERP,5202.1">Property Search | InfoTrackgo</a></h2>
                       <div class="b_caption">
                          <div class="b_attribution" u="3|5069|4611686811764292|s0WH2Zwebs0xJyvjLHTVJDElmUxQWCzI">
                            <cite>infotrackgo.com.au/property</cite>
                            <a href="#" class="trgr_icon" aria-label="Actions for this site" aria-haspopup="true" aria-expanded="false" role="button">
                                <span class="c_tlbxTrg">
                                    <span class="c_tlbxTrgIcn sw_ddgn"></span>
                                    <span class="c_tlbxH" h="BASE:CACHEDPAGEDEFAULT" k="SERP,5203.1"></span>
                                   </span>
                            </a>
                          </div>
                          <p>InfoTrackgo allows you to search by address for NSW, QLD, VIC, ACT, NT, SA, TAS and WA. InfoTrackgo also offers direct title reference, volume/folio or plan number searching nationally across Australia. You'll have the option to order any of the documents related to that property.</p>
                       </div>
                    </li>
                    */

                    var openingLiIndex = 0;
                    do
                    {
                        openingLiIndex = html.IndexOf("<li class=\"b_algo\"", openingLiIndex);
                        if (openingLiIndex != -1)
                        {
                            var closingIndex = html.IndexOf("</li>", openingLiIndex);
                            var block = html.Substring(openingLiIndex, closingIndex - openingLiIndex);
                            var result = RegExHelpers.FindUrl(block);
                            if (result != null)
                            {
                                urls.Add(result);
                            }
                            openingLiIndex++;
                        }
                        if(urls.Count >= _config.MaxMatchesPerRequest)
                        {
                            break;
                        }
                    } while (openingLiIndex != -1);
                }
                else
                {
                    // let this bubble up and a return a 500
                    throw new Exception($"The call to {requestUrl} returned status code {response.StatusCode}");
                }
                // if no urls have been added then it means the algorithim is now out of date
                // we need to exit this loop otherwise it will continue forever.
            } while (urls.Count != 0 && urls.Count < _config.MaxMatchesPerRequest);

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
                SearchEngine = SearchEngine.Bing,
                KeyWords = keyWords
            };
        }
    }
}
