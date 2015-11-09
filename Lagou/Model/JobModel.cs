using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lagou.Model
{
     public  class JobModel
    {

        public int score { get; set; }
        public DateTime createTime { get; set; }
        public string formatCreateTime { get; set; }
        public string positionId { get; set; }
        public string positionName { get; set; }
        /// <summary>
        /// 职位分类
        /// </summary>
        public string positionType { get; set; }//后端开发

        public string workYear { get; set; }
        /// <summary>
        /// 教育程度
        /// </summary>
        public string education { get; set; }
        /// <summary>
        /// 工作性质 (全职)
        /// </summary>
        public string jobNature { get; set; }
        public string companyName { get; set; }
        public string companyId { get; set; }
        public string city { get; set; }
        public string companyLogo { get; set; }
        /// <summary>
        /// 行业
        /// </summary>
        public string industryField { get; set; }
        //职位福利
        public string positionAdvantag { get; set; }// : 千万级用户产品+年底奖金+分红+期权激励 ,
        /// <summary>
        /// 薪水范畴
        /// </summary>
        public string salary { get; set; }

        public string positionFirstType { get; set; } //: 技术 ,

        public string leaderName { get; set; }// : 王乐 ,
        //公司人数
        public string companySize { get; set; }
        /// <summary>
        /// 融资情况
        /// </summary>
        public string financeStage { get; set; }//: 成长型(A轮) ,
        /// <summary>
        /// 公司福利
        /// </summary>
        public List<string> companyLabelList { get; set; }//:[节日礼物 ,带薪年假 ,绩效奖金 ,年度旅游 ]
    }
}
