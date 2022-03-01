namespace ZDC.Server.Services.Interfaces;

public interface IEmailService
{
    Task<string> LoadEmailTemplate(string name);
    Task<bool> SendEmail(string type, string to, string fromName, string subject, string body);

    Task<bool> SendEventPositionAssigned(string to, int eventId, string name, string position, string start,
        string end);

    Task<bool> SendEventRegistrationSubmitted(string to, int eventId, string name, string start, string end);
    Task<bool> SendEventRegistrationRemoved(string to, int eventId, string name);
    Task<bool> SendFeedbackApproved(string to, string position, string callsign, string comments, string staffComments);
    Task<bool> SendLoaAccepted(string to, string start, string end, string reason);
    Task<bool> SendLoaSubmitted(string to, string start, string end, string reason);
    Task<bool> SendLoaDenied(string to, string reason);
    Task<bool> SendLoaEnded(string to);

    Task<bool> SendNewFeedback(string to, int feedbackId, string position, string callsign, string comments);

    Task<bool> SendNewLoa(string to, int loaId, string name, string start, string end, string reason);

    Task<bool> SendNewOts(string to, int otsId, string name, string cid, string recommender, string rating,
        string facility, string position);

    Task<bool> SendNewStaffingRequest(string to, int staffingRequestId, string name, string email, string affiliation,
        string start, string end, string description);

    Task<bool> SendNewVisitRequest(string to, int visitRequestId, string name, string cid, string rating, string reason,
        string requirements);

    Task<bool> SendOtsAssigned(string to, int otsId, string name, string cid, string rating, string facility,
        string position);

    Task<bool> SendStaffingRequestSubmitted(string to, string name, string email, string affiliation, string start,
        string end, string description);

    Task<bool> SendVisitRequestAccepted(string to);
    Task<bool> SendVisitRequestDenied(string to, string reason);
}