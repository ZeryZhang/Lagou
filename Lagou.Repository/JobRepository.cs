using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Lagou.Repository.Entity;

namespace Lagou.Repository
{
    public class JobRepository
    {
        private DapperHelper dapperHelper = new DapperHelper();
        public void Insert(JobEntity entity)
        {
            string sql = @"INSERT  INTO Job
                            ( Score ,CreateTime ,FormatCreateTime ,PositionId ,PositionName ,PositionType ,WorkYear ,Education ,
                            JobNature ,CompanyName ,CompanyId ,City ,CompanyLogo ,IndustryField ,PositionAdvantag ,Salary ,PositionFirstType ,
                            LeaderName ,CompanySize ,FinanceStage)
                            VALUES  ( @Score ,@CreateTime ,@FormatCreateTime ,@PositionId ,@PositionName ,@PositionType ,@WorkYear ,@Education ,
                            @JobNature ,@CompanyName ,@CompanyId ,@City ,@CompanyLogo ,@IndustryField ,@PositionAdvantag ,@Salary ,@PositionFirstType ,
                            @LeaderName ,@CompanySize ,@FinanceStage)";

            using (DbConnection conn = dapperHelper.GetConnection())
            {
                conn.Open();
                conn.Execute(sql, entity);
            }
        }

        public void Insert(List<JobEntity> entity)
        {
            string sql = @"INSERT  INTO Job
                            ( Score ,CreateTime ,FormatCreateTime ,PositionId ,PositionName ,PositionType ,WorkYear ,Education ,
                            JobNature ,CompanyName ,CompanyId ,City ,CompanyLogo ,IndustryField ,PositionAdvantag ,Salary ,PositionFirstType ,
                            LeaderName ,CompanySize ,FinanceStage)
                            VALUES  ( @Score ,@CreateTime ,@FormatCreateTime ,@PositionId ,@PositionName ,@PositionType ,@WorkYear ,@Education ,
                            @JobNature ,@CompanyName ,@CompanyId ,@City ,@CompanyLogo ,@IndustryField ,@PositionAdvantag ,@Salary ,@PositionFirstType ,
                            @LeaderName ,@CompanySize ,@FinanceStage)";

            using (DbConnection conn = dapperHelper.GetConnection())
            {
                conn.Open();

                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        conn.Execute(sql, entity, tran, 20);
                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
        }


        /// <summary>
        /// 城市职位需求总数
        /// </summary>
        /// <returns></returns>
        public List<CityNeedJobNumEntity> QueryJCityNeedJobNum()
        {
            var jobList = new List<CityNeedJobNumEntity>();
            string sql = @"SELECT TOP 15
                                COUNT(City) [JobNum] ,
                                City 
                        FROM    dbo.Job
                        GROUP BY City 
                        ORDER BY JobNum DESC";

            using (var conn = dapperHelper.GetConnection())
            {
                conn.Open();
                jobList = conn.Query<CityNeedJobNumEntity>(sql).ToList();
            }

            return jobList;
        }

        /// <summary>
        /// 查询城市的公司总数
        /// </summary>
        /// <returns></returns>
        public List<CityCompanyNumEntity> QueryCityCompanyNum()
        {
            var companyList = new List<CityCompanyNumEntity>();
            string sql = @"SELECT City,COUNT(DISTINCT CompanyName) AS CompanyNum
                        FROM Job
                        GROUP BY City  ORDER BY CompanyNum DESC ";

            using (var conn = dapperHelper.GetConnection())
            {
                conn.Open();
                companyList = conn.Query<CityCompanyNumEntity>(sql).ToList();
            }

            return companyList;
        }






    }
}
