using CrmAboxApi.Logic.Classes;
using CrmAboxApi.Logic.Classes.Deserializing;
using Logic.CrmAboxApi.Classes.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CrmAboxApi.Controllers
{
    
    public class ContactsController : ApiController
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        

        // GET api/contacts
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/contacts/5
        public IHttpActionResult Get(int id)
        {
            return Ok();
            
        }

        // POST api/contacts
        public IHttpActionResult Post([FromBody]PatientSignup json)
        {

            Contact contactProcedures = new Contact();
            ////c.connect();
            //c.whoAmIFunction();
            //return null;

            //"{'patientid':null,'country':'CR','userType':'01','personalinfo':{'idtype':'02','id':'testWebAPI02','name':'Ricardo','lastname':'Carrillo','secondlastname':'Cubillo','gender':'Femenino','dateofbirth':'1938 - 09 - 13','password':'Password.01'},'contactinfo':{'province':'5','canton':'511','district':'51101','phone':'22222222','mobilephone':'11111111','address':'','email':'testWebAPI02@aboxplan.com','password':'Password.01'},'patientincharge':{'idtype':'Extranjero','id':'testWebAPI02','name':'Ricardo','lastname':'Carrillo','secondlastname':'Cubillo','gender':'Femenino','dateofbirth':'1938 - 09 - 13'},'medication':{'products':[{'productid':'00S167539140140-AP','frequency':'1 al día','other':''},{'productid':'100T119002G02150-AP','frequency':'1 al día','other':''}],'medics':[{'medicid':'3880'}]},'interests':[{'interestid':'05','relations':[{'relation':{'relationid':'Para mí','other':''}}]}],'otherInterest':''}"

            
            //c.whoAmIFunction();
            ServiceResponse response = contactProcedures.Create(json);

            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            else
            {
                return Content(HttpStatusCode.InternalServerError,response);
            }
            

        }

        // PUT api/contacts/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/contacts/5
        public void Delete(int id)
        {
        }
    }
}
