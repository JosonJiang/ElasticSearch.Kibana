using Nest;
using System;

namespace Joson.ElasticSearch.Api
{


    /// <summary>
    /// 5.x ����
    /// </summary>
    [ElasticsearchType(RelationName = "SearchInfo", IdProperty = "Id")]
    public class SearchInfo
    {
        [Number(NumberType.Long, Name = "Id")]
        public long Id { get; set; }

        /// <summary>
        /// keyword ���ִ�
        /// </summary>
        [Keyword(Name = "Name", Index = true)]
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Keyword(Name = "Title", Index = true)]
        public string Title { get; set; }

        /// <summary>
        /// text �ִ�,Analyzer = "ik_max_word"
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
                //���ڴ����ݿ�����ʱ�� Kind = Unspecified ������������Ϊ ����
                //���� д��esʱ���δ֪Ĭ����Ϊ UTC��ʵ���Ǳ���ʱ��
                //ͨ�����ֶβ�ѯ��ʱ��ֵʱҲ��ע�� �����ʱ�����Kindֵ����Ϊ Unspecified
                if (value.Kind == DateTimeKind.Unspecified)
                    value = DateTime.SpecifyKind(value, DateTimeKind.Local);
                _createTime = value;
            }
        }
    }

 
    /// <summary>
    /// VendorPrice ʵ��
    /// </summary>
    [ElasticsearchType(IdProperty = "priceID", RelationName = "VendorPriceInfo")]
    public class VendorPriceInfo
    {
        [Number(NumberType.Long)]
        public Int64 priceID { get; set; }
        [Date(Format = "mmddyyyy")]
        public DateTime modifyTime { get; set; }
        /// <summary>
        /// ���string ���͵��ֶβ���Ҫ����������֣�Ҫ��Ϊһ��������в�ѯ�����Ǵ�����������������ֵ�������������
        /// </summary>
        [Keyword(Index =true)]
        public string pvc_Name { get; set; }
        /// <summary>
        /// ��������ʱ�ֶε�����
        /// </summary>
        [Keyword(Name = "PvcDesc")]
        public string pvc_Desc { get; set; }
        /// <summary>
        /// ����ʹ������������������������ԣ���mapingʱ���Զ�ӳ������
        /// </summary>
        [GeoPoint(Name = "ZuoBiao")]
        public GeoLocation Location { get; set; }
    }

}
