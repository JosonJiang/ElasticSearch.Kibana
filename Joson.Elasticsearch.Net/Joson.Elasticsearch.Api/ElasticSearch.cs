using Nest;
using System;

namespace Joson.ElasticSearch.Api
{


    /// <summary>
    /// 5.x 特性
    /// </summary>
    [ElasticsearchType(RelationName = "SearchInfo", IdProperty = "Id")]
    public class SearchInfo
    {
        [Number(NumberType.Long, Name = "Id")]
        public long Id { get; set; }

        /// <summary>
        /// keyword 不分词
        /// </summary>
        [Keyword(Name = "Name", Index = true)]
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Keyword(Name = "Title", Index = true)]
        public string Title { get; set; }

        /// <summary>
        /// text 分词,Analyzer = "ik_max_word"
        /// </summary>
        [Text(Name = "Dic", Index = true, Fielddata = true)]
        public string Dic { get; set; }

        [Keyword(Name = "DicKeyWord", Index = true)]
        public string DicKeyword { get { return Dic; } }


        [Number(NumberType.Integer, Name = "State")]
        public int State { get; set; }

        [Number(NumberType.Integer, Name = "Group")]
        public int Group { get; set; }

        [Boolean(Name = "Deleted")]
        public bool Deleted { get; set; }

        [Date(Name = "AddTime")]
        public DateTime AddTime { get; set; }

        [Number(NumberType.Float, Name = "PassingRate")]
        public float PassingRate { get; set; }

        [Number(NumberType.Double, Name = "Dvalue")]
        public double Dvalue { get; set; }



        [Number(NumberType.Double, Name = "PriceStart")]
        public Double PriceStart { get; set; }

        [Number(NumberType.Double, Name = "PriceEnd")]
        public Double PriceEnd { get; set; }

        private DateTime _createTime;

        [Date(Name = "CreateTime")]
        public DateTime CreateTime
        {
            get { return _createTime.ToLocalTime(); }
            set
            {
                //由于从数据库查出的时间 Kind = Unspecified 所以主动设置为 本地
                //否则 写入es时会把未知默认作为 UTC，实则是本地时间
                //通过本字段查询的时候赋值时也需注意 传入的时间对象Kind值不能为 Unspecified
                if (value.Kind == DateTimeKind.Unspecified)
                    value = DateTime.SpecifyKind(value, DateTimeKind.Local);
                _createTime = value;
            }
        }
    }

 
    /// <summary>
    /// VendorPrice 实体
    /// </summary>
    [ElasticsearchType(IdProperty = "priceID", RelationName = "VendorPriceInfo")]
    public class VendorPriceInfo
    {
        [Number(NumberType.Long)]
        public Int64 priceID { get; set; }
        [Date(Format = "mmddyyyy")]
        public DateTime modifyTime { get; set; }
        /// <summary>
        /// 如果string 类型的字段不需要被分析器拆分，要作为一个正体进行查询，需标记此声明，否则索引的值将被分析器拆分
        /// </summary>
        [Keyword(Index =true)]
        public string pvc_Name { get; set; }
        /// <summary>
        /// 设置索引时字段的名称
        /// </summary>
        [Keyword(Name = "PvcDesc")]
        public string pvc_Desc { get; set; }
        /// <summary>
        /// 如需使用坐标点类型需添加坐标点特性，在maping时会自动映射类型
        /// </summary>
        [GeoPoint(Name = "ZuoBiao")]
        public GeoLocation Location { get; set; }
    }

}
