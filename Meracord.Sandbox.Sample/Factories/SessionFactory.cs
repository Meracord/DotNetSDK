using Meracord.Sandbox.Helpers;
using NoteWorld.DataServices;

namespace Meracord.Sandbox.Factories
{
    internal class SessionFactory
    {
        public static DataSession Create()
        {
            // Initialize Configuration Variables
            var baseServiceAddress = Settings.BaseServiceAddress;
            var userId = Settings.UserId;
            var password = Settings.Password;

            // Instantiate DataSession
            return new DataSession(baseServiceAddress, userId, password);
        }
    }
}