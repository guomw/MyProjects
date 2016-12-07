/*
 * 版权所有:杭州火图科技有限公司
 * 地址:浙江省杭州市滨江区西兴街道阡陌路智慧E谷B幢4楼在地图中查看
 * (c) Copyright Hangzhou Hot Technology Co., Ltd.
 * Floor 4,Block B,Wisdom E Valley,Qianmo Road,Binjiang District
 * 2013-2016. All rights reserved.
 * author guomw
**/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HotCoreUtils.Helper
{
    /// <summary>
    /// TODO:文章评论助手,使用此帮助类，必须存在hot_comment_list数据表
    /// </summary>
    public class CommentHelper
    {

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        /// <value>The connection string.</value>
        public string connectionString { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public CommentHelper()
        {
            connectionString = ConfigHelper.MssqlDBConnectionString;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="conStr">数据库连接字符串</param>
        public CommentHelper(string conStr)
        {
            connectionString = conStr;
        }

        /// <summary>
        /// 添加评论
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>System.Int32.</returns>
        public int AddComment(CommentModel model)
        {
            if (model == null || model.comment_user_id == 0 || model.article_id == 0 || string.IsNullOrEmpty(model.comment_user_name) || string.IsNullOrEmpty(model.commnet_content)) return 0;

            return 0;
        }


        /// <summary>
        /// 回复评论
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>System.Int32.</returns>
        public int AddReplyComment(ReplyCommentModel model)
        {
            if (model == null || model.reply_comment_id == 0 || model.comment_user_id == 0 || model.reply_user_id == 0 || string.IsNullOrEmpty(model.comment_user_name) || string.IsNullOrEmpty(model.reply_user_name) || string.IsNullOrEmpty(model.reply_commnet_content)) return 0;

            return 0;
        }


        /// <summary>
        /// 添加点赞
        /// </summary>
        /// <param name="commentId">点赞的评论ID</param>
        /// <param name="userId">点赞用户ID</param>
        /// <returns>true or false</returns>
        public bool AddLikeComment(int commentId, int userId)
        {
            return false;
        }

    }


    /// <summary>
    /// 评论对象实体
    /// </summary>
    public class CommentModel
    {
        /// <summary>
        /// 评论ID
        /// </summary>
        /// <value>The comment_id.</value>
        public int comment_id { get; set; }


        /// <summary>
        /// 文章ID
        /// </summary>
        /// <value>The article_id.</value>
        public int article_id { get; set; }


        /// <summary>
        /// 评论用户ID
        /// </summary>
        /// <value>The comment_user_id.</value>
        public int comment_user_id { get; set; }



        /// <summary>
        /// 评论用户名称
        /// </summary>
        /// <value>The comment_user_name.</value>
        public string comment_user_name { get; set; }


        /// <summary>
        /// 回复的评论ID
        /// </summary>
        /// <value>The reply_comment_id.</value>
        public int reply_comment_id { get; set; }

        /// <summary>
        /// 回复评论用户的ID
        /// </summary>
        /// <value>The reply_user_id.</value>
        public int reply_user_id { get; set; }

        /// <summary>
        /// 回复评论用户的名称
        /// </summary>
        /// <value>The reply_user_name.</value>
        public string reply_user_name { get; set; }

        /// <summary>
        /// 回复时间
        /// </summary>
        /// <value>The reply_time.</value>
        public DateTime reply_time { get; set; }


        /// <summary>
        /// 评论或回复内容
        /// </summary>
        /// <value>The commnet_content.</value>
        public string commnet_content { get; set; }



        /// <summary>
        /// 当前评论的回复数
        /// </summary>
        /// <value>The reply_count.</value>
        public int reply_count { get; set; }



        /// <summary>
        /// 当前评论的点赞数
        /// </summary>
        /// <value>The like_count.</value>
        public int like_count { get; set; }

        /// <summary>
        /// 是否是热点评论
        /// </summary>
        /// <value>The hot_comment.</value>
        public bool hot_comment { get; set; }


        /// <summary>
        /// 创建时间
        /// </summary>
        /// <value>The create_time.</value>
        public DateTime create_time { get; set; }


        /// <summary>
        /// 友好的提示时间
        /// </summary>
        /// <value>The friendly_prompt_time.</value>
        public string friendly_prompt_time { get; set; }



    }


    /// <summary>
    /// 回复评论实体对象
    /// </summary>
    public class ReplyCommentModel
    {

        /// <summary>
        /// 评论用户ID
        /// </summary>
        /// <value>The comment_user_id.</value>
        public int comment_user_id { get; set; }

        /// <summary>
        /// 评论用户名称
        /// </summary>
        /// <value>The comment_user_name.</value>
        public string comment_user_name { get; set; }

        /// <summary>
        /// 回复的评论ID
        /// </summary>
        /// <value>The reply_comment_id.</value>
        public int reply_comment_id { get; set; }

        /// <summary>
        /// 回复评论用户的ID
        /// </summary>
        /// <value>The reply_user_id.</value>
        public int reply_user_id { get; set; }

        /// <summary>
        /// 回复评论用户的名称
        /// </summary>
        /// <value>The reply_user_name.</value>
        public string reply_user_name { get; set; }

        /// <summary>
        /// 回复内容
        /// </summary>
        /// <value>The commnet_content.</value>
        public string reply_commnet_content { get; set; }
    }

}
