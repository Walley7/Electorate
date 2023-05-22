using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ElectorateWeb.Filter
{
  public class CustomExceptionAttribute : Attribute, IExceptionFilter
  {
    public void OnException(ExceptionContext context)
    {
      HttpStatusCode status = HttpStatusCode.InternalServerError;
      String message = String.Empty;

      var exceptionType = context.Exception.GetType();
      if (exceptionType == typeof(UnauthorizedAccessException))
      {
        message = "Unauthorized Access";
        status = HttpStatusCode.Unauthorized;
      }
      else if (exceptionType == typeof(NotImplementedException))
      {
        message = "A server error occurred.";
        status = HttpStatusCode.NotImplemented;
      }     
      else
      {
        message = context.Exception.Message;
        status = HttpStatusCode.NotAcceptable;
      }
      context.ExceptionHandled = true;
      context.Result = new ObjectResult(new { Status = status, Success = false, Message = message });
      context.HttpContext.Response.StatusCode = 400;

    }
  }
}
