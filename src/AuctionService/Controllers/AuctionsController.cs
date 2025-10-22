using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entites;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuctionsController : ControllerBase
    {
        private AuctionDbContext _auctionDbContext;
        private IMapper _mapper;

        public AuctionsController(AuctionDbContext auctionDbContext, IMapper mapper)
        {
            _auctionDbContext = auctionDbContext;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<AuctionDto>>> GetAuctions()
        {
            
            var auctions = await _auctionDbContext.Auctions
                .Include(a => a.Item)
                .OrderBy(x => x.Item.Make).ToListAsync();
            return _mapper.Map<List<AuctionDto>>(auctions);

        }

        [HttpGet("{Id}")]
        public async Task<ActionResult<AuctionDto>> GetAuctionById(Guid Id)
        {
            var auction = await _auctionDbContext.Auctions
                .Include(a => a.Item)
                .FirstOrDefaultAsync(a => a.Id == Id);
            
            if (auction == null)
                return NotFound();

            return _mapper.Map<AuctionDto>(auction);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAuction([FromBody] CreateAuctionDto createAuctionDto)
        {
            var auction = _mapper.Map<Auction>(createAuctionDto);
            auction.Id = Guid.NewGuid();
            // TODO : add current user as seller
            try
            {
                await _auctionDbContext.Auctions.AddAsync(auction);
                await _auctionDbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return BadRequest(new { message = "Error adding auction: " + e.Message });
            }
            var createdAuctionDto = _mapper.Map<AuctionDto>(auction);
            return CreatedAtAction(nameof(GetAuctionById), new { Id = createdAuctionDto.Id }, createdAuctionDto);

        }

        [HttpPut("{Id}")]
        public async Task<IActionResult> UpdateAuction([FromRoute] Guid Id,[FromBody] UpdateAuctionDto UpdateAuctionDto)
        {
            var auction = await _auctionDbContext.Auctions
                .Include(a => a.Item)
                .FirstOrDefaultAsync(a => a.Id == Id);


            if (auction == null)return NotFound();

            // todo check seller == username
            auction.UpdatedAt = DateTime.UtcNow;

            auction.Item.Make = UpdateAuctionDto.Make ?? auction.Item.Make;
            auction.Item.Model = UpdateAuctionDto.Model ?? auction.Item.Model;
            auction.Item.Color= UpdateAuctionDto.Color ?? auction.Item.Color;
            auction.Item.Mileage = UpdateAuctionDto.Mileage ?? auction.Item.Mileage;
            auction.Item.Year = UpdateAuctionDto.Year ?? auction.Item.Year;




            var result = await _auctionDbContext.SaveChangesAsync() > 0;
            if(result == false)
                return BadRequest(new { message = "Error updating auction" });
            
            return NoContent();
        }


        [HttpDelete("{Id}")]
        public async Task<IActionResult> DeleteAuction([FromRoute] Guid Id) { 
            
            var auction  = await _auctionDbContext.Auctions.FindAsync(Id);

            if(auction == null)return NotFound();

            _auctionDbContext.Auctions.Remove(auction);

            //TODO add seller check
            var result = await _auctionDbContext.SaveChangesAsync() > 0;
            if(!result)return BadRequest(new { message = "Error deleting auction" });

            return Ok();
        }


    }
}
