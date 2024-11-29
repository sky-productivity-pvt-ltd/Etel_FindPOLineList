
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FindPOLineList
{
    public class ErrorHandling
    {
        public bool WriteErrorLog(Exception ex, string parameterJson, string authJson, string extraRemark)
        {
            bool Status = false;
            try
            {
                string logLine = InsertInDB(ex, parameterJson, authJson, extraRemark);
            }
            catch (Exception ex1)
            {
            }
            return Status;
        }

        private string InsertInDB(Exception ex, string parameterJson, string authJson, string extraRemark)
        {
            string exceptionMessage = ex.Message;
            string stackTrace = ex.StackTrace;
            string source = ex.Source;
            string targetSite = ex.TargetSite.ToString();

            StackTrace st = new StackTrace(ex, true);
            StackFrame sf = st.GetFrame(0);

            string fileName = sf.GetFileName();
            string fn = "";
            if (fileName != null)
            {
                string[] pathArr = fileName.Split('\\');
                fn = pathArr.Last().ToString();
            }

            string methodName = "";
            if (string.IsNullOrEmpty(sf.GetMethod().Name)) { methodName = ""; } else { methodName = sf.GetMethod().Name; }
            int fileLineNumber = 0;
            if (string.IsNullOrEmpty(sf.GetFileLineNumber().ToString())) { fileLineNumber = 0; } else { fileLineNumber = sf.GetFileLineNumber(); }
            int fileColumnNumber = 0;
            if (string.IsNullOrEmpty(sf.GetFileColumnNumber().ToString())) { fileColumnNumber = 0; } else { fileColumnNumber = sf.GetFileColumnNumber(); }

            DatabaseFile db = new DatabaseFile();
            JArray Ja_Main = new JArray
            {
                commanjarray.CommanJobject("FileName",fn),
                commanjarray.CommanJobject("MethodName",methodName),
                commanjarray.CommanJobject("FileLineNumber",fileLineNumber.ToString()),
                commanjarray.CommanJobject("FileColumnNumber",fileColumnNumber.ToString()),
                commanjarray.CommanJobject("ExceptionMessage",exceptionMessage),
                commanjarray.CommanJobject("StackTrace",stackTrace),
                commanjarray.CommanJobject("Source",source),
                commanjarray.CommanJobject("TargetSite",targetSite),
                commanjarray.CommanJobject("ParameterJson",parameterJson),
                commanjarray.CommanJobject("AuthJson",authJson),
                commanjarray.CommanJobject("ExtraRemark",extraRemark),
                commanjarray.CommanJobject("OccurredFrom","Hangfire_Consol"),
                commanjarray.CommanJobject("UserId","0")
            };
            db.OnlyInsert("InsertInErrorMaster", Ja_Main);
            //DA.InsertInErrorMaster(fn, methodName, fileLineNumber, fileColumnNumber, extraRemark, stackTrace, source, targetSite, parameterJson, authJson, "Job_Schedule");

            return "";

        }

    }
}
