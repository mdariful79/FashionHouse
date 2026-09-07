using Microsoft.AspNetCore.Mvc;

namespace MaleFashion.Controllers
{
    public class BlogController : Controller
    {
        // GET: /Blog
        public IActionResult Index()
        {
            return View();
        }

        // GET: /Blog/Details/3
        // TODO: replace with a real lookup, e.g.:
        //   var post = _blogRepository.GetById(id);
        //   if (post == null) return NotFound();
        [Route("Blog/Details/{id?}")]
        public IActionResult Details(int? id)
        {
            return View();
        }
    }
}