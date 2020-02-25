/************************************************************************************************************
* file    : DecryptionRepository.cs
* author  : DESKTOP-7LOFD1V
* function: 
* history : created by DESKTOP-7LOFD1V 2019/12/30 10:38:37
************************************************************************************************************/
using System;
using IStrong.EC.Abstractions.Interfaces;
using IStrong.EC.DAO.Abstractions.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Linq;
using IStrong.EC.Scheduler.DecryptionService.Entity;
using System.Collections.Generic;
using IStrong.EC.DAO.Abstractions.Entity;
using System.Text;

namespace IStrong.EC.Scheduler.DecryptionService.Repository
{
    /// <summary>
    /// 数据访问类
    /// </summary>
    public class DecryptionRepository : IService
    {
        /// <summary>
        /// 数据库操作组件
        /// </summary>
        private readonly IDbContext _db;

        /// <summary>
        /// 配置信息
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// 日志组件
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// 注入服务 
        /// </summary>
        /// <param name="db"></param>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        public DecryptionRepository(IDbContext db, ILogger logger, IConfiguration configuration)
        {
            _db = db;
            _logger = logger;
            _configuration = configuration;
        }

        /// <summary>
        /// 获取台风预报路径要解密的数据
        /// </summary>
        /// <param name="updatetime">上次更新的时间</param>
        /// <returns></returns>
        public IEnumerable<Decryption> getDecryPtionDataTFYBLJ(DateTime updatetime)
        {
            return _db.Query<Decryption>("DecryptionData.s_decryDataTFYBLJ", new { updatetime = updatetime.ToString("yyyy-MM-dd HH:mm:ss.fff")  });
        }

        /// <summary>
        /// 获取台风路径要解密的数据
        /// </summary>
        /// <param name="updatetime">上次更新的时间</param>
        /// <returns></returns>
        public IEnumerable<Decryption> getDecryPtionDataTFLSLJ(DateTime updatetime)
        {
            return _db.Query<Decryption>("DecryptionData.s_decryDataTFLSLJ", new { updatetime = updatetime.ToString("yyyy-MM-dd HH:mm:ss.fff") });
        }


        /// <summary>
        /// 往F02_TFLSLJ数据表中插入解密之后的数据
        /// </summary>
        /// <param name="decryptions">数据</param>
        public void updateTFLSLJ(IEnumerable<Decryption> decryptions)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var decryption in decryptions)
            {
                sb.Append($" UPDATE F02_TFLSLJ SET WD_T = {decryption.WD_T},JD_T = {decryption.JD_T} WHERE RowId = {decryption.RowId} ");
            }
            var sql = sb.ToString();
            sb = null;
            _db.Execute("Conn", sql);
        }

        /// <summary>
        /// 往F02_TFYBLJ数据表中插入解密之后的数据
        /// </summary>
        /// <param name="decryptions">数据</param>
        public void updateTFYBLJ(IEnumerable<Decryption> decryptions)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var decryption in decryptions)
            {
                sb.Append($" UPDATE F02_TFYBLJ SET WD_T = {decryption.WD_T},JD_T = {decryption.JD_T} WHERE RowId = {decryption.RowId} ");
            }
            var sql = sb.ToString();
            sb = null;
            _db.Execute("Conn", sql);
        }

        /// <summary>
        /// 获取最新的配置时间
        /// </summary>
        /// <param name="tbName">表名</param>
        /// <returns></returns>
        public string getConfigTime(string tbName)
        {
            string sql = $"SELECT updatetime FROM {tbName} ORDER BY updatetime DESC";
            return _db.Query<DateTime>("Conn", sql).FirstOrDefault().ToString("yyyy-MM-dd HH:mm:ss.fff");
        }

    }
}
