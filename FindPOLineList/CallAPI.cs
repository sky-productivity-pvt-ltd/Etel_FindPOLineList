using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Metadata;

namespace FindPOLineList
{
    public class CallAPI
    {
        public string CreateTaskAPI(int companyId, int checksheeId, int stepId, int stepNo, int status, int parentTaskId, JArray filledDataJson)
        {
            try
            {
                JObject joMainData = new JObject
                {
                    { "Company_id", companyId },
                    { "Subcategory", "" },
                    { "checksheet_id", checksheeId },
                    { "Step_id", stepId },
                    { "StepNo", stepNo },
                    { "Status", status },
                    { "Data", filledDataJson },
                    { "button", new JArray() },
                    { "millisecond", CurrentMillis.Millis },
                    { "createby", 64132 },
                    { "UpdatedBy", -1 },
                    { "master", "" },
                    { "sub", "[]" },
                    { "type", "new" },
                    { "photo_temp", new JArray() },
                    { "PARENT_TASK_ID", parentTaskId },
                    { "p_match_temp_id", 0 }
                };

                JArray jaMainData = new JArray
                {
                    joMainData
                };

                JObject joSubData = new JObject
                {
                    { "RequestFor", "Single" },
                    { "WhereCondition", "1" }
                };

                JArray jaSubData = new JArray
                {
                    joSubData
                };

                JProperty main_p = new JProperty("MainData", jaMainData);
                JProperty sub_p = new JProperty("SubData", jaSubData);
                JProperty call_type = new JProperty("call_type", "web");

                JArray Data_json = new JArray
                {
                    new JObject
                    {
                        { "MainData", jaMainData },
                        { "SubData", jaSubData },
                        { "call_type", "web" }
                    }
                };


                string Return = "";
                int retries = 3;
                while (retries > 0)
                {
                    try
                    {
                        Return = CoreAPI("https://Dev.easyform.in/coreapi/"
                                     , "api/InsertUpdateDelete_MainAndSub_Task"
                                     , "POST"
                                     , JsonConvert.SerializeObject(Data_json)
                                     , true);

                        JToken Json_old_obj = JToken.Parse(Return);

                        string status1 = Json_old_obj[0]["status"].ToString();
                        if (status1 != "3")
                        {
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        retries--;
                        if (retries == 0)
                        {
                            throw ex;
                        }
                        Thread.Sleep(1000);  // Wait before retrying (e.g., 1 second)
                    }
                }
                return Return;

                 
            }
            catch (Exception ex)
            {
                JObject joPrameter = new JObject
                {
                    { "companyId", companyId },
                    { "checksheeId", checksheeId },
                    { "stepId", stepId },
                    { "stepNo", stepNo },
                    { "status", status },
                    { "parentTaskId", parentTaskId },
                    { "filledDataJson", filledDataJson }
                };

                ErrorHandling Error = new ErrorHandling();
                Error.WriteErrorLog(ex, JsonConvert.SerializeObject(joPrameter), "Mids_CreateTaskAPI", "");
                throw ex;
            }
        }
        public string CoreAPI(string url, string methodName, string methodType
            , string dataJson
            , bool auth, int userId = 64132, string username = "system.tanzania", int companyId = 1838, string authKey = "0d196647a594153cad7dbb5ab166b9d89bea")
        {
            try
            {
                string returnString = "";

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(String.Concat(url, methodName));
                request.Method = methodType;

                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                if (auth)
                {
                    JObject joAuth = new JObject();
                    if (username != "")
                    { joAuth.Add("username", username); }
                    else
                    { joAuth.Add("username", username); }

                    if (userId != 0)
                    { joAuth.Add("user_id", userId); }

                    if (companyId != 0)
                    { joAuth.Add("company_id", companyId); }



                    joAuth.Add("u_c", "");
                    request.Headers.Add("auth1", JsonConvert.SerializeObject(joAuth));

                    request.Headers.Add("authkey", authKey);
                    request.Headers.Add("API_Key", authKey);
                }
                request.ContentType = "application/json";


                if (!string.IsNullOrEmpty(dataJson) && methodType == "POST")
                {
                    byte[] data = Encoding.UTF8.GetBytes(dataJson);
                    using (Stream requestStream = request.GetRequestStream())
                    {
                        requestStream.Write(data, 0, data.Length);
                    }
                    request.ContentLength = data.Length;
                }
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    var responseValue = string.Empty;

                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        var message = String.Format("Request failed. Received HTTP {0}", response.StatusCode);
                        throw new ApplicationException(message);
                    }

                    // grab the response
                    using (var responseStream = response.GetResponseStream())
                    {
                        if (responseStream != null)
                            using (var reader = new StreamReader(responseStream))
                            {
                                responseValue = reader.ReadToEnd();
                            }
                    }

                    returnString = responseValue;
                }

                return returnString;
            }
            catch (Exception ex)
            {
                JObject joPrameter = new JObject
                {
                    { "url", url },
                    { "methodName", methodName },
                    { "methodType", methodType },
                    { "dataJson", dataJson }
                };

                ErrorHandling Error = new ErrorHandling();
                Error.WriteErrorLog(ex, JsonConvert.SerializeObject(joPrameter), "Mids_CoreAPI", "");

                throw ex;
            }
        }

        #region Customer

        public string CreateCustomer(string jaCustomer, int customerChecksheetId, string customerChecksheetName
            , int customerStepId, string customerUniq, int companyId)
        {
            try
            {
                JArray jaRequest = new JArray
                {
                    new JObject
                    {
                        { "CustomerUniqId", customerUniq },
                        { "Company_id", companyId },
                        { "Subcategory", customerChecksheetName },
                        { "Step_id", customerStepId },
                        { "StepNo", 0 },
                        { "checksheet_id", customerChecksheetId },
                        { "Status", 0 },
                        { "Data", JArray.Parse(jaCustomer) },
                        { "createby", -1 },
                        { "UpdatedBy", -1 },
                        { "checksheet_type", 3 },
                        { "photo_temp", new JArray() }
                    }
                };

                return CoreAPI("https://localhost:5000/"
                     , "api/WCF_insert_Customer_Master_And_Sub_Type"
                     , "POST"
                     , JsonConvert.SerializeObject(jaRequest)
                     , true);
            }
            catch (Exception ex)
            {
                JObject joPrameter = new JObject
                {
                    { "jaCustomer", jaCustomer },
                    { "customerChecksheetId", customerChecksheetId },
                    { "customerChecksheetName", customerChecksheetName },
                    { "customerStepId", customerStepId },
                    { "customerUniq", customerUniq },
                    { "companyId", companyId },
                };

                ErrorHandling Error = new ErrorHandling();
                Error.WriteErrorLog(ex, JsonConvert.SerializeObject(joPrameter), "Mids_CreateCustomer", "");
                throw ex;
            }
        }

        #endregion

        #region Log Upload

        public string QuestionLogAPI(string jaQuestionLog, int taskId, int checksheetId, int stepNo, string btnAction = "submit")
        {
            try
            {

                JArray jaRequest = new JArray
                {
                    new JObject
                    {
                        {"server_task_id", taskId },
                        {"c_type", 1 },
                        {"btn_action",btnAction },
                        {"date",CurrentMillis.Millis },
                        {"date_s" ,DateTime.Now.ToString()},
                        {"q_data", new JArray
                            {
                                new JObject
                                {
                                    { "data", JArray.Parse(jaQuestionLog)},
                                    { "User_Id", 0},
                                    { "check_type", checksheetId},
                                    { "stepid", stepNo},
                                    { "lat", 0},
                                    { "long1", 0},
                                    { "address", ""}
                                }
                            }
                        },
                        { "utcmilliseconds", CurrentMillis.Millis },
                        { "timezone", 0 }
                    }
                };

                return CoreAPI("https://localhost:5000/"
                    , "api/WCF_InsertQuestionMaster_Log_EF_1_0_0_0"
                    , "POST"
                    , JsonConvert.SerializeObject(jaRequest)
                    , true);


            }
            catch (Exception ex)
            {
                JObject joPrameter = new JObject
                {
                    { "jaQuestionLog", jaQuestionLog },
                    { "taskId", taskId },
                    { "checksheetId", checksheetId },
                    { "stepNo", stepNo }
                };

                ErrorHandling exeError = new ErrorHandling();
                exeError.WriteErrorLog(ex, JsonConvert.SerializeObject(joPrameter), "", "");
                throw ex;
            }
        }
        #endregion


        public string TechnicianCallAPI(int taskId, int checksheetId, int stepNo, int changeStatus, int flowid, int oldStatus, string username)
        {
            try
            {
                JArray tech_array = new JArray();

                JProperty username_pro = new JProperty("username", username);
                JProperty height = new JProperty("height", "0");
                JProperty tower = new JProperty("tower", "");
                JProperty check_type = new JProperty("check_type", checksheetId);
                JProperty u_c = new JProperty("u_c", "");
                JProperty siteid = new JProperty("siteid", "");
                JProperty servertaskid = new JProperty("servertaskid", taskId);
                JProperty status = new JProperty("status", changeStatus);
                JProperty old_status = new JProperty("old_status", oldStatus);
                JProperty step = new JProperty("step", "0");
                JProperty moduledef = new JProperty("moduledef", "");
                JProperty global_var = new JProperty("global_var", "");
                JProperty flow_id = new JProperty("flow_id", flowid);
                JProperty company_id = new JProperty("company_id", "0");
                JProperty punch_flag = new JProperty("punch_flag", "0");
                JProperty in_ms = new JProperty("in_ms", "0");
                JProperty out_ms = new JProperty("out_ms", "0");
                JProperty duration = new JProperty("duration", "0");
                JProperty utcmilliseconds = new JProperty("utcmilliseconds", "0");
                JProperty timezone = new JProperty("timezone", "0");

                JObject tech_obj = new JObject(username_pro, height, tower, check_type, u_c, siteid, servertaskid, status, old_status,
                    step, moduledef, global_var, flow_id, company_id, punch_flag, in_ms, out_ms, duration, utcmilliseconds, timezone);

                tech_array.Add(tech_obj);

                string response = CoreAPI("https://localhost:5000/"
                    , "api/WCFTechnicianDataUpload"
                    , "POST"
                    , JsonConvert.SerializeObject(tech_array)
                    , true);

                JArray jaResponse = JArray.Parse(response);

                if (jaResponse[0]["status"].ToString() == "1")
                {
                    return "Success";
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                JObject joPrameter = new JObject
                {
                    { "taskId", taskId },
                    { "checksheetId", checksheetId },
                    { "stepNo", stepNo },
                    { "changeStatus", changeStatus },
                    { "oldStatus", oldStatus },
                    { "username", username }
                };
                ErrorHandling exeError = new ErrorHandling();
                exeError.WriteErrorLog(ex, JsonConvert.SerializeObject(joPrameter), "", "");
                return "";
            }
        }


   


        public string Type20API(JArray tye20data)
        {
            try
            {
                
                return CoreAPI("https://login.easyform.in/coreapi/"
                    , "api/WCF_Type20Filter"
                    , "POST"
                    , JsonConvert.SerializeObject(tye20data)
                    , true);
            }
            catch (Exception ex)
            {
                JObject joPrameter = new JObject
                {
                    { "tye20data", tye20data }
                };

                ErrorHandling Error = new ErrorHandling();
                Error.WriteErrorLog(ex, JsonConvert.SerializeObject(joPrameter), "FindPOLineCreation", "");
                throw ex;
            }
        }
    }

    static class CurrentMillis
    {
        private static readonly DateTime Jan1St1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        /// <summary>Get extra long current timestamp</summary>
        public static long Millis { get { return (long)((DateTime.UtcNow - Jan1St1970).TotalMilliseconds); } }
    }
}
