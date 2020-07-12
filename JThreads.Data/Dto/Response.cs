using System;
using System.Collections.Generic;
using System.Text;

namespace JThreads.Data.Dto
{
    public class Response
    {
        public bool Success { get; set; }
        public Dictionary<string, List<string>> Errors { get; set; }

        public Response()
        {
            Errors = new Dictionary<string, List<string>>();
        }

        public void AddError(string key, string msg)
        {
            if (Errors.ContainsKey(key))
            {
                Errors[key].Add(msg);
            }
            else
            {
                Errors[key] = new List<string>
                {
                    msg
                };
            }
        }

        public Response WithSuccess()
        {
            Success = true;
            return this;
        }

        public Response WithError(string key, string msg)
        {
            AddError(key, msg);
            return this;
        }

        public class Error
        {
            public string Key { get; set; }
            public string Msg { get; set; }
        }

        public Response WithErrors(ICollection<Error> errors)
        {
            foreach (var error in errors)
            {
                AddError(error.Key, error.Msg);
            }

            return this;
        }
    }
}
