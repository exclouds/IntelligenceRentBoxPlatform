using Magicodes.Admin;
using Magicodes.Admin.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Application.Custom.API.PublicArea.PubContactNO
{
    public class ContactNOAPPService: AppServiceBase, IContactNOAPPService
    {
        private readonly IConfigurationRoot _appConfiguration;
        public ContactNOAPPService(IHostingEnvironment env)
        {
            _appConfiguration = env.GetAppConfiguration();
        }

        /// <summary>
        /// 单号生成
        /// </summary>
        /// <param name="type">业务类型</param>
        /// <returns></returns>
        public async Task<string> GetBusNO(string type)
        {
            string NO = null;
            var connStr = _appConfiguration["ConnectionStrings:Default"].ToString();
            using (SqlConnection conn = new SqlConnection(connStr))
            {

                SqlCommand cmd = new SqlCommand("SP_ContactNO", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@type", type);  //给输入参数赋值  
                cmd.Parameters.AddWithValue("@creatorUserId", AbpSession.UserId);  //给输入参数赋值  
                cmd.Parameters.AddWithValue("@tenantId", AbpSession.TenantId);  //给输入参数赋值  
                SqlParameter parOutput = cmd.Parameters.Add("@flowNo", SqlDbType.VarChar, 14);  //定义输出参数  
                parOutput.Direction = ParameterDirection.Output;
                conn.Open();
                cmd.ExecuteNonQuery();
                NO = parOutput.Value.ToString();
            }
            return NO;
        }
    }
}
