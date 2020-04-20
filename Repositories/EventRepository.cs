using activitiesapp.Models;
using activitiesapp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace activitiesapp.Repositories
{
    public class EventRepository : Repository<Event>, IEventRepository
    {
        private readonly ApplicationContext _applicationContext;

        public EventRepository(ApplicationContext applicationContext) : base(applicationContext)
        {
            _applicationContext = applicationContext;
        }

        public void CreateEvent(Event eEvent)
        {
            _applicationContext.Add(eEvent);
        }

        public void UpdateEvent(Event eEvent)
        {
            _applicationContext.Update(eEvent);
        }

        public async Task<Event[]> GetAllEventsAsync()
        {
            IQueryable<Event> eventQuery = _applicationContext.Events;
            return eventQuery.ToArray();
        }

        public async Task<Event> GetEventByIdAsync(int id)
        {
            IQueryable<Event> events = _applicationContext.Events
                .Where(c => c.EventId == id);

            return await events.FirstOrDefaultAsync();
        }

        public async Task<Event[]> GetEventsByUserId(int id)
        {
            IQueryable<Event> events = _applicationContext.Events;
            var eventsLst = events.Where(c => c.UserId == id);

            return eventsLst.ToArray();

        }



    }
}