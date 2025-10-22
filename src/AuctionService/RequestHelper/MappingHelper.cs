using AuctionService.DTOs;
using AuctionService.Entites;
using AutoMapper;

namespace AuctionService.RequestHelper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles() {
            // will look for matching names in both Auction and Auction.Item
            CreateMap<Auction, AuctionDto>().IncludeMembers(x => x.Item);

            //AutoMapper is told that, in addition to mapping properties from Auction itself,
            //it should also map properties from its related object Item into AuctionDto.
            //However, AutoMapper can only do that if it already knows how to map from Item to AuctionDto
            CreateMap<Item, AuctionDto>();

            //Maps from CreateAuctionDto to Auction, saying that the Item property 
            //inside Auction should be mapped from the entire source object s (usually meaning that 
            //CreateAuctionDto contains information to initialize the related Item).
            CreateMap<CreateAuctionDto, Auction>()
                .ForMember(d => d.Item, o => o.MapFrom(s => s));

            CreateMap<CreateAuctionDto, Item>();


        }

    }
}
    