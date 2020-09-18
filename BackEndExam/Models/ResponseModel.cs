using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEndExam.Models
{
    public class ResponseModel
    {
        public DateTime DateTime { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public JObject Data { get; set; }
    }
}
