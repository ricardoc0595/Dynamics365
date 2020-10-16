//using Logic.CrmAboxApi.Classes.Helper;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Web.Http;

//namespace CrmAboxApi.Controllers
//{
//    public class ValuesController : ApiController
//    {
//        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

//        /// //////////////////////////

//        // GET api/values
//        public IEnumerable<string> Get()
//        {
//            return new string[] { "value1", "value2" };
//        }

//        // GET api/values/5
//        public IHttpActionResult Get(int id)
//        {
//            Class1 c = new Class1();
//            ////c.connect();
//            //c.whoAmIFunction();
//            //return null;

//            string json = "{'patientid':null,'country':'CR','userType':'01','personalinfo':{'idtype':'02','id':'testWebAPI02','name':'Ricardo','lastname':'Carrillo','secondlastname':'Cubillo','gender':'Femenino','dateofbirth':'1938 - 09 - 13','password':'Password.01'},'contactinfo':{'province':'5','canton':'511','district':'51101','phone':'22222222','mobilephone':'11111111','address':'','email':'testWebAPI02@aboxplan.com','password':'Password.01'},'patientincharge':{'idtype':'Extranjero','id':'testWebAPI02','name':'Ricardo','lastname':'Carrillo','secondlastname':'Cubillo','gender':'Femenino','dateofbirth':'1938 - 09 - 13'},'medication':{'products':[{'productid':'00S167539140140-AP','frequency':'1 al día','other':''},{'productid':'100T119002G02150-AP','frequency':'1 al día','other':''}],'medics':[{'medicid':'3880'}]},'interests':[{'interestid':'05','relations':[{'relation':{'relationid':'Para mí','other':''}}]}],'otherInterest':''}";

//            c.CreateContact(json);
//            //c.whoAmIFunction();
//            ServiceResponse response = new ServiceResponse();

//            try
//            {
//                response.Message = "mensaje";
//                //Logger.Info("mensaje enviado correctamente");
//                //Logger.Trace("mensaje enviado correctamente");
//                Logger.Error("Error enviado");
//            }
//            catch (Exception ex)
//            {
//            }

//            return Ok(response);

//        }

//        // POST api/values
//        public void Post([FromBody]string value)
//        {
//        }

//        // PUT api/values/5
//        public void Put(int id, [FromBody]string value)
//        {
//        }

//        // DELETE api/values/5
//        public void Delete(int id)
//        {
//        }
//    }
//}