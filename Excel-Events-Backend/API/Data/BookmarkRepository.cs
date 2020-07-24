using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data.Interfaces;
using API.Dtos.Bookmark;
using API.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class BookmarkRepository : IBookmarkRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IRegistrationRepository _repo;
        public BookmarkRepository(DataContext context, IMapper mapper, IRegistrationRepository repo)
        {
            _repo = repo;
            _mapper = mapper;
            _context = context;
        }

        public async Task<bool> Add(int excelId, int eventId)
        {
            if( await _context.Bookmarks.FirstOrDefaultAsync(x => x.ExcelId == excelId && x.EventId == eventId) != null)
                throw new Exception(" Event is already in bookmarks. ");
            var favorite = new Bookmark
            {
                ExcelId = excelId, EventId = eventId, IsRegistered = await _repo.HasRegistered(excelId, eventId)
            };
            await _context.Bookmarks.AddAsync(favorite);
            var success = await _context.SaveChangesAsync() > 0;
            return success;
        }

        public async Task<List<EventForBookmarkListViewDto>> EventList(int excelId)
        {
            List<Bookmark> registrations = await _context.Bookmarks.Where(r => r.ExcelId == excelId).Include(x => x.Event).ToListAsync();
            List<EventForBookmarkListViewDto> eventList = new List<EventForBookmarkListViewDto>();
            foreach (var x in registrations)
            {
                var eventForView = _mapper.Map<EventForBookmarkListViewDto>(x.Event);
                eventForView.IsRegistered = x.IsRegistered;
                eventList.Add(eventForView);
            }
            return eventList;
        }

        public async Task<bool> Remove(int excelId, int eventId)
        {
            var fav = await _context.Bookmarks.FirstOrDefaultAsync(x => x.ExcelId == excelId && x.EventId == eventId);
            _context.Remove(fav);
            var success = await _context.SaveChangesAsync() > 0;
            return success;
        }

        public async Task<bool> RemoveAll(int excelId)
        {
            List<Bookmark> bookmarks = await _context.Bookmarks.Where(r => r.ExcelId == excelId).ToListAsync();
            _context.RemoveRange(bookmarks);
            var success = await _context.SaveChangesAsync() > 0;
            return success;
        }
    }
}