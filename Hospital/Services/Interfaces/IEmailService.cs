namespace Hospital.Services.Interfaces
{
    public interface IEmailService
    {
        public void NotifyAdd(string receiver, string username, string password);
        public void NotifyChangePassword(string receiver, string username, string password);
        public void NotifyDelete(string receiver, string username);
        public void NotifyUpdate(string receiver, string username);
        public void SendMessage(string receiver, string subject, string message);
    }
}
