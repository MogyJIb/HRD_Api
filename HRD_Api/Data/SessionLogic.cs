using HRD_DataLibrary.General;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRD_Api.Data
{
    public class SessionLogic
    {
        private const double LIFETIME_SECONDS = 36000;

        private static readonly SessionLogic instance = new SessionLogic();

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static SessionLogic() {}

        private SessionLogic() {}

        public static SessionLogic Instance => instance;

        private ConcurrentBag<AuthSession> sessions = new ConcurrentBag<AuthSession>();

        public void Add(AuthSession authSession)
        {
            if (!sessions.Contains(authSession)) sessions.Add(authSession);
        }

        public bool Valid(string sessionId)
        {
            var foundSessions = sessions.Where(s => s.Id == sessionId).ToList();
            return (foundSessions.Count == 1) 
                ? (DateTime.Now.Subtract(foundSessions[0].StartDate).TotalSeconds < LIFETIME_SECONDS)
                : false;
        }
    }
}
