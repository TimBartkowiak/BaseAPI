using BaseAPI.Entities;
using BaseAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace BaseAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : AbstractController<TestModel, TestEntity>
    {
        private readonly TestService _testService;
        
        public TestController(TestService testService)
        {
            _testService = testService;
        }

        protected override AbstractService<TestModel, TestEntity> getService()
        {
            return _testService;
        }

        // GET: api/Test/{id}
        [HttpGet("{id}", Name = "Get")]
        public TestModel Get(string id)
        {
            return getById(id);
        }

        // POST: api/Test
        [HttpPost]
        public IActionResult Post([FromBody] TestModel model)
        {
            return add(model);
        }

        // PUT: api/Test/{id}
        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody] TestModel model)
        {
            return update(model, id);
        }
    }
}
