using API_Role_Based_Authorization.Entities;
using System.Collections.Generic;
using System.Linq;

namespace API_Role_Based_Authorization.Helpers
{
    public static class ExtensionMethods
    {
        public static IEnumerable<User> WithoutPassword(this IEnumerable<User> users)
        {
            if (users == null)  return null;

            return users.Select(x => x.WithoutPassword());
        }

        public static User WithoutPassword(this User user)
        {
            if (user == null) return null;

            user.Password = null;
            return user;
        }
    }
}
