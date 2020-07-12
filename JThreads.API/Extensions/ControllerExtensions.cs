using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JThreads.Data.Dto;
using Microsoft.AspNetCore.Mvc;

namespace JThreads.API.Extensions
{
    public static class ControllerExtensions
    {
        public static IActionResult GenerateResponse(this ControllerBase controllerBase, Response response)
        {
            if (response == null)
            {
                return controllerBase.NoContent();
            }
            if (response.Success)
            {
                return controllerBase.Ok(response);
            }
            return controllerBase.BadRequest(response);
        }
    }
}
