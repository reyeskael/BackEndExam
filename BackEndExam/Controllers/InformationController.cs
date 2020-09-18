using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BackEndExam.Classes;
using BackEndExam.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BackEndExam.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InformationController : ControllerBase
    {
        private const string TABLENAME = "tinformation";

        private ResponseModel CheckModel(InformationModel model)
        {
            if (model.skillrate > 10)
                return API.ParseReturn(API.Status.ERROR, "skillrate cannot be greater than 10.", null);
            if (model.skillrate < 0)
                return API.ParseReturn(API.Status.ERROR, "skillrate cannot be lower than 0.", null);
            return API.ParseReturn(API.Status.DONE, "success", null);
        }

        private string GetInformation(string mode = "ASC")
        {
            DatabaseConnection databaseConnection = DatabaseConnection.GetInstance;

            JObject order_by = new JObject();
            order_by.Add("column", "skillrate");
            order_by.Add("mode", mode);
            JObject databaseReturn = databaseConnection.SelectData(TABLENAME, 0, "id", order_by);
            API.Status status = (bool)databaseReturn["isError"] ? API.Status.ERROR : API.Status.DONE;
            string message = (bool)databaseReturn["isError"] ? databaseReturn["message"].ToString() : "success";

            ResponseModel responsemodel = API.ParseReturn(status, message, (JObject)databaseReturn["data"]);
            JObject reponse = JObject.FromObject(responsemodel);
            return reponse.ToString();
        }

        // GET: api/<InformationController>
        [HttpGet]
        public string Get()
        {
            return GetInformation();
        }

        // GET api/<InformationController>/5
        [HttpGet("{mode}")]
        public string Get(string mode)
        {
            ResponseModel responsemodel;
            JObject reponse;
            if (mode.ToLower() != "asc" && mode.ToLower() != "desc") {
                responsemodel = API.ParseReturn(API.Status.ERROR, "invalid sorting mode.", null);
                reponse = JObject.FromObject(responsemodel);
                return reponse.ToString();
            }
            return GetInformation(mode.ToUpper());
        }

        // POST api/<InformationController>
        [HttpPost]
        public string Post([FromBody] string value)
        {
            JObject reponse;
            InformationModel model = JsonConvert.DeserializeObject<InformationModel>(value);
            if (CheckModel(model).Status == API.Status.ERROR.ToString())
            {
                reponse = JObject.FromObject(CheckModel(model));
                return reponse.ToString();
            }
            DatabaseConnection databaseConnection = DatabaseConnection.GetInstance;

            PropertyInfo[] propertyInfos =  model.GetType().GetProperties();
            string[] columnname = new string[propertyInfos.Length];
            int index = 0;
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                columnname[index] = propertyInfo.Name;
                index++;
            }

            List<object> data = new List<object>();
            data.Add(model);
            JObject databaseReturn = databaseConnection.InsertData(TABLENAME, columnname, data);
            API.Status status = (bool)databaseReturn["isError"] ? API.Status.ERROR : API.Status.DONE;
            string message = (bool)databaseReturn["isError"] ? databaseReturn["message"].ToString() : "success";

            ResponseModel responsemodel = API.ParseReturn(status, message, (JObject)databaseReturn["data"]);
            reponse = JObject.FromObject(responsemodel);
            return reponse.ToString();
        }

        // PUT api/<InformationController>/5
        [HttpPut("{id}")]
        public string Put(int id, [FromBody] string value)
        {
            JObject reponse;
            InformationModel model = JsonConvert.DeserializeObject<InformationModel>(value);
            if (CheckModel(model).Status == API.Status.ERROR.ToString())
            {
                reponse = JObject.FromObject(CheckModel(model));
                return reponse.ToString();
            }
            DatabaseConnection databaseConnection = DatabaseConnection.GetInstance;

            PropertyInfo[] propertyInfos = model.GetType().GetProperties();
            string[] columnname = new string[propertyInfos.Length];
            int index = 0;
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                columnname[index] = propertyInfo.Name;
                index++;
            }

            JObject databaseReturn = databaseConnection.UpdateData(TABLENAME, id, JObject.FromObject(model));
            API.Status status = (bool)databaseReturn["isError"] ? API.Status.ERROR : API.Status.DONE;
            string message = (bool)databaseReturn["isError"] ? databaseReturn["message"].ToString() : "success";

            ResponseModel responsemodel = API.ParseReturn(status, message, (JObject)databaseReturn["data"]);
            reponse = JObject.FromObject(responsemodel);
            return reponse.ToString();
        }

        // DELETE api/<InformationController>/5
        [HttpDelete("{id}")]
        public string Delete(int id)
        {
            DatabaseConnection databaseConnection = DatabaseConnection.GetInstance;

            JObject databaseReturn = databaseConnection.DeleteData(TABLENAME, id);
            API.Status status = (bool)databaseReturn["isError"] ? API.Status.ERROR : API.Status.DONE;
            string message = (bool)databaseReturn["isError"] ? databaseReturn["message"].ToString() : "success";

            ResponseModel responsemodel = API.ParseReturn(status, message, (JObject)databaseReturn["data"]);
            JObject reponse = JObject.FromObject(responsemodel);
            return reponse.ToString();
        }
    }
}
