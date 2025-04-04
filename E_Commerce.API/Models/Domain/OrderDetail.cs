using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace E_Commerce.API.Models.Domain
{
    public class OrderDetail
    {
        public Guid OrderId { get; set; }        

        public Guid ProductId { get; set; }  

        [Required, Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Required, Range(0, double.MaxValue)]
        [Column(TypeName = "decimal(18, 3)")]
        public decimal UnitPrice { get; set; }

        [JsonIgnore]
        public Order? Order { get; set; }   
        
        public Product? Product { get; set; } 
    }
}
