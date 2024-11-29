using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FindPOLineList
{
    public class E_TelTaskCreation
    {
        public JArray E_Tel_ParentTaskCreation(JArray DATA)
        {
            JArray Rtn_ja = new JArray();
            JArray midja = new JArray();
            DatabaseFile DB = new DatabaseFile();
            JArray FillData_JA = new JArray();
            CallAPI CA = new CallAPI();
            JArray Jarr_1 = new JArray();
            DataTable dt = new DataTable();
            int companyId = 1838;

            try
            {
                DB = new DatabaseFile();
                //insert data in MidsapiData Table

                int totalpoline = 0;

                // Po cal API Call 

                Guid obj = Guid.NewGuid();

                JArray jArray = new JArray {
                    commanjarray.CommanJobject("Guid",obj.ToString())
                };

                DataTable POline_DT = DB.GetData("SelectPO_CalculationBYGuid", jArray);

                E_TelTaskCreation ET = new E_TelTaskCreation();

                if (POline_DT.Rows.Count > 0)
                {

                    for (int k = 0; k < POline_DT.Rows.Count; k++)
                    {
                        string Po_Number = POline_DT.Rows[k]["Po_Number"].ToString();
                        string Po_Task_id = POline_DT.Rows[k]["Po_Task_id"].ToString();
                        int PoLine_Taskid = Convert.ToInt32(POline_DT.Rows[k]["PoLine_Taskid"].ToString());

                        string jobid = POCallationAPICall(PoLine_Taskid);


                        ///update JobID and Status

                        JArray MainJA = new JArray
                        {
                            commanjarray.CommanJobject("Po_Task_id",Po_Task_id),
                            commanjarray.CommanJobject("Guid",obj.ToString()),
                            commanjarray.CommanJobject("Job_ID",jobid),
                        };

                        DB.GetData("UpdateJobPO_CalculationBYGuid", MainJA);
                    }

                }

                #region Fill All Type22
                JArray CountryJA = FillCountry();
                #endregion

                for (int j = 0; j < DATA.Count; j++)
                {



                    string Server_parent_task_id = "0";
                    string Server_Sub_task_id = "0";


                   



                    #region Check E TeL Parent Task
                    string poNumber = DATA[j]["poNumber"].ToString();


                    JArray Po_Ja = new JArray {
                        commanjarray.CommanJobject("poNumber",poNumber)


                    };
                    DataTable potaskdt = DB.GetData("CheckETel_ParentTask", Po_Ja);

                    if (potaskdt.Rows.Count > 0)
                    {
                        Server_parent_task_id = potaskdt.Rows[0]["task_id"].ToString();
                    }
                    #endregion
                    #region E TeL Parent Task Creattion Jayanti
                    if (Server_parent_task_id == "0")
                    {
                        int Parent_checksheeId = 90935;
                        // Questionlog_BulkUpload_DA QBDA = new Questionlog_BulkUpload_DA();

                        Jarr_1 = new JArray
                            {
                                commanjarray.CommanJobject("checksheet_id", Parent_checksheeId.ToString())
                            };

                        dt = DB.GetData("get_checksheetMaster_AllData_using_CheckSheetID", Jarr_1);

                        if (dt.Rows.Count > 0)
                        {
                            string AllDataJSon = dt.Rows[0]["AllData"].ToString();
                            JArray AllDataJA = JArray.Parse(AllDataJSon);
                            string DataJson = AllDataJA[0]["step_data"][0]["step"][0]["data"].ToString();
                            JArray DataJsonJA = JArray.Parse(ThreeKeyLogic.ThreeKeyLogicConvert(JArray.Parse(DataJson)));


                            DataTable Controldt = GetE_Tel_TaskControlDT("parent");
                            for (int i = 0; i < Controldt.Rows.Count; i++)
                            {
                                string KeyName = Controldt.Rows[i]["KeyName"].ToString();
                                string QueID = Controldt.Rows[i]["QueID"].ToString();

                                JToken[] Conrol_ANS = DataJsonJA.Where(z => z["que_id"].ToString() == QueID).Select(x => x["ans"]).ToArray();

                                if (Conrol_ANS.Count() > 0)
                                {
                                    if (QueID == "7")
                                    {
                                        Conrol_ANS[0][0] = "Purchase Order";
                                        Conrol_ANS[0][0].AddAfterSelf("4900");

                                    }
                                    if (QueID == "4")
                                    {
                                        Conrol_ANS[0][0] = "system.tanzania";
                                        Conrol_ANS[0][0].AddAfterSelf("64132");

                                    }
                                    else if (QueID == "1")
                                    {
                                        Conrol_ANS[0][0] = "HUAWEI TECHNOLOGIES (TANZANIA) COMPANY LIMITED";
                                        Conrol_ANS[0][0].AddAfterSelf("22326");

                                    }
                                    else if (QueID == "13")
                                    {

                                        JToken[] AnsJA1 = CountryJA.Where(z => z["Name"].ToString().ToUpper() == DATA[j]["shipToLocationCode"].ToString().ToUpper()).ToArray();

                                        if (AnsJA1.Count() > 0)
                                        {
                                            Conrol_ANS[0][0] =  AnsJA1[0]["Name"].ToString();

                                            Conrol_ANS[0][0].AddAfterSelf(AnsJA1[0]["ID"].ToString());
                                        }

                                    }
                                    else if (QueID == "15")
                                    {
                                        Conrol_ANS[0][0] = "TZS";
                                        Conrol_ANS[0][0].AddAfterSelf("22325");

                                    }
                                    else if (QueID == "20")
                                    {
                                        JArray FinancialYear = FillFinancialYear(DATA[j]["publishDate"].ToString());


                                        if (FinancialYear.Count > 0)
                                        {

                                            Conrol_ANS[0][0] = FinancialYear[0].ToString();
                                            Conrol_ANS[0][0].AddAfterSelf(FinancialYear[1].ToString());

                                        }


                                    }
                                    else if (QueID == "1042")
                                    {
                                        DateTime dateTime = DateTime.Parse(DATA[j]["publishDate"].ToString());

                                        string ans = dateTime.ToString("yyyy/MM/dd");

                                        Conrol_ANS[0][0] = ans;

                                    }

                                    else
                                    {
                                        if (DATA[j][KeyName] != null)
                                        {
                                            Conrol_ANS[0][0] = DATA[j][KeyName].ToString();
                                        }


                                    }

                                }

                            }



                            //PO Approval PRE,AC1 & AC2 spliting

                            string termsDescription = DATA[j]["termsDescription"].ToString();

                            DataJsonJA = SplitPoApproval(DataJsonJA, termsDescription);




                            string api_responce = CA.CreateTaskAPI(companyId, Parent_checksheeId, 1, 0, 0, 0, DataJsonJA);
                            if (api_responce != "")
                            {
                                Rtn_ja = JArray.Parse(api_responce);

                                if (Rtn_ja[0]["status"].ToString() == "1")
                                {
                                    Server_parent_task_id = Rtn_ja[0]["Server_task_id"].ToString();



                                }
                                else
                                {
                                    Server_parent_task_id = "0";

                                }
                            }
                        }


                    }
                    #endregion

                    #region E TeL Sub Task Creattion Jayanti

                    if (Server_parent_task_id != "0")
                    {

                        int Sub_checksheeId = 90967
;
                        // Questionlog_BulkUpload_DA QBDA = new Questionlog_BulkUpload_DA();

                        Jarr_1 = new JArray
                            {
                                commanjarray.CommanJobject("checksheet_id", Sub_checksheeId.ToString())
                            };

                        dt = DB.GetData("get_checksheetMaster_AllData_using_CheckSheetID", Jarr_1);

                        if (dt.Rows.Count > 0)
                        {
                            string AllDataJSon = dt.Rows[0]["AllData"].ToString();
                            JArray AllDataJA = JArray.Parse(AllDataJSon);
                            string DataJson = AllDataJA[0]["step_data"][0]["step"][0]["data"].ToString();
                            JArray DataJsonJA = JArray.Parse(ThreeKeyLogic.ThreeKeyLogicConvert(JArray.Parse(DataJson)));


                            DataTable Controldt = GetE_Tel_TaskControlDT("Sub");
                            for (int i = 0; i < Controldt.Rows.Count; i++)
                            {
                                string KeyName = Controldt.Rows[i]["KeyName"].ToString();
                                string QueID = Controldt.Rows[i]["QueID"].ToString();

                                JToken[] Conrol_ANS = DataJsonJA.Where(z => z["que_id"].ToString() == QueID).Select(x => x["ans"]).ToArray();

                                if (Conrol_ANS.Count() > 0)
                                {
                                    if (QueID == "7")
                                    {
                                        Conrol_ANS[0][0] = "PO Line Item";
                                        Conrol_ANS[0][0].AddAfterSelf("4901");

                                    }
                                    else if (QueID == "13")
                                    {
                                        //Conrol_ANS[0][0] = DATA[j][KeyName].ToString();
                                        //Conrol_ANS[0][0].AddAfterSelf("64132");

                                        JToken[] AnsJA1 = CountryJA.Where(z => z["Name"].ToString().ToUpper() == DATA[j]["shipToLocationCode"].ToString().ToUpper()).ToArray();

                                        if (AnsJA1.Count() > 0)
                                        {
                                            Conrol_ANS[0][0] = AnsJA1[0]["Name"].ToString();

                                            Conrol_ANS[0][0].AddAfterSelf(AnsJA1[0]["ID"].ToString());
                                        }
                                    }
                                    else if (QueID == "18")
                                    {
                                        string manufactureSiteInfo = DATA[j]["manufactureSiteInfo"].ToString();

                                        Conrol_ANS[0][0] = SplitmanufactureSiteInfo_One(manufactureSiteInfo);

                                    }
                                    else if (QueID == "4")
                                    {
                                        Conrol_ANS[0][0] = "system.tanzania";
                                        Conrol_ANS[0][0].AddAfterSelf("64132");

                                    }
                                    else if (QueID == "19")
                                    {
                                        string manufactureSiteInfo = DATA[j]["manufactureSiteInfo"].ToString();

                                        Conrol_ANS[0][0] = SplitmanufactureSiteInfo_Two(manufactureSiteInfo);

                                    }
                                    else if (QueID == "1030")
                                    {
                                        string manufactureSiteInfo = DATA[j]["manufactureSiteInfo"].ToString();

                                        Conrol_ANS[0][0] = SplitmanufactureSiteInfo_Three(manufactureSiteInfo);

                                    }
                                    else if (QueID == "21")
                                    {
                                        Conrol_ANS[0][0] = poNumber;
                                        Conrol_ANS[0][0].AddAfterSelf(Server_parent_task_id);
                                    }
                                    else if (QueID == "1021")
                                    {
                                        Conrol_ANS[0][0] = Convert.ToDouble(DATA[j]["unitPrice"].ToString()) * Convert.ToDouble(DATA[j]["quantityCancelled"].ToString());
                                    }
                                    else if (QueID == "1022")
                                    {
                                        Conrol_ANS[0][0] = Convert.ToDouble(DATA[j]["unitPrice"].ToString()) * Convert.ToDouble(DATA[j]["quantityReceived"].ToString());
                                    }
                                    else if (QueID == "1023")
                                    {
                                        Conrol_ANS[0][0] = Convert.ToDouble(DATA[j]["unitPrice"].ToString()) * Convert.ToDouble(DATA[j]["quantityBilled"].ToString());
                                    }
                                    else if (QueID == "1024")
                                    {
                                        Conrol_ANS[0][0] = Convert.ToDouble(DATA[j]["unitPrice"].ToString()) * Convert.ToDouble(DATA[j]["quantityAccepted"].ToString());
                                    }
                                    else if (QueID == "1025")
                                    {
                                        Conrol_ANS[0][0] = Convert.ToDouble(DATA[j]["unitPrice"].ToString()) * Convert.ToDouble(DATA[j]["quantityRejected"].ToString());
                                    }
                                    else if (QueID == "1027")
                                    {
                                        Conrol_ANS[0][0] = Convert.ToDouble(DATA[j]["unitPrice"].ToString()) * Convert.ToDouble(DATA[j]["acQty"].ToString());
                                    }
                                    else if (QueID == "1026")
                                    {
                                        Conrol_ANS[0][0] = Convert.ToDouble(DATA[j]["unitPrice"].ToString()) * Convert.ToDouble(DATA[j]["dueQty"].ToString());
                                    }
                                    else if (QueID == "24")
                                    {
                                        Conrol_ANS[0][0] = Convert.ToDouble(DATA[j]["unitPrice"].ToString()) * Convert.ToDouble(DATA[j]["quantity"].ToString());
                                    }
                                    else if (QueID == "1032")
                                    {
                                        Conrol_ANS[0][0] = (Convert.ToDouble(DATA[j]["unitPrice"].ToString()) * Convert.ToDouble(DATA[j]["quantity"].ToString())) * Convert.ToDouble(DATA[j]["taxRate"].ToString());
                                    }
                                    else if (QueID == "1031")
                                    {
                                        Conrol_ANS[0][0] = ((Convert.ToDouble(DATA[j]["unitPrice"].ToString()) * Convert.ToDouble(DATA[j]["quantity"].ToString())) * Convert.ToDouble(DATA[j]["taxRate"].ToString())) + Convert.ToDouble(DATA[j]["unitPrice"].ToString()) * Convert.ToDouble(DATA[j]["quantity"].ToString());
                                    }
                                    else if (QueID == "25")
                                    {
                                        Conrol_ANS[0][0] = Server_parent_task_id;
                                    }
                                    else if (QueID == "1002")
                                    {
                                        Conrol_ANS[0][0] = "PO Activity";
                                        Conrol_ANS[0][0].AddAfterSelf("150");
                                    }
                                    else if (QueID == "1050")
                                    {
                                        JArray FinancialYear = FillFinancialYear(DATA[j]["publishDate"].ToString());


                                        if (FinancialYear.Count > 0)
                                        {

                                            Conrol_ANS[0][0] = FinancialYear[0].ToString();
                                            Conrol_ANS[0][0].AddAfterSelf(FinancialYear[1].ToString());

                                        }


                                    }
                                    else if (QueID == "1047")
                                    {
                                        DateTime dateTime = DateTime.Parse(DATA[j]["publishDate"].ToString());

                                        string ans = dateTime.ToString("yyyy/MM/dd");

                                        Conrol_ANS[0][0] = ans;

                                    }
                                    else if (QueID == "1049")
                                    {
                                        Conrol_ANS[0][0] = "TZS";
                                        Conrol_ANS[0][0].AddAfterSelf("22325");

                                    }
                                    else if (QueID == "1048")
                                    {
                                        Conrol_ANS[0][0] = "HUAWEI TECHNOLOGIES (TANZANIA) COMPANY LIMITED";
                                        Conrol_ANS[0][0].AddAfterSelf("22326");

                                    }
                                    else if (QueID == "1062")
                                    {
                                        Conrol_ANS[0][0] = "Pre Invoice";

                                    }
                                    else if (QueID == "1063")
                                    {
                                        Conrol_ANS[0][0] = "AC1 Invoice";

                                    }
                                    else if (QueID == "1064")
                                    {
                                        Conrol_ANS[0][0] = "AC2 invoice";

                                    }
                                    else
                                    {
                                        if (DATA[j][KeyName] != null)
                                        {
                                            Conrol_ANS[0][0] = DATA[j][KeyName].ToString();
                                        }


                                    }

                                }

                            }


                            string termsDescription = DATA[j]["termsDescription"].ToString();

                            DataJsonJA = SplitPoApproval_Sub(DataJsonJA, termsDescription);

                            string api_responce = CA.CreateTaskAPI(companyId, Sub_checksheeId, 1, 0, 0, Convert.ToInt32(Server_parent_task_id), DataJsonJA);
                            if (api_responce != "")
                            {
                                Rtn_ja = JArray.Parse(api_responce);

                                if (Rtn_ja[0]["status"].ToString() == "1")
                                {
                                    Server_Sub_task_id = Rtn_ja[0]["Server_task_id"].ToString();

                                    // Insrt in PO_Calculation_log

                                    JArray Po_cal = new JArray {

                                        commanjarray.CommanJobject("Po_Number",poNumber),
                                        commanjarray.CommanJobject("Po_Task_id",Server_parent_task_id),
                                        commanjarray.CommanJobject("PoLine_Taskid",Server_Sub_task_id)
                                    };

                                    DB.GetData("InsertvPOCalculationlog", Po_cal);
                                }
                                else
                                {

                                    Server_Sub_task_id = "0";
                                }
                            }
                        }



                    }

                    JObject JO = JObject.Parse(DATA[j].ToString());

                    var properties = JO.Properties().ToList();


                    // Insert the new key-value pair at the desired index
                    if (properties.FindIndex(p => p.Name == "receivingRoutingId") != -1)
                    {
                        properties.Insert(properties.FindIndex(p => p.Name == "receivingRoutingId"), new JProperty("Po Number", Server_parent_task_id));
                        properties.Insert(properties.FindIndex(p => p.Name == "receivingRoutingId"), new JProperty("Po Line", Server_Sub_task_id));
                    }

                    // Rebuild the JObject from the updated properties list
                    JObject updatedJsonObject = new JObject(properties);

                    DATA[j] = updatedJsonObject;
                    #endregion
                }


                Guid obj1 = Guid.NewGuid();
                jArray = new JArray {
                    commanjarray.CommanJobject("Guid",obj1.ToString())
                };

                POline_DT = DB.GetData("SelectPO_CalculationBYGuid", jArray);



                if (POline_DT.Rows.Count > 0)
                {

                    for (int k = 0; k < POline_DT.Rows.Count; k++)
                    {
                        string Po_Number = POline_DT.Rows[k]["Po_Number"].ToString();
                        string Po_Task_id = POline_DT.Rows[k]["Po_Task_id"].ToString();
                        int PoLine_Taskid = Convert.ToInt32(POline_DT.Rows[k]["PoLine_Taskid"].ToString());

                        string jobid = POCallationAPICall(PoLine_Taskid);


                        ///update JobID and Status

                        JArray MainJA = new JArray
                        {
                            commanjarray.CommanJobject("Po_Task_id",Po_Task_id),
                            commanjarray.CommanJobject("Guid",obj1.ToString()),
                            commanjarray.CommanJobject("Job_ID",jobid),
                        };

                        DB.GetData("UpdateJobPO_CalculationBYGuid", MainJA);
                    }

                }



            }
            catch (Exception ex)
            {
                DB = new DatabaseFile();
                ErrorHandling Error = new ErrorHandling();
                Error.WriteErrorLog(ex, DATA.ToString(), "Mids_LBTask_Creattion", "");

                throw ex;
            }
            return DATA;

        }


        public DataTable GetE_Tel_TaskControlDT(string flag)
        {
            DataTable dt = new DataTable();
            try
            {
                DatabaseFile DB = new DatabaseFile();
                JArray JA = new JArray {
                    commanjarray.CommanJobject("flag",flag)
                };

                dt = DB.GetData("GetE_Tel_TaskControlDT", JA);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return dt;
        }


        public JArray SplitPoApproval(JArray DataJsonJA, string termsDescription)
        {

            try
            {

                termsDescription = termsDescription.Replace("(", "");
                termsDescription = termsDescription.Replace(")", "");
                termsDescription = termsDescription.Replace("▍", "/");

                string[] Split = termsDescription.Split('/');


                // Trim and extract AC1 and AC2

                string pre = "";
                string ac1 = "";
                string ac2 = "";
                string ac3 = "";


                for (int i = 0; i < Split.Length; i++)
                {
                    if (Split[i].Contains("PRE"))
                    {
                        pre = Split[i].Trim();
                    }

                    if (Split[i].Contains("AC1"))
                    {
                        ac1 = Split[i].Trim();
                    }

                    if (Split[i].Contains("AC2"))
                    {
                        ac2 = Split[i].Trim();
                    }

                    if (Split[i].Contains("AC3"))
                    {
                        ac3 = Split[i].Trim();
                    }
                }

                pre = pre.Replace("PRE", "");
                ac1 = ac1.Replace("AC1", "");
                ac2 = ac2.Replace("AC2", "");
                ac3 = ac3.Replace("AC3", "");

                string[] prelsit = pre.Split(',');
                string[] ac1lsit = ac1.Split(',');
                string[] ac2lsit = ac2.Split(',');
                string[] ac3lsit = ac3.Split(',');

                string PRI_Percent = "";
                string PRI_Days = "";
                string PRI_Remark = "";
                string AC1_Percent = "";
                string AC1_Days = "";
                string AC1_Remark = "";
                string AC2_Percent = "";
                string AC2_Days = "";
                string AC2_Remark = "";
                string AC3_Percent = "";
                string AC3_Days = "";
                string AC3_Remark = "";


                if (prelsit.Length == 3)
                {
                    PRI_Percent = prelsit[0].Trim();

                    PRI_Percent = PRI_Percent.Replace("%", "").Trim();
                    PRI_Days = prelsit[1].Trim();
                    PRI_Remark = prelsit[2].Trim();

                    PRI_Days = GetNumberFromString(PRI_Days).ToString();

                    JToken[] PRI_PercentJA = DataJsonJA.Where(z => z["que_id"].ToString() == "1008").Select(x => x["ans"]).ToArray();
                    PRI_PercentJA[0][0] = PRI_Percent;
                    JToken[] PRI_DaysJA = DataJsonJA.Where(z => z["que_id"].ToString() == "1009").Select(x => x["ans"]).ToArray();
                    PRI_DaysJA[0][0] = PRI_Days;
                    JToken[] PRI_RemarkJA = DataJsonJA.Where(z => z["que_id"].ToString() == "1010").Select(x => x["ans"]).ToArray();
                    PRI_RemarkJA[0][0] = PRI_Remark;

                }

                if (ac1lsit.Length == 3)
                {
                    AC1_Percent = ac1lsit[0].Trim();
                    AC1_Percent = AC1_Percent.Replace("%", "").Trim();
                    AC1_Days = ac1lsit[1].Trim();
                    AC1_Remark = ac1lsit[2].Trim();

                    AC1_Days = GetNumberFromString(AC1_Days).ToString();

                    JToken[] AC1_PercentJA = DataJsonJA.Where(z => z["que_id"].ToString() == "1011").Select(x => x["ans"]).ToArray();
                    AC1_PercentJA[0][0] = AC1_Percent;
                    JToken[] AC1_DaysJA = DataJsonJA.Where(z => z["que_id"].ToString() == "1012").Select(x => x["ans"]).ToArray();
                    AC1_DaysJA[0][0] = AC1_Days;
                    JToken[] AC1_RemarkJA = DataJsonJA.Where(z => z["que_id"].ToString() == "1013").Select(x => x["ans"]).ToArray();
                    AC1_RemarkJA[0][0] = AC1_Remark;
                }
                if (ac2lsit.Length == 3)
                {
                    AC2_Percent = ac2lsit[0].Trim();

                    AC2_Percent = AC2_Percent.Replace("%", "").Trim();
                    AC2_Days = ac2lsit[1].Trim();
                    AC2_Remark = ac2lsit[2].Trim();

                    AC2_Days = GetNumberFromString(AC2_Days).ToString();

                    JToken[] AC2_PercentJA = DataJsonJA.Where(z => z["que_id"].ToString() == "1014").Select(x => x["ans"]).ToArray();
                    AC2_PercentJA[0][0] = AC2_Percent;
                    JToken[] AC2_DaysJA = DataJsonJA.Where(z => z["que_id"].ToString() == "1015").Select(x => x["ans"]).ToArray();
                    AC2_DaysJA[0][0] = AC2_Days;
                    JToken[] AC2_RemarkJA = DataJsonJA.Where(z => z["que_id"].ToString() == "1016").Select(x => x["ans"]).ToArray();
                    AC2_RemarkJA[0][0] = AC2_Remark;
                }

                if (ac2lsit.Length == 3)
                {
                    AC3_Percent = ac3lsit[0].Trim();
                    AC3_Percent = AC3_Percent.Replace("%", "").Trim();
                    AC3_Days = ac3lsit[1].Trim();
                    AC3_Remark = ac3lsit[2].Trim();

                    AC3_Days = GetNumberFromString(AC3_Days).ToString();

                    //JToken[] AC3_PercentJA = DataJsonJA.Where(z => z["que_id"].ToString() == "1014").Select(x => x["ans"]).ToArray();
                    //AC3_PercentJA[0][0] = AC3_Percent;
                    //JToken[] AC3_DaysJA = DataJsonJA.Where(z => z["que_id"].ToString() == "1015").Select(x => x["ans"]).ToArray();
                    //AC3_DaysJA[0][0] = AC3_Days;
                    //JToken[] AC3_RemarkJA = DataJsonJA.Where(z => z["que_id"].ToString() == "1016").Select(x => x["ans"]).ToArray();
                    //AC3_RemarkJA[0][0] = AC3_Remark;
                }


                return DataJsonJA;
            }
            catch (Exception)
            {
                return DataJsonJA;
            }
        }

        public JArray SplitPoApproval_Sub(JArray DataJsonJA, string termsDescription)
        {

            try
            {

                termsDescription = termsDescription.Replace("(", "");
                termsDescription = termsDescription.Replace(")", "");
                termsDescription = termsDescription.Replace("▍", "/");

                string[] Split = termsDescription.Split('/');


                // Trim and extract AC1 and AC2

                string pre = "";
                string ac1 = "";
                string ac2 = "";
                string ac3 = "";


                for (int i = 0; i < Split.Length; i++)
                {
                    if (Split[i].Contains("PRE"))
                    {
                        pre = Split[i].Trim();
                    }

                    if (Split[i].Contains("AC1"))
                    {
                        ac1 = Split[i].Trim();
                    }

                    if (Split[i].Contains("AC2"))
                    {
                        ac2 = Split[i].Trim();
                    }

                    if (Split[i].Contains("AC3"))
                    {
                        ac3 = Split[i].Trim();
                    }
                }

                pre = pre.Replace("PRE", "");
                ac1 = ac1.Replace("AC1", "");
                ac2 = ac2.Replace("AC2", "");
                ac3 = ac3.Replace("AC3", "");

                string[] prelsit = pre.Split(',');
                string[] ac1lsit = ac1.Split(',');
                string[] ac2lsit = ac2.Split(',');
                string[] ac3lsit = ac3.Split(',');

                string PRI_Percent = "";
                string PRI_Days = "";
                string PRI_Remark = "";
                string AC1_Percent = "";
                string AC1_Days = "";
                string AC1_Remark = "";
                string AC2_Percent = "";
                string AC2_Days = "";
                string AC2_Remark = "";
                string AC3_Percent = "";
                string AC3_Days = "";
                string AC3_Remark = "";


                if (prelsit.Length == 3)
                {
                    PRI_Percent = prelsit[0].Trim();

                    PRI_Percent = PRI_Percent.Replace("%", "").Trim();
                    PRI_Days = prelsit[1].Trim();
                    PRI_Remark = prelsit[2].Trim();

                    PRI_Days = GetNumberFromString(PRI_Days).ToString();

                    JToken[] PRI_PercentJA = DataJsonJA.Where(z => z["que_id"].ToString() == "1043").Select(x => x["ans"]).ToArray();
                    PRI_PercentJA[0][0] = PRI_Percent;
                    JToken[] PRI_DaysJA = DataJsonJA.Where(z => z["que_id"].ToString() == "1044").Select(x => x["ans"]).ToArray();
                    PRI_DaysJA[0][0] = PRI_Days;
                    JToken[] PRI_RemarkJA = DataJsonJA.Where(z => z["que_id"].ToString() == "1052").Select(x => x["ans"]).ToArray();
                    PRI_RemarkJA[0][0] = PRI_Remark;

                }

                if (ac1lsit.Length == 3)
                {
                    AC1_Percent = ac1lsit[0].Trim();
                    AC1_Percent = AC1_Percent.Replace("%", "").Trim();
                    AC1_Days = ac1lsit[1].Trim();
                    AC1_Remark = ac1lsit[2].Trim();

                    AC1_Days = GetNumberFromString(AC1_Days).ToString();

                    JToken[] AC1_PercentJA = DataJsonJA.Where(z => z["que_id"].ToString() == "1037").Select(x => x["ans"]).ToArray();
                    AC1_PercentJA[0][0] = AC1_Percent;
                    JToken[] AC1_DaysJA = DataJsonJA.Where(z => z["que_id"].ToString() == "1038").Select(x => x["ans"]).ToArray();
                    AC1_DaysJA[0][0] = AC1_Days;
                    JToken[] AC1_RemarkJA = DataJsonJA.Where(z => z["que_id"].ToString() == "1039").Select(x => x["ans"]).ToArray();
                    AC1_RemarkJA[0][0] = AC1_Remark;
                }
                if (ac2lsit.Length == 3)
                {
                    AC2_Percent = ac2lsit[0].Trim();

                    AC2_Percent = AC2_Percent.Replace("%", "").Trim();
                    AC2_Days = ac2lsit[1].Trim();
                    AC2_Remark = ac2lsit[2].Trim();

                    AC2_Days = GetNumberFromString(AC2_Days).ToString();

                    JToken[] AC2_PercentJA = DataJsonJA.Where(z => z["que_id"].ToString() == "1040").Select(x => x["ans"]).ToArray();
                    AC2_PercentJA[0][0] = AC2_Percent;
                    JToken[] AC2_DaysJA = DataJsonJA.Where(z => z["que_id"].ToString() == "1041").Select(x => x["ans"]).ToArray();
                    AC2_DaysJA[0][0] = AC2_Days;
                    JToken[] AC2_RemarkJA = DataJsonJA.Where(z => z["que_id"].ToString() == "1042").Select(x => x["ans"]).ToArray();
                    AC2_RemarkJA[0][0] = AC2_Remark;
                }

                if (ac2lsit.Length == 3)
                {
                    AC3_Percent = ac3lsit[0].Trim();
                    AC3_Percent = AC3_Percent.Replace("%", "").Trim();
                    AC3_Days = ac3lsit[1].Trim();
                    AC3_Remark = ac3lsit[2].Trim();

                    AC3_Days = GetNumberFromString(AC3_Days).ToString();

                    //JToken[] AC3_PercentJA = DataJsonJA.Where(z => z["que_id"].ToString() == "1014").Select(x => x["ans"]).ToArray();
                    //AC3_PercentJA[0][0] = AC3_Percent;
                    //JToken[] AC3_DaysJA = DataJsonJA.Where(z => z["que_id"].ToString() == "1015").Select(x => x["ans"]).ToArray();
                    //AC3_DaysJA[0][0] = AC3_Days;
                    //JToken[] AC3_RemarkJA = DataJsonJA.Where(z => z["que_id"].ToString() == "1016").Select(x => x["ans"]).ToArray();
                    //AC3_RemarkJA[0][0] = AC3_Remark;
                }


                return DataJsonJA;
            }
            catch (Exception)
            {
                return DataJsonJA;
            }
        }


        public string SplitmanufactureSiteInfo_One(string manufactureSiteInfo)
        {

            string SiteID = "";

            try
            {

                string[] Split = manufactureSiteInfo.Split("<!>");
                SiteID = Split[0].Trim();
                return SiteID;
            }
            catch (Exception)
            {
                return SiteID;
            }
        }

        public string SplitmanufactureSiteInfo_Two(string manufactureSiteInfo)
        {

            string SiteID = "";

            try
            {

                string[] Split = manufactureSiteInfo.Split("<!>");
                SiteID = Split[1].Trim();
                return SiteID;
            }
            catch (Exception)
            {
                return SiteID;
            }
        }

        public string SplitmanufactureSiteInfo_Three(string manufactureSiteInfo)
        {

            string SiteID = "";

            try
            {

                string[] Split = manufactureSiteInfo.Split("<!>");
                SiteID = Split[2].Trim();
                return SiteID;
            }
            catch (Exception)
            {
                return SiteID;
            }
        }



        public JArray FillFinancialYear(string PublishDate)
        {
            JArray answe = new JArray();

            try
            {

                DateTime dateTime = DateTime.Parse(PublishDate);

                string ans = dateTime.ToString("yyyy/MM/dd");


                string type20payload = @"[{""flag"":1,""value_new"":[{""default_value"":[""""],""view_label"":""true"",""autofill"":""true"",""auto_select_first"":""false"",""self_assignment"":""false"",""qr"":""false"",""search"":""false"",""push_data"":[{""push_array"":[{""value"":""<@publish_date@>"",""type"":""text"",""ans"":""@FinancialYear@""},{""value"":""<@publish_date@>"",""type"":""text"",""ans"":""@FinancialYear@""}],""window_style"":""attached"",""view_style"":""dropdownview"",""customer_checksheetid"":""0"",""multi_select"":""false"",""ddl_info_view"":""true"",""select_all"":""true"",""add_cust_from_search"":""false"",""fetch_limit"":""50""}],""barcode"":""false"",""Checksheet_id"":""91238"",""columndata"":[{""checksheetID"":""91238"",""step"":""1"",""qid"":""12"",""header"":"" Circle"",""position"":""1"",""format"":""bold"",""font_size"":""8"",""color"":""""},{""checksheetID"":""91238"",""step"":""1"",""qid"":""1"",""header"":"" Financial Master ID"",""position"":""2"",""format"":""bold"",""font_size"":""8"",""color"":""""},{""checksheetID"":""91238"",""step"":""1"",""qid"":""15"",""header"":"" Financial Year"",""position"":""3"",""format"":""bold"",""font_size"":""8"",""color"":""""},{""checksheetID"":""91238"",""step"":""1"",""qid"":""16"",""header"":"" Start Date"",""position"":""4"",""format"":""bold"",""font_size"":""8"",""color"":""""},{""checksheetID"":""91238"",""step"":""1"",""qid"":""17"",""header"":"" End Date"",""position"":""5"",""format"":""bold"",""font_size"":""8"",""color"":""""}],""Fill_Data"":[],""Target_Id"":""CUSTOMER_ID"",""Target_Value"":""Financial Year"",""SearchID"":32106,""SearchName"":""FY from Master-1-1-1-1-1"",""Query_type"":""Formatted"",""ques_id"":20,""Current_Checksheet_id"":90935,""OFFSET"":0,""FETCH"":50,""searchValue"":""""}],""searchable_id"":""ddl_search_type20_0_90935_20"",""auth1"":[{""username"":""system.tanzania"",""u_c"":"""",""user_id"":""64132""}]}]";

                type20payload = type20payload.Replace("@FinancialYear@", ans);

                CallAPI CA = new CallAPI();

                string Returndate = CA.Type20API(JArray.Parse(type20payload));

                JArray Json_final_ = JArray.Parse(Returndate);

                string status = Json_final_[0]["status"].ToString();

                if (status == "1")
                {
                    #region  Status1
                    JArray Message = JArray.Parse(Json_final_[0]["data"].ToString());
                    foreach (JObject content in Message.Children<JObject>())
                    {
                        List<string> keys = content.Properties().Select(p => p.Name).ToList();


                        List<string> list = new List<string>();
                        int ID = 0;
                        string name = "";
                        for (int ii = 0; ii < keys.Count; ii++)
                        {
                            if (ii == 0)
                            {
                                ID = Convert.ToInt32(Message[0].ToObject<JObject>().GetValue(keys[ii]).ToString());
                            }

                            if (ii == 1)
                            {
                                name = Message[0].ToObject<JObject>().GetValue(keys[ii]).ToString();
                                //name = Controle_Obj["ans"][0].ToString().TrimStart().TrimEnd();
                            }
                        }
                        list.Add(name);
                        list.Add(ID.ToString());
                        List<string> list_ans = new List<string>
                                        {
                                            name,
                                            ID.ToString()
                                        };
                        answe = (JArray)JToken.FromObject(list_ans);
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return answe;
        }

        public JArray FillCountry()
        {
            JArray answe = new JArray();
            try
            {

                string type20payload = @"[{""flag"":1,""value_new"":[{""default_value"":[""""],""view_label"":""true"",""autofill"":""true"",""auto_select_first"":""false"",""self_assignment"":""false"",""qr"":""false"",""search"":""false"",""push_data"":[{""push_array"":[{""value"":""<#user_id#>"",""type"":""id"",""ans"":""64132""}],""window_style"":""attached"",""view_style"":""dropdownview"",""customer_checksheetid"":""0"",""multi_select"":""false"",""ddl_info_view"":""true"",""select_all"":""true"",""add_cust_from_search"":""false"",""fetch_limit"":""50""}],""barcode"":""false"",""Checksheet_id"":""85117"",""columndata"":[],""Fill_Data"":[],""Target_Id"":""USER_ID"",""Target_Value"":""Country"",""SearchID"":33687,""SearchName"":""Country_User_wise"",""Query_type"":""Formatted"",""ques_id"":13,""Current_Checksheet_id"":90935,""OFFSET"":0,""FETCH"":50,""searchValue"":""""}],""searchable_id"":""ddl_search_type20_0_90935_13"",""auth1"":[{""username"":""system.tanzania"",""u_c"":"""",""user_id"":""64132""}]}]";


                CallAPI CA = new CallAPI();

                string Returndate = CA.Type20API(JArray.Parse(type20payload));

                JArray Json_final_ = JArray.Parse(Returndate);

                string status = Json_final_[0]["status"].ToString();

                if (status == "1")
                {
                    #region  Status1
                    JArray Message = JArray.Parse(Json_final_[0]["data"].ToString());

                    for (int i = 0; i < Message.Count; i++)
                    {

                        var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(Message[i].ToString());

                        if (data != null && data.Count >= 2)
                        {
                            var keys = new List<string>(data.Keys);
                            var firstKey = keys[0];
                            var secondKey = keys[1];
                            
                            JObject jo = new JObject
                            {
                                { "Name", data[secondKey]} ,
                                { "ID", data[firstKey]}
                            };

                            answe.Add(jo);

                        }

                    }
                    #endregion


                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return answe;
        }


        public string POCallationAPICall(int task_id)
        {
            string JobID = "";
            try
            {
                JObject joMainData = new JObject
                            {
                                { "flowId", 4901 },
                                { "taskId", task_id },
                                { "Checksheet_id", 90967 }
                            };

                CallAPI AP = new CallAPI();

                JobID = AP.CoreAPI("https://Dev.easyform.in/HangfireApp/"
                    , "api/Assign/CalculationFieldEnqueue"
                    , "POST"
                    , JsonConvert.SerializeObject(joMainData)
                    , true);





            }
            catch (Exception)
            {

                throw;
            }

            return JobID;


        }


        static int GetNumberFromString(string input)
        {
            // Regular expression to match a number (positive or negative)
            Regex regex = new Regex(@"\d+");
            Match match = regex.Match(input);

            // If a match is found, parse the number; otherwise, return 0
            return match.Success ? int.Parse(match.Value) : 0;
        }

    }
}
