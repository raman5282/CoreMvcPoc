using CoreMvcPoc.BLL;
using CoreMvcPoc.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CoreMvcPoc.Api
{
    [Route("api/[controller]")]    
    [ServiceFilter(typeof(CustomAuth))]   
   
    public class BooksController : Controller
    {
        private readonly IBookService bookService;
        public readonly IUserService userService;
        public BooksController(IBookService service, IUserService userService)
        {
          this.bookService = service;  
          this.userService = userService;
        }
        // GET api/values
        [HttpGet]       
        public IActionResult Get(string sortby, int pagenumber, int pagesize, string filter)
        {
            var user = User.FindFirst("Id");
            int userId = user!=null ? Convert.ToInt32(user.Value):0;
            var pagingParams = new PagingParams{PageNumber = pagenumber, PageSize = pagesize};
            var books = bookService.GetUserBooks(userId, sortby, pagingParams, filter);
            if(books!=null)
            {
                return new OkObjectResult(books);
            }
            return BadRequest("Error in finding books");
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var book = bookService.GetBook(id);
            if(book!=null)
            {
                return new OkObjectResult(book);
            }
            return BadRequest("Error in finding book");
        }

        // POST api/values
        [HttpPost]
        public IActionResult Post([FromBody]Book model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = User.FindFirst("Id");
            int userId = user!=null ? Convert.ToInt32(user.Value):0;
            model.UserId = userId;
            bool isUpdate =  model.Id>0 ;
            bool requestStatus =  isUpdate ? bookService.UpdateUserBook(model) : bookService.AddUserBook(model);

            if(requestStatus)
            {
                return new JsonResult( isUpdate ? "Book Updated" : "Book Added");
            }
            
            return new BadRequestObjectResult("Error in adding book");            
        }

        // DELETE api/values/5
        [HttpDelete]
        public bool Delete([FromQuery]int id)
        {
            return bookService.DeleteUserBook(id);            
        }
    }
}
