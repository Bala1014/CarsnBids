using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.ContextProvider;
using SearchService.Models;

namespace SearchService.Controllers
{
    [ApiController]
    [Route("api/Search")]
    public class SearchController : ControllerBase
    {
        private readonly IConfiguration _config;
        public SearchController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet()]
        public async Task<ActionResult<Item>> SearchItems(string SearchTerms)
        {
            var client = new MongoClient(MongoClientSettings.FromConnectionString(_config.GetConnectionString("MongoDBConnectionString")));
            //var dbContext = new searchDbContext();
            var db = searchDbContext.Create(client.GetDatabase("SearchDB"));
            var item = db.Items.OrderBy(i => i.Make).FirstOrDefault(k => k.Make == SearchTerms);
            return Ok(item);
        } 


        //{
        //    var query = DB.Find<Item>();

        //    query.Sort(x => x.Ascending(a => a.Make));

        //    query.Match(Search.Full, searchTerm).SortByTextScore();

        //    var result = await Queryable.ExecuteAsync();

        //    return result;

        //}

    }
}
