using System;
using Rebus.Handlers;
using Thunders.TechTest.ApiService.Infrastructure;
using Thunders.TechTest.Domain;

namespace Thunders.TechTest.ApiService.Infrastructure.Messaging
{
    public class TollUsageMessageHandler : IHandleMessages<TollUsage>
    {
        private readonly AppDbContext _context;

        public TollUsageMessageHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task Handle(TollUsage message)
        {
            await _context.TollUsages.AddAsync(message);
            await _context.SaveChangesAsync();
        }
    }
}