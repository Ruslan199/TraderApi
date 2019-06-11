using Domain;
using Domain.Enum;
using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace Storage.Mappings
{
    public class ActivationKeyMap : ClassMap<ActivationKey>
    {
        public ActivationKeyMap()
        {
            Table("activation_keys");

            Id(u => u.ID, "id");

            References(e => e.User, "id_user");

            Map(u => u.Key, "key_str");
            Map(u => u.Type, "key_type").CustomType<KeyType>();
            Map(u => u.CreatedDate, "created_date");
            Map(u => u.ActivatedDate, "activated_date");
            Map(u => u.Deleted, "deleted").Not.Nullable();
        }
    }
}
