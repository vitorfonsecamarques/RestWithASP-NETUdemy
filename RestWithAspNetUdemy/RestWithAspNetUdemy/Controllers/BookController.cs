using Microsoft.AspNetCore.Mvc;
using RestWithAspNetUdemy.Business;
using RestWithAspNetUdemy.Data.VO;
using RestWithAspNetUdemy.Hypermedia.Filters;

namespace RestWithAspNetUdemy.Controllers
{
    [ApiVersion("1")]
    [ApiController]
    [Route("api/[controller]/v{version:apiVersion}")]
    public class BookController : ControllerBase
    {
        private readonly ILogger<BookController> _logger;
        private IBookBusiness _BookBusiness;

        public BookController(ILogger<BookController> logger, IBookBusiness BookBusiness)
        {
            _logger = logger;
            _BookBusiness = BookBusiness;
        }

        [HttpGet]
        [TypeFilter(typeof(HyperMediaFilter))]
        public IActionResult Get() 
        {
            return Ok(_BookBusiness.FindAll());
        }

        [HttpGet("{id}")]
        [TypeFilter(typeof(HyperMediaFilter))]
        public IActionResult Get(long id)
        {
            var book = _BookBusiness.FindByID(id);
            if (book == null) return NotFound();
            return Ok(book);
        }

        [HttpPost]
        [TypeFilter(typeof(HyperMediaFilter))]
        public IActionResult Post([FromBody] BookVO book)
        {
            if (book == null) return BadRequest();
            return Ok(_BookBusiness.Create(book));
        }

        [HttpPut]
        [TypeFilter(typeof(HyperMediaFilter))]
        public IActionResult Update([FromBody] BookVO book)
        {
            if (book == null) return BadRequest();
            return Ok(_BookBusiness.Update(book));
        }

        [HttpDelete("{id}")]
        public IActionResult Post(long id)
        {
            _BookBusiness.Delete(id);
            return NoContent();
        }
    }
}