using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FindPOLineList
{
    public class DatabaseFile
    {
        private readonly SqlDataReader myReader_1;

        public static string sqlDataSource = "Data Source=103.233.76.87;Initial Catalog=EasyForm_Finance;Persist Security Info=True;User ID=sa;Password=SkyProd@49;Connect Timeout=6000;Min Pool Size=0;Max Pool Size=1500;Pooling=true;";

        public DataTable GetData(string str, JArray parameters)
        {
            return MSSQLGetData(str, parameters);
        }

        public DataTable MSSQLGetData(string str, JArray parameters)
        {



            DataTable objresutl = new DataTable();
            try
            {
                SqlDataReader myReader;

                using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                {
                    myCon.Open();
                    using (SqlCommand myCommand = new SqlCommand(str, myCon))
                    {

                        myCommand.CommandType = CommandType.StoredProcedure;
                        for (int i = 0; i < parameters.Count; i++)
                        {

                            myCommand.Parameters.AddWithValue(string.Concat("@", parameters[i]["key"].ToString()), parameters[i]["value"].ToString());
                        }

                        myReader = myCommand.ExecuteReader();
                        objresutl.Load(myReader);
                        //Get_SP_List(str);
                        myReader.Close();

                        myCon.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandling ErrorHandle = new ErrorHandling(); ErrorHandle.WriteErrorLog(ex, str + "_" + parameters, "", GetType().Name + " - " + MethodBase.GetCurrentMethod().Name);
                throw ex;
            }

            return objresutl;

        }

        public void OnlyInsert(string str, JArray parameters)
        {
            MsSqlOnlyInsert(str, parameters);
        }
        public void MsSqlOnlyInsert(string str, JArray parameters)
        {

            try
            {
                using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                {
                    myCon.Open();
                    using (SqlCommand myCommand = new SqlCommand(str, myCon))
                    {
                        myCommand.CommandType = CommandType.StoredProcedure;
                        for (int i = 0; i < parameters.Count; i++)
                        {
                            myCommand.Parameters.AddWithValue(string.Concat("@", parameters[i]["key"].ToString()), parameters[i]["value"].ToString());
                        }
                        myCommand.ExecuteNonQuery();

                    }
                }
            }
            catch (Exception ex)
            {
            }

        }
    }
}
