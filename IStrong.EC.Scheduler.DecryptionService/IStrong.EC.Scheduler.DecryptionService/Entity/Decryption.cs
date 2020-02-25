/************************************************************************************************************
* file    : Decryption.cs
* author  : DESKTOP-7LOFD1V
* function: 
* history : created by DESKTOP-7LOFD1V 2019/12/30 10:28:39
************************************************************************************************************/
using IStrong.EC.Abstractions.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace IStrong.EC.Scheduler.DecryptionService.Entity
{
    /// <summary>
    /// 解密实体类
    /// </summary>
    public class Decryption : IEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int RowId { get; set; }

        /// <summary>
        /// 未解密纬度
        /// </summary>
        public double WD { get; set; }


        /// <summary>
        /// 未解密经度
        /// </summary>
        public double JD { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime updatetime { get; set; }

        /// <summary>
        /// 解密ID
        /// </summary>
        public long ID { get; set; }

        /// <summary>
        /// 解密后纬度
        /// </summary>
        public double WD_T { get; set; }

        /// <summary>
        /// 解密后经度
        /// </summary>
        public double JD_T { get; set; }
    }
}
