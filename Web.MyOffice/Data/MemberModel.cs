using ASE.EF;
using ASE.MVC;
using MyBank.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Web.MyOffice.Data;
using Web.MyOffice.Res;


namespace Web.MyOffice.Models
{
    [SelectAttribute(Action = "Index", Controller = "Member", ResourceType = typeof(S), ResourceName = "Members")]
    [Displayble(Displayble = "FullName")]
    public partial class Member : EFModel//, IMember
    {
        private DB DataBase { set; get; }

        public Member()
        {
            DataBase = new DB();
        }

        /// <summary>
        /// Owner
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Owner
        /// </summary>
        public Member User { get; set; }

        /// <summary>
        /// User from auth
        /// </summary>
        public Guid MainMemberId { get; set; }

        /// <summary>
        /// User from auth
        /// </summary>
        [LocalizedDisplayAttribute("Member")]
        public Member MainMember { get; set; }

        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        [Required]
        public string Email { get; set; }

        [Required]
        [LocalizedDisplayAttribute("View")]
        public string FullName { get; set; }

        [LocalizedDisplayAttribute("FirstName")]
        public string FirstName { get; set; }

        [LocalizedDisplayAttribute("MiddleName")]
        public string MiddleName { get; set; }

        [LocalizedDisplayAttribute("LastName")]
        public string LastName { get; set; }

        [LocalizedDisplayAttribute("MyCurrency")]
        public string MyCurrencyLabel { get; set; }
        public List<Currency> Currencies { get; set; }
        public Guid? APISessionId { get; set; }

        public DateTime? APISessionDT { get; set; }

        public List<Budget> Budgets { get; set; }
    }
}