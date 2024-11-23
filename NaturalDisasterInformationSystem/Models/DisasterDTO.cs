using System.Text.Json.Serialization;

namespace NaturalDisasterInformationSystem.Models
{
    public class DisasterDTO
    {
        [JsonPropertyName("id")]

        public int Id { get; set; }

        [JsonPropertyName("kv_anhhuong")]
        public string AffectedArea { get; set; } // Vị trí ảnh hưởng

        [JsonPropertyName("time_update")]
        public DateTime TimeUpdate { get; set; }

        //[JsonPropertyName("time_start")]
        public DateTime TimeStart { get; set; }

        [JsonPropertyName("lon")]
        public double Longitude { get; set; } // Kinh độ

        [JsonPropertyName("lat")]
        public double Latitude { get; set; } // Vĩ độ

        [JsonPropertyName("id_disaster")]
        public int DisasterId { get; set; }

        [JsonPropertyName("huong_dichuyen")]
        public string Direction { get; set; } // Hướng di chuyển

        [JsonPropertyName("link_detail")]
        public string DetailLink { get; set; } // Liên kết chi tiết

        [JsonPropertyName("name_vn")]
        public string NameVn { get; set; } // Tên bão

        [JsonPropertyName("description")]
        public string Description { get; set; } // Mô tả

        [JsonPropertyName("level")]
        public int Level { get; set; } // Cấp độ rủi ro

        [JsonPropertyName("da_ketthuc")]
        public bool IsCompleted { get; set; } // Đã kết thúc hay chưa

        [JsonPropertyName("thiethai_ve_nguoi")]
        public int HumanLoss { get; set; } // Thiệt hại về người

        [JsonPropertyName("thiethai_ve_tien")]
        public int EconomicLoss { get; set; } // Thiệt hại về tiền

        [JsonPropertyName("disaster")]
        public DisasterDetails DisasterDetails { get; set; } // Thông tin chi tiết về bão
    }

    public class DisasterDetails
    {
        public int Id { get; set; }

        [JsonPropertyName("name_disaster")]
        public string NameDisaster { get; set; }

        [JsonPropertyName("image")]
        public string ImageUrl { get; set; }

        [JsonPropertyName("can_delete")]
        public bool CanDelete { get; set; }

        [JsonPropertyName("TotalDisaster")]
        public int TotalDisaster { get; set; }

        [JsonPropertyName("CategoryDisasterAttachments")]
        public List<object> CategoryDisasterAttachments { get; set; } // Có thể thay đổi kiểu dữ liệu nếu cần
    }
}
