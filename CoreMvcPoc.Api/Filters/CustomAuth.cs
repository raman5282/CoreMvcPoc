using System;
using System.Linq;
using System.Net;
using System.Security.Claims;
using CoreMvcPoc.BLL;
using CoreMvcPoc.DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CoreMvcPoc.Api
{
    public class CustomAuth :  AuthorizeAttribute, IAuthorizationFilter
    {   
        private readonly IUserService service;
        public CustomAuth()
        {
            
        } 
        public CustomAuth(IUserService service)
        {
            this.service = service;
        }       

        public void OnAuthorization(AuthorizationFilterContext context)
        {
             var val = context.HttpContext.Request.Headers.Where(x => x.Key == "Authorization").FirstOrDefault();
             if(val.Value.Count>0)
             {
                var token = val.Value[0].Replace("Bearer ","");
                var userIdClaim = context.HttpContext.User.Claims.FirstOrDefault(c=>c.Type == "Id"); 
                if(!UserTokenIsValid(userIdClaim, token))
                {
                    context.Result = new Http403Result(context.Result);
                }        
             }
        }

        private bool UserTokenIsValid(Claim claim, string token)
        {
            bool returnVal=false;
            if(claim!=null)
            {
                returnVal = service.CheckUserToken(Convert.ToInt32(claim.Value), token);
            }
            return returnVal;
        }
    }
}