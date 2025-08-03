namespace MVP_Core.Services;

/// <summary>
/// Field First Circle class for Sprint122_CertumDNSBypass
/// Manages inviting and organizing tech IDs in field-first development
/// </summary>
public class FieldFirstCircle
{
    private readonly Dictionary<string, TechIdMember> _members;
    private readonly Dictionary<string, CircleInvitation> _pendingInvitations;
    private readonly DateTime _circleCreationTime;
    
    public string CircleId { get; private set; }
    public string CircleName { get; private set; }
    public int MaxMembers { get; private set; }
    public bool IsActive { get; private set; }
    
    public FieldFirstCircle(string circleName, int maxMembers = 50)
    {
        CircleId = GenerateCircleId();
        CircleName = circleName ?? "Sprint122_FieldFirstCircle";
        MaxMembers = maxMembers;
        IsActive = true;
        _circleCreationTime = DateTime.UtcNow;
        _members = new Dictionary<string, TechIdMember>();
        _pendingInvitations = new Dictionary<string, CircleInvitation>();
        
        LogCircleCreation();
    }
    
    /// <summary>
    /// Tech ID member representation
    /// </summary>
    public class TechIdMember
    {
        public string TechId { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public DateTime JoinedDate { get; set; }
        public MemberRole Role { get; set; } = MemberRole.Member;
        public List<string> Specializations { get; set; } = new();
        public bool IsActive { get; set; } = true;
        public Dictionary<string, object> Metadata { get; set; } = new();
    }
    
    /// <summary>
    /// Circle invitation representation
    /// </summary>
    public class CircleInvitation
    {
        public string InvitationId { get; set; } = Guid.NewGuid().ToString();
        public string TechId { get; set; } = string.Empty;
        public string InvitedBy { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime ExpiryDate { get; set; }
        public InvitationStatus Status { get; set; } = InvitationStatus.Pending;
        public string Message { get; set; } = string.Empty;
    }
    
    public enum MemberRole
    {
        Member = 1,
        Contributor = 2,
        Moderator = 3,
        Admin = 4,
        Owner = 5
    }
    
    public enum InvitationStatus
    {
        Pending,
        Accepted,
        Declined,
        Expired,
        Revoked
    }
    
    /// <summary>
    /// Invites a tech ID to join the circle
    /// </summary>
    public string InviteTechId(string techId, string invitedBy, string message = "", TimeSpan? validity = null)
    {
        if (string.IsNullOrWhiteSpace(techId))
            throw new ArgumentException("Tech ID cannot be empty", nameof(techId));
            
        if (_members.ContainsKey(techId))
            throw new InvalidOperationException($"Tech ID {techId} is already a member");
            
        if (_members.Count >= MaxMembers)
            throw new InvalidOperationException("Circle has reached maximum member capacity");
            
        var invitation = new CircleInvitation
        {
            TechId = techId,
            InvitedBy = invitedBy,
            Message = message,
            ExpiryDate = DateTime.UtcNow.Add(validity ?? TimeSpan.FromDays(7))
        };
        
        _pendingInvitations[techId] = invitation;
        LogInvitation(invitation);
        
        return invitation.InvitationId;
    }
    
    /// <summary>
    /// Accepts an invitation for a tech ID
    /// </summary>
    public bool AcceptInvitation(string techId, string displayName = "", List<string>? specializations = null)
    {
        if (!_pendingInvitations.TryGetValue(techId, out var invitation))
            return false;
            
        if (invitation.Status != InvitationStatus.Pending)
            return false;
            
        if (DateTime.UtcNow > invitation.ExpiryDate)
        {
            invitation.Status = InvitationStatus.Expired;
            return false;
        }
        
        var member = new TechIdMember
        {
            TechId = techId,
            DisplayName = string.IsNullOrWhiteSpace(displayName) ? techId : displayName,
            JoinedDate = DateTime.UtcNow,
            Role = MemberRole.Member,
            Specializations = specializations ?? new List<string>()
        };
        
        _members[techId] = member;
        invitation.Status = InvitationStatus.Accepted;
        _pendingInvitations.Remove(techId);
        
        LogMemberJoined(member);
        return true;
    }
    
    /// <summary>
    /// Removes a member from the circle
    /// </summary>
    public bool RemoveMember(string techId, string removedBy, string reason = "")
    {
        if (!_members.TryGetValue(techId, out var member))
            return false;
            
        _members.Remove(techId);
        LogMemberRemoved(techId, removedBy, reason);
        
        return true;
    }
    
    /// <summary>
    /// Updates member role
    /// </summary>
    public bool UpdateMemberRole(string techId, MemberRole newRole, string updatedBy)
    {
        if (!_members.TryGetValue(techId, out var member))
            return false;
            
        var oldRole = member.Role;
        member.Role = newRole;
        
        LogRoleUpdate(techId, oldRole, newRole, updatedBy);
        return true;
    }
    
    /// <summary>
    /// Gets all active members
    /// </summary>
    public List<TechIdMember> GetActiveMembers()
    {
        return _members.Values.Where(m => m.IsActive).ToList();
    }
    
    /// <summary>
    /// Gets members by specialization
    /// </summary>
    public List<TechIdMember> GetMembersBySpecialization(string specialization)
    {
        return _members.Values
            .Where(m => m.IsActive && m.Specializations.Contains(specialization, StringComparer.OrdinalIgnoreCase))
            .ToList();
    }
    
    /// <summary>
    /// Gets circle statistics
    /// </summary>
    public string GetCircleStats()
    {
        var totalMembers = _members.Count;
        var activeMembers = _members.Values.Count(m => m.IsActive);
        var pendingInvitations = _pendingInvitations.Values.Count(i => i.Status == InvitationStatus.Pending);
        var uptime = DateTime.UtcNow - _circleCreationTime;
        
        return $"Circle '{CircleName}' ({CircleId}): {activeMembers}/{totalMembers} active members, {pendingInvitations} pending invitations, Uptime: {uptime:dd\\:hh\\:mm\\:ss}";
    }
    
    /// <summary>
    /// Searches for members by criteria
    /// </summary>
    public List<TechIdMember> SearchMembers(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return GetActiveMembers();
            
        var term = searchTerm.ToLower();
        return _members.Values
            .Where(m => m.IsActive && 
                       (m.TechId.ToLower().Contains(term) ||
                        m.DisplayName.ToLower().Contains(term) ||
                        m.Specializations.Any(s => s.ToLower().Contains(term))))
            .ToList();
    }
    
    /// <summary>
    /// Exports circle data for backup
    /// </summary>
    public object ExportCircleData()
    {
        return new
        {
            CircleId,
            CircleName,
            CreationTime = _circleCreationTime,
            Members = _members.Values.ToList(),
            PendingInvitations = _pendingInvitations.Values.ToList(),
            Stats = GetCircleStats()
        };
    }
    
    private string GenerateCircleId()
    {
        return $"CIRCLE_{DateTime.UtcNow:yyyyMMddHHmmss}_{Random.Shared.Next(1000, 9999)}";
    }
    
    private void LogCircleCreation()
    {
        Console.WriteLine($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] CIRCLE_CREATED: {CircleId} '{CircleName}' Max: {MaxMembers}");
    }
    
    private void LogInvitation(CircleInvitation invitation)
    {
        Console.WriteLine($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] INVITATION_SENT: {invitation.InvitationId} to {invitation.TechId} by {invitation.InvitedBy}");
    }
    
    private void LogMemberJoined(TechIdMember member)
    {
        Console.WriteLine($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] MEMBER_JOINED: {member.TechId} ({member.DisplayName}) Role: {member.Role}");
    }
    
    private void LogMemberRemoved(string techId, string removedBy, string reason)
    {
        Console.WriteLine($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] MEMBER_REMOVED: {techId} by {removedBy} Reason: {reason}");
    }
    
    private void LogRoleUpdate(string techId, MemberRole oldRole, MemberRole newRole, string updatedBy)
    {
        Console.WriteLine($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] ROLE_UPDATE: {techId} {oldRole} -> {newRole} by {updatedBy}");
    }
}