using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using activitiesapp.Helpers;

namespace activitiesapp.Services.Interfaces
{
    public interface IEmailService
    {
        void SendEmail(MailModel mailModel);

    }
}
