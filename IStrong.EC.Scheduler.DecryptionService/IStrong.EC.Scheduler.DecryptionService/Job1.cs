using IStrong.EC.TaskManager.Abstractions;
using IStrong.EC.TaskManager;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Linq;
using IStrong.EC.Scheduler.DecryptionService.Repository;
using System.IO;
using IStrong.EC.Scheduler.DecryptionService.Entity;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace IStrong.EC.Scheduler.DecryptionService
{
    //如需禁止job并行执行，可开启以下特性
    //[DisallowConcurrentExecution]
    public class Job1 : JobBase
    {
        private ILogger _logger;

        private IJobExecutionContext _context;

        /// <summary>
        /// 路径表更新时间
        /// </summary>
        private string updateTimeTFLSLJ;

        /// <summary>
        /// 预报表更新时间
        /// </summary>
        private string updateTimeTFYBLJ;

        /// <summary>
        /// 数据仓库
        /// </summary>
        private DecryptionRepository _repository;

        /// <summary>
        /// 用于解密
        /// </summary>
        private string[] Key = new string[10];

        /// <summary>
        /// 时间文件路径
        /// </summary>
        private string configPath;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="job"></param>
        /// <param name="logger"></param>
        public override void ExecuteJob(IJobExecutionContext context, Job job, ILogger logger)
        {
            _logger = logger;
            _context = context;
            run();
        }

        /// <summary>
        /// 主程序
        /// </summary>
        private void run()
        {
            try
            {
                _logger.LogInformation("*************************解密服务开始*************************");
                //获取配置文件信息
                readConfig();
                DateTime timeTFLSLJ = Convert.ToDateTime(updateTimeTFLSLJ);
                DateTime timeTFYBLJ = Convert.ToDateTime(updateTimeTFYBLJ);

                //解析密码并获取已更新最新的时间
                _logger.LogInformation($"解析F02_TFLSLJ数据开始，此次解析开始时间{timeTFLSLJ}");
                var newTimeTFLSLJ = decryTFLSLJ(timeTFLSLJ);
                _logger.LogInformation($"解析F02_TFLSLJ数据结束，此次解析直到时间{newTimeTFLSLJ}");

                _logger.LogInformation($"解析F02_TFYBLJ数据开始，此次解析开始时间{timeTFYBLJ}");
                var newTimeTFYBLJ = decryTFYBLJ(timeTFYBLJ);
                _logger.LogInformation($"解析F02_TFYBLJ数据结束，此次解析直到时间{newTimeTFYBLJ}");

                //将已经更新的时间写入到配置文件中
                File.WriteAllText(configPath, $"TFLSLJ:{newTimeTFLSLJ}\r\nTFYBLJ:{newTimeTFYBLJ}");
                _logger.LogInformation("*************************解密服务结束*************************");
            }
            catch (Exception ex)
            {

                _logger.LogWarning($"这是run方法你出错的原因是{ex.Message}");
            }
           
        }

        #region 解密数据，插入库表，并返回已更新时间


        /// <summary>
        /// 解密台风路径经纬度
        /// </summary>
        private string decryTFLSLJ(DateTime timeTFLSLJ)
        {
            //获取要更新数据
            var TFLSLJdecrypyions = _repository.getDecryPtionDataTFLSLJ(timeTFLSLJ);
            //解密
            foreach (var decrypyion in TFLSLJdecrypyions)
            {
                decrypyion.JD_T = DeJwdInfo(decrypyion.JD.ToString(), decrypyion.ID);
                decrypyion.WD_T = DeJwdInfo(decrypyion.WD.ToString(), decrypyion.ID);
            }
            //更新数据
            _repository.updateTFLSLJ(TFLSLJdecrypyions);
            return _repository.getConfigTime("F02_TFLSLJ");
        }

        /// <summary>
        /// 解密预报台风路径经纬度
        /// </summary>
        private string decryTFYBLJ(DateTime timeTFYBLJ)
        {
            //获取要更新数据
            var TFYBLJdecrypyions = _repository.getDecryPtionDataTFYBLJ(timeTFYBLJ);
            foreach (var decrypyion in TFYBLJdecrypyions)
            {
                decrypyion.JD_T = DeJwdInfo(decrypyion.JD.ToString(), decrypyion.ID);
                decrypyion.WD_T = DeJwdInfo(decrypyion.WD.ToString(), decrypyion.ID);
            }
            //更新数据
            _repository.updateTFYBLJ(TFYBLJdecrypyions);
            return _repository.getConfigTime("F02_TFYBLJ");

        }
        #endregion

        #region 解密经纬度


        private void SetKey()
        {
            this.Key[0] = "0458623971";
            this.Key[1] = "0723486915";
            this.Key[2] = "0235189674";
            this.Key[3] = "0342189576";
            this.Key[4] = "0764832915";
            this.Key[5] = "0165734982";
            this.Key[6] = "0635179284";
            this.Key[7] = "0182397654";
            this.Key[8] = "0467812395";
            this.Key[9] = "0375649281";
        }


        /// <summary>
        /// 经纬度解密方法
        /// </summary>
        /// <param name="strJwd">经纬度</param>
        /// <param name="jmId">解密编号</param>
        /// <returns></returns>
        public double DeJwdInfo(string strJwd, Int64 jmId)
        {
            if (String.IsNullOrEmpty(Key[0]))
            {
                SetKey();
            }
            double jwd = 0.0;
            if (jmId > 0 && !string.IsNullOrEmpty(strJwd))
            {
                strJwd = DeData(jmId.ToString(), strJwd);
            }
            if (string.IsNullOrEmpty(strJwd) || !double.TryParse(strJwd, out jwd))
            {
                jwd = 0.0;
            }
            return jwd;
        }


        /// <summary>
        /// 解密方法
        /// </summary>
        /// <param name="Id">解密编号</param>
        /// <param name="data">数据</param>
        /// <returns></returns>
        public string DeData(string Id, string data)
        {
            string text = "";
            if (Id != "" && data != "")
            {
                if (this.Key[0] == null)
                {
                    this.SetKey();
                }
                int num = Convert.ToInt32(Id.Substring(Id.Length - 1, 1));
                data = (Convert.ToDouble(data) / 4.0).ToString("0");
                for (int i = 0; i <= data.Length - 1; i++)
                {
                    string text2 = data.Substring(i, 1);
                    if (text2 == "0")
                    {
                        text += "0";
                    }
                    else
                    {
                        text += this.Key[num].IndexOf(text2).ToString();
                    }
                }
                if (text != "")
                {
                    text = Convert.ToString(Convert.ToDouble(text) / 100.0);
                }
            }
            return text;
        }



        #endregion 解密经纬度

        #region 读取配置文件

        /// <summary>
        /// 获取配置文件信息
        /// </summary>
        private void readConfig()
        {
            try
            {
                _repository = _context.GetRepository<DecryptionRepository>();
                //获取配置时间
                configPath = _context.GetCustomData<string>("configPath");
                string[] times = File.ReadAllLines(configPath);
                //获取配置时间
                foreach (var time in times)
                {
                    if (time.Contains("TFLSLJ"))
                    {
                        updateTimeTFLSLJ = time.Substring(7, 23);
                    }
                    if (time.Contains("TFYBLJ"))
                    {
                        updateTimeTFYBLJ = time.Substring(7, 23);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"这里是readConfig方法，出错误的原因是{ex.Message}");
                
            }

        }

        #endregion
    }
}
