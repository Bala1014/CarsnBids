using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.VisualBasic;

namespace AuctionService.Entites
{
    [Table("Items")]
    public class Item
    {
        public Guid Id { get; set; }
        public string? Make { get; set; }
        public string? Model { get; set; }
        public int year { get; set; }
        public string? color { get; set; }

        public int Mileage { get; set; }

        public string? ImgUrl { get; set; }
        public Auction? Auction { get; set; }
        public Guid AuctionId {  get; set; }



    }
}
