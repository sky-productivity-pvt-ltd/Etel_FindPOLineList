using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FindPOLineList
{
    public class ThreeKeyLogic
    {
        public static string ThreeKeyLogicConvert(JArray Data)
        {
            try
            {
                var New_Data = (from e in Data
                                where e["type"].ToString() != "10"
                                select new JObject
                        {
                            new JProperty("type", e["type"]),
                            new JProperty("que_id", e["que_id"]),
                            new JProperty("ans", ThreeKey_Type22(e["ans"]))

                        }).ToList();


                return JsonConvert.SerializeObject(New_Data);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public static JToken ThreeKey_Type22(JToken AnsData)
        {
            try
            {
                if (AnsData.ToString() != "" && AnsData.ToString() != "\"\"")
                {
                    if (AnsData.ToString().Contains("json_type_22"))
                    {
                        JArray Rowrow_finaljson;
                        try
                        {

                            Rowrow_finaljson = JArray.Parse(AnsData[0].ToString());
                        }
                        catch
                        {

                            Rowrow_finaljson = JArray.Parse(AnsData.ToString());
                        }
                        //JArray Rowrow_finaljson = JArray.Parse(AnsData[0].ToString());
                        int json_type_22cnt = Rowrow_finaljson[0]["row_finaljson"].Count();
                        for (int i = 0; i < json_type_22cnt; i++)
                        {
                            JArray edt_btn_json = JArray.Parse(Rowrow_finaljson[0]["row_finaljson"][i]["json_type_22"][0]["edt_btn_json"].ToString());
                            var New_Data = (from e in edt_btn_json
                                            select new JObject
                        {
                            new JProperty("type", e["type"]),
                            new JProperty("que_id", e["que_id"]),
                            new JProperty("ans", e["ans"])
                                            }).ToList();

                            JArray jo = JArray.Parse(JsonConvert.SerializeObject(New_Data));
                            Rowrow_finaljson[0]["row_finaljson"][i]["json_type_22"][0]["edt_btn_json"][0] = jo[0];
                        }
                        AnsData[0] = Rowrow_finaljson;
                        return AnsData;
                    }
                    else
                    {
                        return AnsData;
                    }
                }
                else
                {
                    return AnsData;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return AnsData;
        }
    }
}
