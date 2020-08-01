using System.Threading.Tasks;

namespace Scrapper.Scrappers
{
    public interface IScrapper
    {
        Task<Ranking> ScrapeAsync(string keyWords);
    }
}
