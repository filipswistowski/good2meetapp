using activitiesapp.Models;
using System.Threading.Tasks;

namespace activitiesapp.Repositories.Interfaces
{
    public interface IParticipantsRepository : IRepository<Participants>
    {
        Task<Participants[]> GetAllEventsWithParticipantsAsync();

        Task<Participants[]> GetEventParticipantsByEventIdAsync(int id);

        Task<Participants[]> GetEventsByParticipantsIdAsync(int id);

        void CreateParticipants(Participants participant);

        Task<bool> SaveChangeAsync();
    }
}