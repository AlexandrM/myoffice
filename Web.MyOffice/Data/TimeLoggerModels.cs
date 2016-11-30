using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Web.MyOffice.Res;

namespace Web.MyOffice.Data
{

    #region Settings

    /*public class TimeLoggerSetting
    {
        /// <summary>
        /// User id
        /// </summary>
        [Key]
        [Display(ResourceType = typeof(S), Name="Id")]
        public Guid Id { get; set; }
    }*/

    public class TimeLoggerSettingWorkStation
    {
        [Key]
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        //public Guid SettingsId  { get; set; }

        //public virtual TimeLoggerSetting Settings { get; set; }

        [Display(ResourceType = typeof(S), Name="Name")]
        public string Name { get; set; }

        [Display(ResourceType = typeof(S), Name = "WorkStationId")]
        public Guid WorkStationId { get; set; }
    }

    #endregion Settings

    public class TimeLoggerApplicationCategory
    {
        [Key]
        [Display(ResourceType = typeof(S), Name = "Id")]
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        [Display(ResourceType = typeof(S), Name = "Name")]
        public string Name { get; set; }
    }

    #region Logger

    public class TimeLoggerStartStop
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime Start { get; set; }
        public DateTime LastFix { get; set; }
        public DateTime Stop { get; set; }

        public Guid WorkStationId { get; set; }
    }

    public class TimeLoggerApplicationArgument
    {
        [Key]
        public Guid Id { get; set; }

        public Guid WorkStationId { get; set; }

        public Guid? CategoryId { get; set; }
        public virtual TimeLoggerApplicationCategory Category { get; set; }

        public string WorkStationName { get; set; }

        public string AppPath { get; set; }
        public string AppExe { get; set; }
        public string Argument { get; set; }

        public virtual List<TimeLoggerLogItem> LogItems { get; set; }
    }

    public class TimeLoggerLogItem
    {
        [Key]
        public long Id { get; set; }

        public virtual Guid ApplicationArgumentId { get; set; }
        public virtual TimeLoggerApplicationArgument ApplicationArgument { get; set; }

        public DateTime DTActivate { get; set; }
        public DateTime DTDeActivate { get; set; }

        public string WindowHeader { get; set; }

        public long LogId { get; set; }
    }
    
    #endregion Logger
}