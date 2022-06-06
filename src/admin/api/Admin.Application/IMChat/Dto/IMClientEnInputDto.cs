using System;
using System.Collections.Generic;
using System.Text;

namespace Magicodes.Admin.IMChat.Dto
{
    public class IMClientEnInputDto
    {
        public int Id { get; set; }
        /// <summary>
        /// 会话Id (客户端用户ID)
        /// </summary>
        public long ClientChatId { get; set; }
        /// <summary>
        /// 会话名称 
        /// </summary>
        public string ClientChatName { get; set; }
        /// <summary>
        /// 客服Id
        /// </summary>
        public long ServerChatId { get; set; }
        /// <summary>
        /// 在线状态 ：on 在线；off离线
        /// </summary>
        public string State { get; set; }
        /// <summary>
        /// 访问时间
        /// </summary>
        public string AccessTime { get; set; }
        /// <summary>
        /// 单号
        /// </summary>
        public string BillNO { get; set; }
        /// <summary>
        /// 会话框内容
        /// </summary>
        public string InputContent { get; set; }
        /// <summary>
        /// 新消息数量
        /// </summary>
        public int NewMsgCount { get; set; }
        /// <summary>
        /// 是否关注（已关注的排在最前面并按最后一次时间倒序）
        /// </summary>
        public bool IsFollow { get; set; }
        /// <summary>
        /// 最新一条消息时间
        /// </summary>
        public DateTime LastMsgTime { get; set; }
        /// <summary>
        /// 最新消息内容 (显示在会话列表)
        /// </summary>
        public string LastMsgContent { get; set; }
        /// <summary>
        /// 最后一个消息的显示时间
        /// </summary>
        public DateTime LastMsgShowTime { get; set; }
        public DateTime CreationTime { get; set; }
        public int CreatorUserId { get; set; }
        public DateTime LastModificationTime { get; set; }
        public int LastModifierUserId { get; set; }
        public bool IsDeleted { get; set; }
        public int DeleterUserId { get; set; }
        public int DeletionTime { get; set; }
    }
}
