using Domain;
using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace Storage.Mappings
{
    public class UserMap : ClassMap<User>
    {
        public UserMap()
        {
            Table("users");
            Id(u => u.ID, "id");
            Map(u => u.UserName, "user_name");
            Map(u => u.Mail, "mail");
            Map(u => u.Password, "password");
            Map(u => u.Deleted, "deleted").Not.Nullable();
        }
    }
}
