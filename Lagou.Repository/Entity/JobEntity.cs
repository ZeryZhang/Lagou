using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lagou.Repository
{
    public class JobEntity
    {
        public String Id { get; set; }

        public int Score { get; set; }
        public DateTime CreateTime { get; set; }
        public string FormatCreateTime { get; set; }
        public string PositionId { get; set; }
        public string PositionName { get; set; }
        /// <summary>
        /// 职位分类
        /// </summary>
        public string PositionType { get; set; }//后端开发

        public string WorkYear { get; set; }
        /// <summary>
        /// 教育程度
        /// </summary>
        public string Education { get; set; }
        /// <summary>
        /// 工作性质 (全职)
        /// </summary>
        public string JobNature { get; set; }
        public string CompanyName { get; set; }
        public string CompanyId { get; set; }
        public string City { get; set; }
        public string CompanyLogo { get; set; }
        /// <summary>
        /// 行业
        /// </summary>
        public string IndustryField { get; set; }
        //职位福利
        public string PositionAdvantag { get; set; }// : 千万级用户产品+年底奖金+分红+期权激励 ,
        /// <summary>
        /// 薪水范畴
        /// </summary>
        public string Salary { get; set; }

        public string PositionFirstType { get; set; } //: 技术 ,

        public string LeaderName { get; set; }// : 王乐 ,
        //公司人数
        public string CompanySize { get; set; }
        /// <summary>
        /// 融资情况
        /// </summary>
        public string FinanceStage { get; set; }//: 成长型(A轮) ,
        /// <summary>
        /// 公司福利
        /// </summary>
        public List<string>  CompanyLabelList { get; set; }//:[节日礼物 ,带薪年假 ,绩效奖金 ,年度旅游 ]
    }

    public class CompanyLabel
    {
 
        public int Id { get; set; }

        public string Tag { get; set; }
    }
}
