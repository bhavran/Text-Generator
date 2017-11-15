using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using System.Runtime.Caching;

using TextGenerator;

namespace TextGenerator.WebApp.Controllers
{
    public class GeneratorController : ApiController
    {
        private MemoryCache cache = MemoryCache.Default;

        public string Get(string id)
        {
            if (cache.Contains(id))
                return (cache[id] as ITextGenerator).Generate();
            else
                return $"Generator Not Found for Key {id}";
        }

        public string Post([FromBody]Models.CreateGeneratorRequest value)
        {
            string id = Guid.NewGuid().ToString();
            cache.Add(id,GeneratorFactory.BuildGenerator(value), DateTime.Now.AddMinutes(30));
            return id;
        }

        // DELETE: api/Generator/5
        public void Delete(string id)
        {
            if (cache.Contains(id))
                cache.Remove(id);
        }
    }
}
