using ASE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web.MyOffice.Models;

namespace Web.MyOffice.Data
{
    public static class ExtensionsModel2Json
    {

        /*public static dynamic ToJson(this CurrencyRate item)
        {
            return new
            {
                Id = item.Id,
                CurrencyId = item.CurrencyId,
                DateTime = item.DateTime,
                Value = item.Value,
                Rate = 0
            };
        }*/

        /*public static dynamic ToJson(this Currency item)
        {
            return new
            {
                Id = item.Id,
                CurrencyType = item.CurrencyType,
                MyCurrency = item.MyCurrency,
                Name = item.Name,
                ShortName = item.ShortName,
                UserId = item.UserId
            };
        }*/

        /*public static dynamic ToJson(this Currency item, DB db, DateTime rateDate)
        {
            var rate = db.CurrencyRates.Where(x => x.CurrencyId == item.Id & x.DateTime < rateDate).OrderByDescending(x => x.DateTime).Take(1).FirstOrDefault() ?? new CurrencyRate { Value = 1 };
            return new
            {
                Id = item.Id,
                CurrencyType = item.CurrencyType,
                MyCurrency = item.MyCurrency,
                Name = item.Name,
                ShortName = item.ShortName,
                UserId = item.UserId,
                Rate = rate.ToJson()
            };
        }*/

        /*public static dynamic ToJson(this List<Currency> items)
        {
            return items.Select(x => x.ToJson());
        }*/

        /*public static dynamic ToJson(this List<Currency> items, DB db, DateTime rateDate)
        {
            return items.Select(x => x.ToJson(db, rateDate));
        }*/

        /*public static dynamic ToJson(this ProjectMember item)
        {
            return new
            {
                MemberId = item.MemberId,
                ProjectId = item.ProjectId,
                FullName = item.Member.FullName,
                Email = item.Member.Email,
                MemberType = item.MemberType,
                MemberTypeS = item.MemberType.ToDisplayName(),
            };
        }*/

        /*public static dynamic ToJson(this MemberDayReport item)
        {
            return new
            {
                CurrencyId = item.CurrencyId,
                Currency = item.Currency == null ? null : item.Currency.ToJson(),
                DateTime = item.DateTime,
                Description = item.Description,
                Id = item.Id,
                MemberId = item.MemberId,
                Member = item.Member == null ? null : item.Member.ToJson(),
                ProjectId = item.ProjectId,
                Project = item.Project == null ? null : item.Project.ToJson(),
                RateType = item.RateType,
                UserId = item.UserId,
                Amount = item.Amount,
                Value = item.Value,
            };
        }*/

        /*public static dynamic ToJson(this List<MemberDayReport> items)
        {
            return items.Select(x => x.ToJson());
        }*/

        /*public static dynamic ToJson(this Project item)
        {
            return new
            {
                AuthorId = item.AuthorId,
                DateTime = item.DateTime,
                Description = item.Description,
                Id = item.Id,
                Name = item.Name,
                State = item.State,
                StateS = item.State.ToDisplayName(),
            };
        }*/

        /*public static dynamic ToJson(this Member item)
        {
            return new
            {
                Email = item.Email,
                FirstName = item.FirstName,
                FullName = item.FullName,
                Id = item.Id,
                LastName = item.LastName,
                LocalMemberId = item.LocalMemberId,
                MainMemberId = item.MainMemberId,
                MiddleName = item.MiddleName,
                UserId = item.UserId,
            };
        }*/

        /*public static dynamic ToJson(this MemberRate item)
        {
            return new
            {
                CurrencyId = item.CurrencyId,
                Currency = item.Currency == null ? null : item.Currency.ToJson(),
                DateTime = item.DateTime,
                Id = item.Id,
                MemberId = item.MemberId,
                Member = item.Member == null ? null : item.Member.ToJson(),
                RateType = item.RateType,
                RateTypeS = item.RateType.ToDisplayName(),
                UserId = item.UserId,
                Value = item.Value,
            };
        }*/

        /*public static dynamic ToJson(this MemberPayment item)
        {
            return new
            {
                CurrencyId = item.CurrencyId,
                Currency = item.Currency == null ? null : item.Currency.ToJson(),
                DateTime = item.DateTime,
                Description = item.Description,
                Id = item.Id,
                MemberId = item.MemberId,
                Member = item.Member == null ? null : item.Member.ToJson(),
                ProjectId = item.ProjectId,
                Project = item.Project == null ? null : item.Project.ToJson(),
                UserId = item.UserId,
                Amount = item.Amount,
            };
        }*/

        /*public static dynamic ToJson(this List<MemberPayment> items)
        {
            return items.Select(x => x.ToJson());
        }*/
    }
}