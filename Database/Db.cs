// DigitalesSchaufenster (C) 2020 DIH-OST

using System;
using Database.Tables;
using Exchange;
using Exchange.Helper;

namespace Database
{
    /// <summary>
    ///     <para>Projektweite Datenbank - Entity Framework Core</para>
    ///     Klasse CLASS. (C) 2017 FOTEC Forschungs- und Technologietransfer GmbH
    /// </summary>
    public class Db : DbContext
    {
        #region Properties

        public virtual DbSet<TableUser> TblUsers { get; set; }
        public virtual DbSet<TableNotifications> TblNotifications { get; set; }
        public virtual DbSet<TableUserDevice> TblUserDevices { get; set; }
        public virtual DbSet<TableDeliveryOption> TblDeliveryOptions { get; set; }
        public virtual DbSet<TableAppointment> TblAppointments { get; set; }
        public virtual DbSet<TableEmployee> TblEmployees { get; set; }
        public virtual DbSet<TableOpeningHours> TblOpeningHours { get; set; }
        public virtual DbSet<TablePaymentOption> TblPaymentOptions { get; set; }
        public virtual DbSet<TableProductCategory> TblProductCategories { get; set; }
        public virtual DbSet<TableStore> TblStores { get; set; }
        public virtual DbSet<TableVirtualWorkTime> TblVirtualWorkTimes { get; set; }
        public virtual DbSet<TableSpecialDay> TblSpecialDays { get; set; }
        public virtual DbSet<TableImage> TblImages { get; set; }
        public virtual DbSet<TableAbsence> TblAbsences { get; set; }
        public virtual DbSet<TableLocation> TblLocations { get; set; }
        public virtual DbSet<TableLocationEmployee> TblLocationEmployee { get; set; }
        public virtual DbSet<TableStoreCategory> TblStoreCategory { get; set; }
        public virtual DbSet<TableStorePayment> TblStorePayment { get; set; }
        public virtual DbSet<TableStoreDelivery> TblStoreDelivery { get; set; }
        public virtual DbSet<TableStaticSetting> TblStaticSettings { get; set; }
        public virtual DbSet<TableDynamicSetting> TblDynamicSettings { get; set; }

        #endregion

        #region Erzugen, Neu Erzeugen

        /// <summary>
        ///     Datenbank wird bei Aufruf erzugt bzw. gelöscht und neu erzeugt
        /// </summary>
        /// <param name="recreate">Löschen und neu erzeugen</param>
        /// <returns></returns>
        public static bool CreateAnFillUp(bool recreate = false)
        {
            return true;

            using (var db = new Db())
            {
                if (recreate)
                {
                    db.Database.EnsureDeleted();
                }

                if (db.Database.EnsureCreated())
                {
                    var admin = new TableUser
                                {
                                    PhoneNumber = "biss@fotec.at",
                                    Firstname = "Biss",
                                    Lastname = "Admin",
                                    Locked = false,
                                    Password = PasswordHelper.CumputeHash("biss"),
                                    DefaultUserLanguage = "de",
                                    PhoneChecked = true,
                                    IsAdmin = true
                                };
                    db.TblUsers.Add(admin);

                    if (Constants.HasDemoUser)
                    {
                        var demo = new TableUser
                                   {
                                       PhoneNumber = "demo@fotec.at",
                                       Firstname = "Demo",
                                       Lastname = "User",
                                       Locked = false,
                                       Password = "",
                                       DefaultUserLanguage = "de",
                                       PhoneChecked = true,
                                       IsDemoUser = true
                                   };
                        db.TblUsers.Add(demo);
                    }

                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        Logging.Log.LogWarning($"Datenbank Initialwerte konnten nicht erzeugt werden: {e}");
                        return false;
                    }
                }
                else
                {
                    return false;
                }

                return true;
            }
        }

        #endregion

        /// <summary>
        ///     Db Context initialisieren - für SQL Server
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(Constants.ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            Logging.Log.LogInfo("DB OnModelCreating");
            modelBuilder.Entity<TableUserDevice>().OwnsOne(x => x.Device);

            modelBuilder.Entity<TableLocationEmployee>().HasOne(x => x.TblLocation).WithMany(x => x.TblLocationEmployee).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<TableLocationEmployee>().HasOne(x => x.TblEmployee).WithMany(x => x.TblLocationEmployee).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TableStorePayment>().HasOne(x => x.TblStore).WithMany(x => x.TblStorePayments).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<TableStorePayment>().HasOne(x => x.TblPaymentOption).WithMany(x => x.TblStorePayments).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TableStoreCategory>().HasOne(x => x.TblStore).WithMany(x => x.TblStoreCategories).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<TableStoreCategory>().HasOne(x => x.TblProductCategory).WithMany(x => x.TblStoreCategory).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TableStoreDelivery>().HasOne(x => x.TblStore).WithMany(x => x.TblStoreDelivery).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<TableStoreDelivery>().HasOne(x => x.TblDeliveryOption).WithMany(x => x.TblStoreDelivery).OnDelete(DeleteBehavior.Restrict);
        }
    }
}