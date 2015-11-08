using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

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
                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        conn.Open();
                        conn.Execute(sql, entity,tran,20);
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}
