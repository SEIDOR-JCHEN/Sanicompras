using Microsoft.AspNetCore.Mvc;
using System.Net;
using RestSharp;
using System.Net.Security;
using Sanicompras.Classes;
using Sanicompras.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;

namespace Sanicompras.Controllers
{
    public class ServiceLayer : ControllerBase
    {
        private IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        public HttpStatusCode CallServiceLayer(
            ref string responseContent,
            string SessionId,
            string service,
            Method method,
            string jsonBody,
            object key = null,
            string query = "",
            string action = ""
            )
        {

            RestClient restClient = null;
            RestRequest restRequest = null;
            IRestResponse response = null;
            string resource = string.Empty;

            string errmsg = string.Empty;
            clsLog clog = new clsLog();


            try
            {

                //errmsg += " Functon: CallServiceLayer " + "\r\n";
                //errmsg += " Service: " + service + "\r\n";
                //errmsg += " Method: " + method.ToString() + "\r\n";
                //errmsg += " Body: " + jsonBody + "\r\n";
                //clog.log.Info(errmsg);

                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(SkipSSL);

                if (key != null)
                {
                   if(key is string){
                        service += "('" + key.ToString() + "')";
                   }
                   else {
                        service += "(" + key.ToString() + ")";
                   }

                }


                if (action != "")
                {
                    service += "/" + action;
                }

                restClient = new RestClient(configuration["URLServiceLayer"]);
                restRequest = new RestRequest("/" + service + query) { Method = method };
                restRequest.AddHeader("Cookie", $"B1SESSION={SessionId}");

                if (method == Method.GET && action == "")
                {
                    restRequest.AddParameter("Content-type", "application/json");
                }

                if (jsonBody != "")
                {
                    restRequest.AddJsonBody(jsonBody);
                }

                response = restClient.Execute(restRequest);
                responseContent = response.Content;


                //errmsg += " Functon: CallServiceLayer " + "\r\n";
                //errmsg += " Service: " + service + "\r\n";
                //errmsg += " Method: " + method.ToString() + "\r\n";
                //errmsg += " Body: " + jsonBody + "\r\n";
                //errmsg += " StatusCode: " + response.StatusCode.ToString() + "\r\n";
                //errmsg += " Message: " + response.Content + "\r\n";
                //clog.log.Info(errmsg);

                return response.StatusCode;

            }
            catch (Exception e)
            {
                //errmsg += " Functon: CallServiceLayer " + "\r\n";
                //errmsg += " Service: " + service + "\r\n";
                //errmsg += " Method: " + method.ToString() + "\r\n";
                //errmsg += " Body: " + jsonBody + "\r\n";
                //errmsg += " Message: " + e.Message + "\r\n";
                //clog.log.Fatal(errmsg);
                throw e;
            }
        }


        public bool LoginSL(ref string SessionId)
        {
            RestClient restClient = null;
            RestRequest restRequest = null;
            IRestResponse response = null;
            Login login = new Login() { CompanyDB = configuration["CompanyDB"], UserName = configuration["UserName"], Password = configuration["Password"] };

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(SkipSSL);
                restClient = new RestClient(configuration["URLServiceLayer"]);
                restRequest = new RestRequest("/Login") { Method = Method.POST };
                restRequest.AddParameter("Content-Type", "application/json");
                restRequest.AddJsonBody(JsonConvert.SerializeObject(login));

                response = restClient.Execute(restRequest);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    SessionId = (string)JObject.Parse(response.Content)["SessionId"];
                    return true;
                }

                return false;
            }
            catch (Exception e)
            {
                throw e;
            }
            
        }

        public bool SkipSSL(object sender, X509Certificate certification, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }

    
}
