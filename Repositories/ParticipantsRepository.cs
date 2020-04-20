using activitiesapp.Models;
using activitiesapp.Repositories.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace activitiesapp.Repositories
{
    public class ParticipantsRepository : Repository<Participants>, IParticipantsRepository
    {
        private readonly ApplicationContext _applicationContext;

        public ParticipantsRepository(ApplicationContext applicationContext) : base(applicationContext)
        {
            _applicationContext = applicationContext;
        }

        public async Task<Participants[]> GetAllEventsWithParticipantsAsync()
        {
            IQueryable<Participants> participantsQuery = _applicationContext.Participants;
            return participantsQuery.ToArray();
        }

        public async Task<Participants[]> GetEventParticipantsByEventIdAsync(int id)
        {
            IQueryable<Participants> participants = _applicationContext.Participants;

            var participantsList = participants.Where(c => c.EventId == id);

            return participantsList.ToArray();
        }

        public async Task<Participants[]> GetEventsByParticipantsIdAsync(int id)
        {
            IQueryable<Participants> participants = _applicationContext.Participants;

            var participantsList = participants.Where(c => c.UserId == id);

            return participantsList.ToArray();
        }

        public void CreateParticipants(Participants participant)
        {
            _applicationContext.Add(participant);
        }
    }
}