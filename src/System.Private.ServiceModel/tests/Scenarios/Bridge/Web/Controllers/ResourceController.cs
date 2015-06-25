using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using Web.Models.Data;

namespace Web.Controllers
{
    public class ResourceController : ApiController
    {
        /// <summary>
        /// Initialize a test
        /// </summary>
        /// <param name="resource"></param>
        /// <response code="200">Test initialized.</response>
        public HttpResponseMessage PUT(resource resource)
        {
            if (resource == null) {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "resource data { name:'...' } not specified.");
            }

            var correlationId = Guid.NewGuid();
            var json = JsonConvert.SerializeObject(resource, Formatting.Indented);
            
            try
            {         
                Debug.WriteLine("Received request to create resource \n" + json);

                var result = ResourceInvoker.DynamicInvoke(resource);

                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    id = correlationId,
                    details = result
                });
            }
            catch (Exception exception)
            {
                Debug.WriteLine("Exception when trying to create resource : " + resource.name + " : " + exception.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new
                {
                    id = correlationId,
                    error = exception,
                });
            }
        }
    }
}