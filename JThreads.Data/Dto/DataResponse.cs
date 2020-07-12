using System;
using System.Collections.Generic;
using System.Text;

namespace JThreads.Data.Dto
{
    public class DataResponse<T> : Response
    {
        public T Data { get; set; }

        public DataResponse<T> WithData(T data)
        {
            this.Data = data;
            this.Success = true;
            return this;
        }
    }
}
