using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CoreMvcPoc.Web.Models;

using CoreMvcPoc.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Http.Extensions;
using ServiceManager;

namespace CoreMvcPoc.Web.Controllers
{
    [Authorize(Roles="User")]
    public class BooksController : Controller
    {
        private readonly WebServiceManager webServiceManager; 
        string userToken=string.Empty;
        private List<RequestHeaderItem> headers ;
        private IHostingEnvironment _appEnvironment;
        public BooksController(IHostingEnvironment appEnvironment)
        {
            webServiceManager = new WebServiceManager();
            _appEnvironment = appEnvironment;        
        }

        private string GetUserToken(ClaimsPrincipal user)
        {
            string returnVal = string.Empty;
            if(user.Claims.FirstOrDefault(x=>x.Type.Equals("UserToken"))!=null)
            {
                returnVal = user.Claims.FirstOrDefault(x=>x.Type.Equals("UserToken")).Value;
            }
            return returnVal;
        }

        public async Task<IActionResult> Index(string sortOrder,string currentFilter,string searchString, int page=1)
        {
            var folder = Path.Combine(_appEnvironment.WebRootPath, "uploads\\img");
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["PriceSortParm"] = sortOrder == "price" ? "price_desc" : "price";
            ViewData["AuthorSortParm"] = sortOrder == "author" ? "author_desc" : "author";
            if (searchString != null)
            {
               page = 1;
            }
            else
            {
               searchString = currentFilter;
            }
            ViewData["CurrentFilter"] = searchString;
            userToken = GetUserToken(HttpContext.User);    
            PagedList<Book> model = null;
            headers= new List<RequestHeaderItem>();
            headers.Add(new RequestHeaderItem{ HeaderItemType = HeaderItemType.Authorization, Key="Bearer", Value=userToken});
            var result = await webServiceManager.Get<PagedList<Book>>(string.Format("{0}{1}", Constant.BaseApiUrl,Constant.BooksUrl+"?sortby="+sortOrder+"&pagenumber="+page+"&pagesize=5&filter="+searchString), headers); 
            if(result.Success)
            {
                model = result.ResponseObject;
            }
            else 
            { 
               var authorizationCheck =  this.CheckAuthorization(result);
               if(authorizationCheck!=null)
               {
                   return authorizationCheck;
               }
            }
            
            if(model!=null)
            {
                return View(model);
            }
            else{
                return BadRequest("No data found");
            }
        }

        [ActionName("Edit")]      
        public async Task<IActionResult> EditBook(int id)
        {
            userToken = GetUserToken(HttpContext.User); 
            headers= new List<RequestHeaderItem>();
            headers.Add(new RequestHeaderItem{ HeaderItemType = HeaderItemType.Authorization, Key="Bearer", Value=userToken});   
            var result = await  webServiceManager.Get<Book>(string.Format("{0}{1}{2}", Constant.BaseApiUrl,Constant.BooksUrl+"/",Convert.ToString(id)), headers); 
            if(!result.Success)
            {
                var authorizationCheck =  this.CheckAuthorization(result);
               if(authorizationCheck!=null)
               {
                   return authorizationCheck;
               }
                
            }
            else if( result.ResponseObject==null)
            { 
               return RedirectToAction("Index");
            }
            return View(result.ResponseObject);           
        }

        [ActionName("Edit")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> EditBook(Book model)
        {
            if(!ModelState.IsValid)
            {
              return View("Edit", model);
            }
            userToken = GetUserToken(HttpContext.User);    
            headers= new List<RequestHeaderItem>();
            headers.Add(new RequestHeaderItem{ HeaderItemType = HeaderItemType.Authorization, Key="Bearer", Value=userToken});   
            bool isNew = model.Id==0;
            var result = await  webServiceManager.Post<SignupResponse, Book>(string.Format("{0}{1}", Constant.BaseApiUrl,Constant.BooksUrl), model, headers); 
            if(result.Success && result.ResponseObject!=null && result.ResponseObject.SuccessMessage.ToLower().Contains(isNew?"added":"updated"))
            {
                TempData["Success"] =isNew?"Book Added successfully": "Book updated successfully";
                return RedirectToAction("Index");
            }
            else 
            { 
               var authorizationCheck =  this.CheckAuthorization(result);
               if(authorizationCheck!=null)
               {
                   return authorizationCheck;
               }
            }
            ViewBag.Error =isNew?"Error in adding book": "Error in updating book";
            return View(isNew?"AddNew":"Edit", model);           
        }

        [ActionName("Details")]
        public async Task<IActionResult> BookDetails(int id)
        {
            userToken = GetUserToken(HttpContext.User); 
            headers= new List<RequestHeaderItem>();
            headers.Add(new RequestHeaderItem{ HeaderItemType = HeaderItemType.Authorization, Key="Bearer", Value=userToken});   
            var result = await  webServiceManager.Get<Book>(string.Format("{0}{1}{2}", Constant.BaseApiUrl,Constant.BooksUrl+"/",Convert.ToString(id)), headers); 
            if(!result.Success)
            {
               var authorizationCheck =  this.CheckAuthorization(result);
               if(authorizationCheck!=null)
               {
                   return authorizationCheck;
               }
               
            }  
            else if(result.ResponseObject==null)
            {
                 return RedirectToAction("Index");
            }          
            return View(result.ResponseObject);           
        }

        [ActionName("Delete")]
        [HttpPost]
        public async Task<ActionResult> DeleteBook(int id)
        {
            userToken = GetUserToken(HttpContext.User); 
            headers= new List<RequestHeaderItem>();
            headers.Add(new RequestHeaderItem{ HeaderItemType = HeaderItemType.Authorization, Key="Bearer", Value=userToken});   
            var result = await  webServiceManager.Delete(string.Format("{0}{1}", Constant.BaseApiUrl,Constant.BooksUrl+"?id="+id), headers); 
            if(result.Success && result.ResponseObject)
            {
                return Json(new {success=true, message="Book deleted successfully"});                     
            }
            else
            {          
               var authorizationCheck =  this.CheckAuthorization(result);
               if(authorizationCheck!=null)
               {
                  return Json(new {success=false, message="unauthorized"});
               }
               return Json(new {success=false, message="Error in deleting book"});                              
            }
        }
        [ActionName("UploadFile")]
        [HttpPost]
        public async Task<JsonResult> UploadFile(string image, IList<IFormFile> files)
        {
            IFormFile file=null;
            bool fileresult =true;
            string filename=string.Empty;
            string uploads=string.Empty;
            if(!string.IsNullOrEmpty(image)&& image != "undefined")
            {
                image = image.Replace(image.Substring(0,image.LastIndexOf('/')+1), "");
            }
            else
            {
                image =string.Empty;
            }
            if(files==null || files.Count==0)
            {
                fileresult=false;                    
            }
            else{
                file = files.FirstOrDefault();
            }
            if(file!=null || fileresult)
            {
                uploads = Path.Combine(_appEnvironment.WebRootPath, "uploads\\img");
                System.IO.Directory.CreateDirectory(uploads);
                if(System.IO.File.Exists(Path.Combine(uploads, image)))
                {
                    System.IO.File.Delete(Path.Combine(uploads, image));
                }
                if (file.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(file.FileName);
                    using (var fileStream = new FileStream(Path.Combine(uploads, fileName), FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                        filename = fileName;
                    }

                }
                else{
                     fileresult=false;   
                }                
            }
            else{ 
                fileresult=false;
            }
            //There is an error here
            
            if(!fileresult)
            {
                return Json(new {success=false, message="Error in upload"});    
            } 
            
            return Json(new {success=true, message= filename});                     
        }
        [HttpPost]
        public JsonResult TestBook(int id)
        {
            return Json(true);     
            
        }
        [ActionName("AddNew")]
        public IActionResult Add()
        {
            return View(new Book());           
        }       
        
    }
}