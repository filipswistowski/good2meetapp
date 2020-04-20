using activitiesapp.Models;
using System.Threading.Tasks;

namespace activitiesapp.Repositories.Interfaces
{
    public interface IEventRepository : IRepository<Event>
    {
        Task<Event[]> GetAllEventsAsync();

        Task<Event> GetEventByIdAsync(int id);

        Task<Event[]> GetEventsByUserId(int id);

        void CreateEvent(Event eEvent);

        void UpdateEvent(Event eEvent);

        Task<bool> SaveChangeAsync();
    }
}