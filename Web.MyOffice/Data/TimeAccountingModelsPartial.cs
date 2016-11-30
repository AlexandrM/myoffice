using ASE.EF;
using ASE.MVC;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using Microsoft.AspNet.Identity;

using Web.MyOffice.Res;
using System.ComponentModel.DataAnnotations.Schema;
using Web.MyOffice.Data;

namespace Web.MyOffice.Models
{
    public partial class Member
    {
        public static Member NewMember(string email)
        {
            Guid id = Guid.NewGuid();
            return new Member 
            { 
                Id  = id,
                UserId = id,
                MainMemberId = id,
                Email = email,
                FullName = String.Format("{0} ({1})", S.IAm, email)
            };
        }


        [NotMapped]
        public Member LocalMember
        {
            get 
            {
                return this.LocalMember(ContextPerRequest<DB>.Db, ContextPerRequest<DB>.UserId);                
            }
        }

        [NotMapped]
        public Guid LocalMemberId { get; set; }
    }
    
    public partial class Currency
    {
        [NotMapped]
        public Member Owner
        {
            get
            {
                return this.User;
            }
            set
            {
                this.User = value;
            }
        }
    }
}