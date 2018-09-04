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

        private ConcurrentDictionary<string, AuthSession> sessions = new ConcurrentDictionary<string, AuthSession>();

        public void Add(AuthSession authSession)
        {
            if (!sessions.ContainsKey(authSession.Id)) sessions.TryAdd(authSession.Id, authSession);
        }

        public bool Valid(string sessionId)
        {
            var foundSessions = sessions.Where(s => s.Key == sessionId).ToList();

            if (foundSessions.Count != 1) return false;

            var session = foundSessions[0].Value;
            if (DateTime.Now.Subtract(session.StartDate).TotalSeconds > LIFETIME_SECONDS)
            {
                sessions.TryRemove(session.Id, out session);
                return false;
            }

            return true;
        }
    }
}
