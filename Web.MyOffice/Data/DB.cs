using MyBank.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.Migrations;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using Web.MyOffice.Models;

namespace Web.MyOffice.Data
{
    public class DB : DbContext
    {
        #region TimeLogger

        //public DbSet<TimeLoggerSetting> TimeLoggerSettings { get; set; }

        public DbSet<TimeLoggerSettingWorkStation> TimeLoggerSettingWorkStations { get; set; }


        public DbSet<TimeLoggerApplicationCategory> TimeLoggerApplicationCategories { get; set; }

        public DbSet<TimeLoggerStartStop> TimeLoggerStartStop { get; set; }

        public DbSet<TimeLoggerApplicationArgument> TimeLoggerApplicationArguments { get; set; }

        public DbSet<TimeLoggerLogItem> TimeLoggerLogItems { get; set; }
        
        #endregion TimeLogger

        #region QuickRecords

        public DbSet<QuickRecordCategory> QuickRecordCategories { get; set; }

        public DbSet<QuickRecord> QuickRecords { get; set; }

        #endregion QuickRecords


        public DbSet<Member> Members { get; set; }

        public DbSet<MemberRate> MemberRates { get; set; }

        public DbSet<Currency> Currencies { get; set; }
        
        public DbSet<CurrencyRate> CurrencyRates { get; set; }

        public DbSet<MemberDayReport> MemberDayReports { get; set; }

        public DbSet<MemberPayment> MemberPayments { get; set; }

        public DbSet<Project> Projects { get; set; }

        public DbSet<ProjectMember> ProjectMembers { get; set; }

        public DbSet<ProjectTask> ProjectTasks { get; set; }

        public DbSet<Attachment> Attachments { get; set; }

        public DbSet<ProjectTaskComment> ProjectTaskComments { get; set; }

        #region MyBank

        public DbSet<CategoryAccount> CategoryAccounts { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Item> Items { get; set; }

        public DbSet<CategoryItem> CategoryItems { get; set; }

        public DbSet<Motion> Motions { get; set; }

        public DbSet<Record> BankRecords { get; set; }

        public DbSet<Budget> Budgets { get; set; }

        public DbSet<BudgetUser> BudgetUsers { get; set; }
        

        #endregion MyBank

        public DB()
            : base("ConnectionString")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Entity<Member>().HasRequired(x => x.MainMember).WithMany().WillCascadeOnDelete(false);
            modelBuilder.Entity<Member>().HasRequired(x => x.User).WithMany().WillCascadeOnDelete(false);

            modelBuilder.Entity<Project>().HasRequired(x => x.Author).WithMany().WillCascadeOnDelete(false);
            modelBuilder.Entity<ProjectTask>().HasRequired(x => x.Author).WithMany().WillCascadeOnDelete(false);

            modelBuilder.Entity<CategoryAccount>()
                .Property(x => x.Name)
                .HasMaxLength(255);

            modelBuilder.Entity<Budget>()
                .Property(x => x.Name)
                .HasMaxLength(255);

            modelBuilder.Entity<Project>()
                .Property(x => x.Name)
                .HasMaxLength(255);

            modelBuilder.Entity<Account>()
                .Property(x => x.Name)
                .HasMaxLength(255);

            modelBuilder.Entity<CategoryItem>()
                .Property(x => x.Name)
                .HasMaxLength(255);

            modelBuilder.Entity<Member>()
                .Property(x => x.FullName)
                .HasMaxLength(255);

            modelBuilder.Entity<Item>()
                .Property(x => x.Name)
                .HasMaxLength(255);

            modelBuilder.Entity<Record>()
                .Property(x => x.Name)
                .HasMaxLength(255);

            modelBuilder.Entity<Attachment>()
                .Property(x => x.Name)
                .HasMaxLength(255);

            modelBuilder.Entity<Attachment>()
                .Property(x => x.OriginalName)
                .HasMaxLength(255);

            modelBuilder.Entity<Currency>()
                .Property(x => x.Name)
                .HasMaxLength(255);

            modelBuilder.Entity<Currency>()
                .Property(x => x.ShortName)
                .HasMaxLength(255);

            modelBuilder.Entity<Member>()
                .Property(x => x.Email)
                .HasMaxLength(255);

            modelBuilder.Entity<Member>()
                .Property(x => x.FirstName)
                .HasMaxLength(255);

            modelBuilder.Entity<Member>()
                .Property(x => x.MiddleName)
                .HasMaxLength(255);

            modelBuilder.Entity<Member>()
                .Property(x => x.LastName)
                .HasMaxLength(255);

            modelBuilder.Entity<ProjectTask>()
                .Property(x => x.Name)
                .HasMaxLength(255);

            modelBuilder.Entity<QuickRecordCategory>()
                .Property(x => x.Name)
                .HasMaxLength(255);


            #region MyBank

            modelBuilder.Entity<Account>().HasMany(x => x.Motions).WithRequired(x => x.Account).WillCascadeOnDelete(false);

            modelBuilder.Entity<Member>().HasMany(x => x.Currencies).WithRequired(x => x.User).WillCascadeOnDelete(false);
            modelBuilder.Entity<Member>().HasMany(x => x.Budgets).WithRequired(x => x.Owner).WillCascadeOnDelete(false);

            modelBuilder.Entity<Budget>().HasMany(x => x.Items).WithRequired(x => x.Budget).WillCascadeOnDelete(false);
            modelBuilder.Entity<Budget>().HasMany(x => x.CategoryAccounts).WithRequired(x => x.Budget).WillCascadeOnDelete(false);
            modelBuilder.Entity<Budget>().HasMany(x => x.CategoryItems).WithRequired(x => x.Budget).WillCascadeOnDelete(false);

            modelBuilder.Entity<CurrencyRate>().Property(x => x.Value).HasPrecision(19, 4);
            modelBuilder.Entity<Currency>().Property(x => x.Value).HasPrecision(19, 4);

            //modelBuilder.Entity<AccountUser>().HasRequired(x => x.User).WithMany().WillCascadeOnDelete(false);
            //modelBuilder.Entity<AccountUser>().HasOptional(x => x.Category).WithMany().WillCascadeOnDelete(false);

            #endregion MyBank

            //modelBuilder.Ignore<Budget>();
            //modelBuilder.Ignore<BudgetUser>();
            //modelBuilder.Ignore<CategoryAccount>();
            //modelBuilder.Ignore<CategoryItem>();
            //modelBuilder.Ignore<Account>();
            //modelBuilder.Ignore<Item>();
            //modelBuilder.Ignore<Motion>();
            //modelBuilder.Ignore<Record>();

            if (System.Configuration.ConfigurationManager.AppSettings["TablePrefix"] != null)
            {
                modelBuilder.Types()
                    .Configure(entity => entity.ToTable(System.Configuration.ConfigurationManager.AppSettings["TablePrefix"] + "_" + entity.ClrType.Name));
            }
        }

        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                if ((HttpContext.Current != null) && (HttpContext.Current.Response != null))
                {
                    foreach (var entityValidationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in entityValidationErrors.ValidationErrors)
                        {
                            HttpContext.Current.Response.Write("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
                        }
                    }
                }
                throw ex;
            }
            catch (DbUpdateException ex)
            {
                if ((HttpContext.Current != null) && (HttpContext.Current.Response != null))
                { 
                    HttpContext.Current.Response.Write(String.Format("DbUpdateException Message: {0}", ex.Message));
                    HttpContext.Current.Response.Write(String.Format("DbUpdateException InnerException: {0}", ex.InnerException));
                    if (ex.InnerException != null)
                        HttpContext.Current.Response.Write(String.Format("DbUpdateException InnerException: {0}", ex.InnerException.InnerException));

                    foreach (var data in ex.Data)
                    {
                        HttpContext.Current.Response.Write(String.Format("DbUpdateException data: {0}", data));
                    }
                    foreach (var entry in ex.Entries)
                    {
                        HttpContext.Current.Response.Write(String.Format("DbUpdateException entry: {0}", entry));
                    }
                }
                throw ex;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
    }

    public class InitData : DBInit<DB>
    {

    }

    public class DBInit<TContext> : IDatabaseInitializer<TContext> where TContext : System.Data.Entity.DbContext
    {
        public DBInit()
        {
        }

        public void InitializeDatabase(TContext context)
        {
            var configuration = new DbMigrationsConfiguration<TContext>();
            configuration.AutomaticMigrationDataLossAllowed = true;
            configuration.AutomaticMigrationsEnabled = true;
            var migrator = new DbMigrator(configuration);
            migrator.Update();
        }

    }
}