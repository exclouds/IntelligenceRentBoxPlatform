using Magicodes.Admin.IMChat.Dto;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Web.UI.Util;

namespace Magicodes.Admin.IMChat
{
    public class IMChatAppService : AdminAppServiceBase
    {
        /// <summary>
        /// 新增客服会话
        /// </summary>
        /// <param name="input"></param>
        public bool AddServerChat(IMServerEnInputDto input)
        {
            string sql = "insert into IMServerEn(ServerChatId,ServerChatName,State,AccessTime,CreationTime,CreatorUserId,IsDeleted)"
            + "values(@ServerChatId, @ServerChatName, @State, @AccessTime, @CreationTime, @CreatorUserId, 0)";
            SqlParameter[] cmdParms = new SqlParameter[] {
                new SqlParameter("@ServerChatId", SqlDbType.Int),
                new SqlParameter("@ServerChatName", SqlDbType.VarChar, 20),
                new SqlParameter("@State", SqlDbType.VarChar, 10),
                new SqlParameter("@AccessTime", SqlDbType.DateTime),
                new SqlParameter("@CreationTime", SqlDbType.DateTime),
                new SqlParameter("@CreatorUserId", SqlDbType.Int)
            };
            cmdParms[0].Value = input.ServerChatId;
            cmdParms[1].Value = input.ServerChatName;
            cmdParms[2].Value = input.State;
            cmdParms[3].Value = input.AccessTime;
            cmdParms[4].Value = DateTime.Now;
            cmdParms[5].Value = input.ServerChatId;
            return DbHelperSQL.ExecuteSql(sql, cmdParms) > 0;
        }
        /// <summary>
        /// 新增会话窗口
        /// </summary>
        /// <param name="input"></param>
        public bool AddClientChat(IMClientEnInputDto input)
        {
            string sql = "insert into IMClientEn(ClientChatId,ClientChatName,State,AccessTime,InputContent,NewMsgCount,IsFollow,"
            + "LastMsgTime,LastMsgContent,LastMsgShowTime,CreationTime,CreatorUserId,IsDeleted,BillNO,ServerChatId)"
            + "values(@ClientChatId, @ClientChatName, @State, @AccessTime, @InputContent, @NewMsgCount, 0,"
            + "@LastMsgTime, @LastMsgContent, @LastMsgShowTime, @CreationTime, @CreatorUserId, 0, @BillNO, @ServerChatId)";
            SqlParameter[] cmdParms = new SqlParameter[] { 
                new SqlParameter("@ClientChatId", SqlDbType.Int),
                new SqlParameter("@ClientChatName", SqlDbType.VarChar, 20),
                new SqlParameter("@State", SqlDbType.VarChar, 10),
                new SqlParameter("@AccessTime", SqlDbType.DateTime),
                new SqlParameter("@InputContent", SqlDbType.VarChar, 500),
                new SqlParameter("@NewMsgCount", SqlDbType.Int),
                new SqlParameter("@LastMsgTime", SqlDbType.DateTime),
                new SqlParameter("@LastMsgContent", SqlDbType.VarChar, 500),
                new SqlParameter("@LastMsgShowTime", SqlDbType.DateTime),
                new SqlParameter("@CreationTime", SqlDbType.DateTime),
                new SqlParameter("@CreatorUserId", SqlDbType.Int),
                new SqlParameter("@BillNO", SqlDbType.VarChar, 50),
                new SqlParameter("@ServerChatId", SqlDbType.Int)
            };
            cmdParms[0].Value = input.ClientChatId;
            cmdParms[1].Value = input.ClientChatName;
            cmdParms[2].Value = input.State;
            cmdParms[3].Value = input.AccessTime;
            cmdParms[4].Value = input.InputContent;
            cmdParms[5].Value = input.NewMsgCount;
            cmdParms[6].Value = input.LastMsgTime;
            cmdParms[7].Value = input.LastMsgContent;
            cmdParms[8].Value = input.LastMsgShowTime;
            cmdParms[9].Value = DateTime.Now;
            cmdParms[10].Value = input.ServerChatId;
            cmdParms[11].Value = input.BillNO;
            cmdParms[12].Value = input.ServerChatId;
            return DbHelperSQL.ExecuteSql(sql, cmdParms) > 0;
        }

        public bool UpdateClientChat(long clientChatId,string column,string value)
        {
            string sql = "";
            SqlParameter[] cmdParms = null;
            if (column == "state")
            {
                sql = "update IMChatEn set State=@Val, LastModificationTime=@LastModificationTime, LastModifierUserId=@LastModifierUserId where ClientChatId=@ClientChatId";
                cmdParms = new SqlParameter[] {
                    new SqlParameter("@Val", SqlDbType.VarChar, 10),
                    new SqlParameter("@LastModificationTime", SqlDbType.DateTime),
                    new SqlParameter("@LastModifierUserId", SqlDbType.Int),
                    new SqlParameter("@ClientChatId", SqlDbType.Int),
                };
                cmdParms[0].Value = value;
                cmdParms[1].Value = DateTime.Now;
                cmdParms[2].Value = clientChatId;
                cmdParms[3].Value = clientChatId;
                
            }
            else if (column == "newMsgCount")
            {
                sql = "update IMChatEn set NewMsgCount=@Val, LastModificationTime=@LastModificationTime, LastModifierUserId=@LastModifierUserId where ClientChatId=@ClientChatId";
                cmdParms = new SqlParameter[] {
                    new SqlParameter("@Val", SqlDbType.Int),
                    new SqlParameter("@LastModificationTime", SqlDbType.DateTime),
                    new SqlParameter("@LastModifierUserId", SqlDbType.Int),
                    new SqlParameter("@ClientChatId", SqlDbType.Int),
                };
                cmdParms[0].Value = value;
                cmdParms[1].Value = DateTime.Now;
                cmdParms[2].Value = clientChatId;
                cmdParms[3].Value = clientChatId;
            }
            return DbHelperSQL.ExecuteSql(sql, cmdParms) > 0;
        }

        /// <summary>
        /// 新增会话消息
        /// </summary>
        /// <param name="input"></param>
        public bool AddChatMsg(IMChatMsgInputDto input)
        {
            string sQLString = "insert into IMChatMsg(ClientChatId,Role,ContentType,[Content],IsNewMsg,"
            + "CreationTime,CreatorUserId,IsDeleted)"
            + "values(@ClientChatId, @Role, @ContentType, @Content, 1,"
            + "@CreationTime, @CreatorUserId, 0)";
            SqlParameter[] cmdParms = new SqlParameter[] {
                new SqlParameter("@ClientChatId", SqlDbType.Int),
                new SqlParameter("@Role", SqlDbType.VarChar, 20),
                new SqlParameter("@ContentType", SqlDbType.VarChar, 20),
                new SqlParameter("@Content", SqlDbType.VarChar, 500),
                new SqlParameter("@CreationTime", SqlDbType.DateTime),
                new SqlParameter("@CreatorUserId", SqlDbType.Int)
            };
            cmdParms[0].Value = input.ClientChatId;
            cmdParms[1].Value = input.Role;
            cmdParms[2].Value = input.ContentType;
            cmdParms[3].Value = input.Content;
            cmdParms[4].Value = DateTime.Now;
            cmdParms[5].Value = input.ClientChatId;
            return DbHelperSQL.ExecuteSql(sQLString, cmdParms) > 0;
        }

        public ClientCountListDto GetClientCount(int serverChatId)
        {
            ClientCountListDto dto = new ClientCountListDto();
            string sql = "select count(*) clientCount from IMClientEn where [state]='on' and ServerChatId='" + serverChatId + "'";
            DataSet ds = DbHelperSQL.Query(sql, null);
            if (ds != null)
            {
                dto.ServerChatId = serverChatId;
                dto.ClientCount = Convert.ToInt32(ds.Tables[0].Rows[0]["clientCount"]);
            }
            return dto;
        }
    }
}
