using MVP_Core.Data.Models;
using System.Collections.Generic;
using System.Linq;
using System;
using Data;

namespace Services
{
    /// <summary>
    /// Handles logic for technician-dispatcher messaging threads, sending, and read status.
    /// </summary>
    public class TechnicianMessageService
    {
        private readonly ApplicationDbContext _db;
        public TechnicianMessageService(ApplicationDbContext db)
        {
            _db = db;
        }
        /// <summary>
        /// Sends a message to a technician or dispatcher for a specific request.
        /// </summary>
        public void SendMessageToTechnician(int technicianId, int requestId, string senderType, string messageBody)
        {
            var msg = new TechnicianMessage
            {
                TechnicianId = technicianId,
                RequestId = requestId,
                Type = senderType,
                Message = messageBody,
                Timestamp = DateTime.UtcNow,
                ReadFlag = false
            };
            _db.TechnicianMessages.Add(msg);
            _db.SaveChanges();
        }
        /// <summary>
        /// Gets the chronological message thread for a request.
        /// </summary>
        public List<TechnicianMessage> GetMessageThreadForRequest(int requestId)
        {
            return _db.TechnicianMessages
                .Where(m => m.RequestId == requestId)
                .OrderBy(m => m.Timestamp)
                .ToList();
        }
        /// <summary>
        /// Marks all messages as read for a technician and request.
        /// </summary>
        public void MarkMessageAsRead(int technicianId, int requestId)
        {
            var unread = _db.TechnicianMessages
                .Where(m => m.TechnicianId == technicianId && m.RequestId == requestId && !m.ReadFlag)
                .ToList();
            foreach (var msg in unread)
                msg.ReadFlag = true;
            _db.SaveChanges();
        }
        /// <summary>
        /// Gets count of unread messages for a technician and request.
        /// </summary>
        public int GetUnreadCount(int technicianId, int requestId)
        {
            return _db.TechnicianMessages.Count(m => m.TechnicianId == technicianId && m.RequestId == requestId && !m.ReadFlag);
        }
    }
}
