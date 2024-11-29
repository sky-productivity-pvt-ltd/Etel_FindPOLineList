// See https://aka.ms/new-console-template for more information

using FindPOLineList;
using Microsoft.VisualBasic;
using Newtonsoft.Json.Linq;
using SuperConvert.Extensions;
using System.Data;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json.Nodes;


Console.WriteLine("Hello, World!");

JArray apiResult = new JArray();

try
{

    string ponumber = "4241HG2883539-18 ";
    int i = 1;
    while (i > 0 && i < 17)
    {


        


        string url = String.Concat("https://apigw-scs.huawei.com/api/service/esupplier/findPoLineList/1.0.0?suffix_path=/200/", i);
        var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Headers.Add("X-HW-ID", "APP_Z06WR2_B2B");
        request.Headers.Add("X-HW-APPKEY", "Ou9LhCTTwH/LkJM/EqOSCw==");
        request.Headers.Add("Cookie", "ztsg_ruuid=44530fd9df2012e1-22e1-4af2-8319-701b292d2dfe");
        var content = new StringContent("{\r\n    \"poSubType\": \"E\",\r\n    \"combo2\": \"all\",\r\n    \"combo3\": \"all\",\r\n    \"colTaskOrPoStatus\": \"all\",\r\n    \"statusType\": \"COL_TASK_STATUS\",\r\n    \"includeVCICA\": -1,\r\n    \"startIndex\": 21\r\n        \r\n}", null, "application/json");

        //string paylod = "{\r\n    \"poSubType\": \"E\",\r\n    \"combo2\": \"all\",\r\n    \"combo3\": \"all\",\r\n    \"poNumber\": \"@ponumber@\",\r\n    \"colTaskOrPoStatus\": \"all\",\r\n    \"statusType\": \"COL_TASK_STATUS\",\r\n    \"includeVCICA\": -1,\r\n    \"startIndex\": 21\r\n        \r\n}";

        //paylod = paylod.Replace("@ponumber@", ponumber);
        //var content = new StringContent(paylod, null, "application/json");
        
        
        request.Content = content;
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        Console.WriteLine("API Call - " + i);
        string XYZ = response.Content.ReadAsStringAsync().Result;

        JObject xyz_ = JObject.Parse(XYZ);
        JArray innerResult_ = JArray.Parse(xyz_["result"].ToString());
        E_TelTaskCreation Etask = new E_TelTaskCreation();
        innerResult_ = Etask.E_Tel_ParentTaskCreation(innerResult_);

        for (var j = 0; j < innerResult_.Count; j++)
        {
            innerResult_[j]["objectChangeContext"] = "";

            apiResult.Add(innerResult_[j]);
        }


        i++;
    }
    long milliseconds = DateTimeOffset.Now.ToUnixTimeMilliseconds();

     string csvPath = apiResult.ToString().ToCsv("D:\\19-09-2024", String.Concat("FindPOLineItems", "_" + ponumber + "_", milliseconds), ',');

   // string csvPath = apiResult.ToString().ToCsv("D:\\19-09-2024", String.Concat("FindPOLineItems", "_", milliseconds),',');
    Console.WriteLine(csvPath);
}
catch (Exception ex)
{
    throw ex;
}
