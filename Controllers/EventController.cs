using activitiesapp.Models;
using activitiesapp.Models.EventDTO;
using activitiesapp.Repositories.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Threading.Tasks;
using activitiesapp.Helpers;
using activitiesapp.Services.Interfaces;

namespace activitiesapp.Controllers
{
    [ApiController]
    [Route("/event")]
    public class EventController : Controller
    {
        private readonly IEventRepository _eventRepository;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;
        private readonly LinkGenerator _linkGenerator;
        private readonly IParticipantsRepository _participantsRepository;
        private readonly IUserRepository _userRepository;

        public EventController(IEventRepository eventRepository,IEmailService emailService , IMapper mapper, LinkGenerator linkGenerator,
            IParticipantsRepository participantsRepository, IUserRepository userRepository)
        {
            _eventRepository = eventRepository;
            _emailService = emailService;
            _mapper = mapper;
            _linkGenerator = linkGenerator;
            _participantsRepository = participantsRepository;
            _userRepository = userRepository;
        }

        /// <summary>
        /// Returns all events in database
        /// </summary>
        /// <response code="200">Returns list of events</response>
        /// <response code="500">If there is a server error</response>  
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        public async Task<ActionResult<EventFullDTO[]>> Get()
        {
            try
            {
                var events = await _eventRepository.GetAllEventsAsync();

                return Ok(_mapper.Map<EventFullDTO[]>(events));
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }

        /// <summary>
        /// Returns event of given id
        /// </summary>
        /// <param name="EventId">Id of the event we want to get</param>
        /// <response code="200">Returns event by given id</response>
        /// <response code="404">If there is no event with given id</response>  
        /// <response code="500">If there is a server error</response>  
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("{EventId:int}")]
        public async Task<ActionResult<EventFullDTO>> GetEventById(int EventId)
        {
            {
                try
                {
                    var eEvent = await _eventRepository.GetEventByIdAsync(EventId);

                    if (eEvent == null) return NotFound();

                    return Ok(_mapper.Map<EventFullDTO>(eEvent));
                }
                catch (Exception)
                {
                    return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
                }
            }
        }

        /// <summary>
        /// Creates a new event
        /// </summary>
        /// <response code="201">Creates event with given parameters</response>
        /// <response code="400">Invalid input</response>  
        /// <response code="500">If there is a server error</response>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost]
        public async Task<ActionResult<EventFullDTO>> Post(EventFullDTO eventDto)
        {
            try
            {
                var eEvent = _mapper.Map<Event>(eventDto);

                var location = _linkGenerator.GetPathByAction("Get", "Event", new { id = eEvent.EventId });

                if (string.IsNullOrWhiteSpace(location))
                {
                    return BadRequest("Could not post current Event");
                }

                _eventRepository.CreateEvent(eEvent);

                if (await _eventRepository.SaveChangeAsync())
                {
                    return Created($"/api/event/{eEvent.EventId}", (_mapper.Map<EventFullDTO>(eEvent)));
                }

                return Ok();
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }

        /// <summary>
        /// Deletes event with given ID
        /// </summary>
        /// <param name="EventId">Id of the event we want to delete</param>
        /// <response code="200">Succesfully removed event</response>
        /// <response code="400">Invalid input</response>  
        /// <response code="500">If there is a server error</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpDelete("{EventId:int}")]
        public async Task<ActionResult<EventFullDTO>> Delete(int EventId)
        {
            try
            {
                var oldEvent = await _eventRepository.GetEventByIdAsync(EventId);
                if (oldEvent == null) return NotFound($"Could not find event with id of {EventId}");

                _eventRepository.Delete(oldEvent);

                if (await _eventRepository.SaveChangeAsync())
                {
                    return Ok();
                }
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }

            return BadRequest();
        }

        /// <summary>
        /// Adds a user of given id as participant to event with given id
        /// </summary>
        /// <param name="EventId">Id of the event we want to join</param>
        /// <param name="UserId">Id of the user which want to join the event</param>
        /// <response code="200">Succesfully signed for event</response>
        /// <response code="400">Invalid input</response>  
        /// <response code="500">If there is a server error</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("joinevent/{EventId:int}/{UserId:int}")]
        public async Task<ActionResult<ParticipantsDTO>> AddParticipant(int EventId, int UserId, ParticipantsDTO participantsDto)
        {
            try
            {
                var participant = _mapper.Map<Participants>(participantsDto);
                participant.EventId = EventId;
                participant.UserId = UserId;

                _participantsRepository.CreateParticipants(participant);

                var eventTemp = await _eventRepository.GetEventByIdAsync(EventId);
                var mappedEvent = _mapper.Map<EventFullDTO>(eventTemp);
                string eventTitle = mappedEvent.EventName;

                var hostUser = await _userRepository.GetUserByIdAsync(eventTemp.UserId);
                var mappedHost = _mapper.Map<UserFullDTO>(hostUser);
                string hostName = mappedHost.Name;
                string hostEmail = mappedHost.Email;

                var participantUser = await _userRepository.GetUserByIdAsync(UserId);
                var mappedParticipant = _mapper.Map<UserFullDTO>(participantUser);
                string participantsName = mappedParticipant.Name;

                MailModel mailModel = new MailModel();
                mailModel.From = "good2meet.info@gmail.com";
                mailModel.To = participantUser.Email;
                mailModel.Subject = "You have joined the event " + eventTitle;
                mailModel.Body = "Hi " + participantsName + "\n ! You have joined the event hosted by \n " + hostName + ". Here is the " + hostName + "'s contact email address: " + hostEmail + " We hope you will have fun. Thanks for using our app!";
                _emailService.SendEmail(mailModel);

                if (await _participantsRepository.SaveChangeAsync())
                {
                    return Ok();
                }

                
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }

            return BadRequest();
        }

        /// <summary>
        /// Gets list of all events with participants
        /// </summary>
        /// <response code="200">Succesfully signed for event</response>
        /// <response code="400">Invalid input</response>  
        /// <response code="500">If there is a server error</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("getalleventswithusers")]
        public async Task<ActionResult<ParticipantsDTO[]>> GetAllEventsWithParticipants()
        {
            try
            {
                var eventsParticipants = await _participantsRepository.GetAllEventsWithParticipantsAsync();

                return Ok(_mapper.Map<ParticipantsDTO[]>(eventsParticipants));
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }

        /// <summary>
        /// Gets list of participant for given event
        /// </summary>
        /// <param name="EventId">Id of the event we want to get users of</param>
        /// <response code="200">Returns list of users for given event</response>
        /// <response code="400">Invalid input</response>  
        /// <response code="500">If there is a server error</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("geteventwithusers/{EventId:int}")]
        public async Task<ActionResult<ParticipantsDTO[]>> GetParticipantsByEventId(int EventId)
        {
            {
                try
                {
                    var eEvent = await _participantsRepository.GetEventParticipantsByEventIdAsync(EventId);

                    if (eEvent == null) return NotFound();

                    return Ok(_mapper.Map<ParticipantsDTO[]>(eEvent));
                }
                catch (Exception)
                {
                    return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
                }
            }
        }

        /// <summary>
        /// Gets list of events that given user participates
        /// </summary>
        /// <param name="UserId">Id of the user we want to get events he/she participates</param>
        /// <response code="200">Returns list of events for given user</response>
        /// <response code="400">Invalid input</response>  
        /// <response code="500">If there is a server error</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("geteventsbyparticipantsId/{UserId:int}")]
        public async Task<ActionResult<ParticipantsDTO[]>> GetEventsByParticipantsId(int UserId)
        {
            {
                try
                {
                    var eEvent = await _participantsRepository.GetEventsByParticipantsIdAsync(UserId);

                    if (eEvent == null) return NotFound();

                    return Ok(_mapper.Map<ParticipantsDTO[]>(eEvent));
                }
                catch (Exception)
                {
                    return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
                }
            }
        }

        /// <summary>
        /// Gets list of events that given user hosts
        /// </summary>
        /// <param name="UserId">Id of the user we want to get events he/she hosts</param>
        /// <response code="200">Returns list of events hosted by given user</response>
        /// <response code="400">Invalid input</response>  
        /// <response code="500">If there is a server error</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("geteventsbyhost/{UserId:int}")]
        public async Task<ActionResult<EventFullDTO[]>> GetEventsHostedByUser(int UserId)
        {
            {
                try
                {
                    var eEvent = await _eventRepository.GetEventsByUserId(UserId);

                    if (eEvent == null) return NotFound();

                    return Ok(_mapper.Map<EventFullDTO[]>(eEvent));
                }
                catch (Exception)
                {
                    return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
                }
            }
        }

        /// <summary>
        /// Changes parameters of event by given id
        /// </summary>
        /// <param name="EventId">Id of the event we want to change parameters</param>
        /// <response code="200">Succesfully changed parameters</response>
        /// <response code="400">Invalid input</response>  
        /// <response code="500">If there is a server error</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPatch("{EventId:int}")]
        public async Task<ActionResult<EventUpdateDTO>> Put(int EventId, EventUpdateDTO eventUpdateDto)
        {
            try
            {
                var oldEvent = await _eventRepository.GetEventByIdAsync(EventId);
                if (oldEvent == null) return NotFound($"Could not find event with id of {EventId}");

                _mapper.Map(eventUpdateDto, oldEvent);

                if (await _eventRepository.SaveChangeAsync())
                {
                    return _mapper.Map<EventUpdateDTO>(oldEvent);
                }
            }
            catch (Exception)
            {
/*
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
*/
            }

            return BadRequest();
        }

        /// <summary>
        /// Deletes participant from event
        /// </summary>
        /// <param name="EventId">Id of the event we want to user to stop participating</param>
        /// <param name="UserId">Id of the user which want we want to remove from participating</param>
        /// <response code="200">Succesfully removed user from event</response>
        /// <response code="400">Invalid input</response>  
        /// <response code="500">If there is a server error</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpDelete("removeparticipant/{EventId:int}/{UserId:int}")]
        public async Task<ActionResult<ParticipantsDTO>> DeleteParticipantFromEvent(int EventId, int UserId)
        {
            try
            {
                ParticipantsDTO participantsDto = new ParticipantsDTO();
                var participant = _mapper.Map<Participants>(participantsDto);
                participant.EventId = EventId;
                participant.UserId = UserId;
                _participantsRepository.Delete(participant);
                if (await _participantsRepository.SaveChangeAsync())
                {
                    return Ok();
                }
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
            return BadRequest();
        }
    }
}