using BackEndExam.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEndExam.Classes
{
    public class API
    {
        public static ResponseModel ParseReturn(Status status, string message, JObject data)
        {
            ResponseModel response = new ResponseModel();
            response.DateTime = DateTime.UtcNow;
            response.Status = status.ToString();
            response.Message = message;
            response.Data = data;
            return response;
        }

        public enum Status
        {
            PROCESSING,
            DONE,
            ERROR
        }
    }
}
