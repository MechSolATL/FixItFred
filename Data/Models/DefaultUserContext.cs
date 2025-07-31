using Infrastructure.Context;

namespace Data.Models
{
    public class DefaultUserContext : IUserContext
    {
        public string User => "System"; // stub user identity
    }
}
