using JThreads.Data.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace JThreads.Application.Services
{
    public class BaseService
    {
        public Response Respond()
        {
            return new Response();
        }

        public DataResponse<object> RespondWithData(object o)
        {
            return new DataResponse<object>().WithData(o);
        }
    }
}
