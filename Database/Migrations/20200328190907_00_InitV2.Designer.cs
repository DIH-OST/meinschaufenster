﻿// <auto-generated />
using System;
using Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Database.Migrations
{
    [DbContext(typeof(Db))]
    [Migration("20200328190907_00_InitV2")]
    partial class _00_InitV2
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Database.Tables.TableAbsence", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Date");

                    b.Property<string>("Reason");

                    b.Property<int>("StoreId");

                    b.HasKey("Id");

                    b.HasIndex("StoreId");

                    b.ToTable("Absence");
                });

            modelBuilder.Entity("Database.Tables.TableAppointment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Attended");

                    b.Property<DateTime>("BookedOn");

                    b.Property<bool>("Canceled");

                    b.Property<int>("EmployeeId");

                    b.Property<string>("Text");

                    b.Property<int>("UserId");

                    b.Property<DateTime>("ValidFrom");

                    b.Property<DateTime>("ValidTo");

                    b.HasKey("Id");

                    b.HasIndex("EmployeeId");

                    b.HasIndex("UserId");

                    b.ToTable("Appointment");
                });

            modelBuilder.Entity("Database.Tables.TableDeliveryOption", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description");

                    b.Property<string>("Icon");

                    b.HasKey("Id");

                    b.ToTable("DeliveryOption");
                });

            modelBuilder.Entity("Database.Tables.TableEmployee", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Active");

                    b.Property<string>("DefaultAnnotation");

                    b.Property<string>("FirstName");

                    b.Property<byte[]>("Image");

                    b.Property<string>("LastName");

                    b.Property<int>("StoreId");

                    b.Property<string>("TelephoneNumber");

                    b.HasKey("Id");

                    b.HasIndex("StoreId");

                    b.ToTable("Employee");
                });

            modelBuilder.Entity("Database.Tables.TableImage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<byte[]>("File");

                    b.Property<string>("FileName");

                    b.Property<int>("ImageType");

                    b.Property<DateTime>("SavedAt");

                    b.Property<int>("StoreId");

                    b.HasKey("Id");

                    b.HasIndex("StoreId");

                    b.ToTable("Image");
                });

            modelBuilder.Entity("Database.Tables.TableLocation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Address");

                    b.Property<string>("City");

                    b.Property<string>("Country");

                    b.Property<string>("EMail");

                    b.Property<string>("FederalState");

                    b.Property<double>("Latitude");

                    b.Property<double>("Longitude");

                    b.Property<string>("Name");

                    b.Property<string>("PostCode");

                    b.Property<int>("StoreId");

                    b.Property<string>("Telephonenumber");

                    b.HasKey("Id");

                    b.HasIndex("StoreId");

                    b.ToTable("Location");
                });

            modelBuilder.Entity("Database.Tables.TableLocationEmployee", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("TblEmployeeId");

                    b.Property<int>("TblLocationId");

                    b.HasKey("Id");

                    b.HasIndex("TblEmployeeId");

                    b.HasIndex("TblLocationId");

                    b.ToTable("LocationEmployee");
                });

            modelBuilder.Entity("Database.Tables.TableNotifications", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AzureTag");

                    b.Property<int>("TblUserId");

                    b.HasKey("Id");

                    b.HasIndex("TblUserId");

                    b.ToTable("TableNotifications");
                });

            modelBuilder.Entity("Database.Tables.TableOpeningHours", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("StoreId");

                    b.Property<DateTime?>("TimeFrom");

                    b.Property<DateTime?>("TimeTo");

                    b.Property<int>("Weekday");

                    b.HasKey("Id");

                    b.HasIndex("StoreId");

                    b.ToTable("OpeningHours");
                });

            modelBuilder.Entity("Database.Tables.TablePaymentOption", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description");

                    b.Property<string>("Icon");

                    b.HasKey("Id");

                    b.ToTable("PaymentOption");
                });

            modelBuilder.Entity("Database.Tables.TableProductCategory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description");

                    b.Property<string>("Icon");

                    b.HasKey("Id");

                    b.ToTable("ProductCategory");
                });

            modelBuilder.Entity("Database.Tables.TableSpecialDay", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Date");

                    b.Property<string>("Reason");

                    b.Property<int>("StoreId");

                    b.HasKey("Id");

                    b.HasIndex("StoreId");

                    b.ToTable("SpecialDay");
                });

            modelBuilder.Entity("Database.Tables.TableStore", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Activated");

                    b.Property<string>("ActivationCode");

                    b.Property<string>("Address");

                    b.Property<string>("City");

                    b.Property<string>("CompanyName");

                    b.Property<string>("Country");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("Description");

                    b.Property<string>("EMail");

                    b.Property<string>("FederalState");

                    b.Property<double>("Latitude");

                    b.Property<double>("Longitude");

                    b.Property<string>("Password");

                    b.Property<string>("PostCode");

                    b.Property<string>("RestPassword");

                    b.Property<string>("Telephonenumber");

                    b.Property<string>("Website");

                    b.HasKey("Id");

                    b.ToTable("Store");
                });

            modelBuilder.Entity("Database.Tables.TableStoreCategory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("IsMainStoreCategory");

                    b.Property<int>("TblProductCategoryId");

                    b.Property<int>("TblStoreId");

                    b.HasKey("Id");

                    b.HasIndex("TblProductCategoryId");

                    b.HasIndex("TblStoreId");

                    b.ToTable("StoreCategory");
                });

            modelBuilder.Entity("Database.Tables.TableStoreDelivery", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("TblDeliveryOptionId");

                    b.Property<int>("TblStoreId");

                    b.HasKey("Id");

                    b.HasIndex("TblDeliveryOptionId");

                    b.HasIndex("TblStoreId");

                    b.ToTable("StoreDelivery");
                });

            modelBuilder.Entity("Database.Tables.TableStorePayment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("TblPaymentOptionId");

                    b.Property<int>("TblStoreId");

                    b.HasKey("Id");

                    b.HasIndex("TblPaymentOptionId");

                    b.HasIndex("TblStoreId");

                    b.ToTable("StorePayment");
                });

            modelBuilder.Entity("Database.Tables.TableUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("City");

                    b.Property<DateTime>("CreatedAtUtc");

                    b.Property<string>("DefaultUserLanguage");

                    b.Property<string>("Firstname");

                    b.Property<bool>("IsAdmin");

                    b.Property<bool>("IsDemoUser");

                    b.Property<string>("Lastname");

                    b.Property<bool>("Locked");

                    b.Property<string>("Password");

                    b.Property<bool>("PhoneChecked");

                    b.Property<string>("PhoneNumber");

                    b.Property<string>("PostalCode");

                    b.Property<string>("RestPasswort");

                    b.Property<string>("Street");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Database.Tables.TableUserDevice", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("TblUserId");

                    b.HasKey("Id");

                    b.HasIndex("TblUserId");

                    b.ToTable("UserDevices");
                });

            modelBuilder.Entity("Database.Tables.TableVirtualWorkTime", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("EmployeeId");

                    b.Property<TimeSpan>("TimeFrom");

                    b.Property<int>("TimeSlot");

                    b.Property<TimeSpan>("TimeTo");

                    b.Property<int>("Weekday");

                    b.HasKey("Id");

                    b.HasIndex("EmployeeId");

                    b.ToTable("VirtualWorkTime");
                });

            modelBuilder.Entity("Database.Tables.TableAbsence", b =>
                {
                    b.HasOne("Database.Tables.TableStore", "Store")
                        .WithMany("Absences")
                        .HasForeignKey("StoreId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Database.Tables.TableAppointment", b =>
                {
                    b.HasOne("Database.Tables.TableEmployee", "Employee")
                        .WithMany("TblAppointments")
                        .HasForeignKey("EmployeeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Database.Tables.TableUser", "User")
                        .WithMany("TblAppointments")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Database.Tables.TableEmployee", b =>
                {
                    b.HasOne("Database.Tables.TableStore", "Store")
                        .WithMany("Employees")
                        .HasForeignKey("StoreId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Database.Tables.TableImage", b =>
                {
                    b.HasOne("Database.Tables.TableStore", "Store")
                        .WithMany("Images")
                        .HasForeignKey("StoreId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Database.Tables.TableLocation", b =>
                {
                    b.HasOne("Database.Tables.TableStore", "Store")
                        .WithMany("TblLocations")
                        .HasForeignKey("StoreId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Database.Tables.TableLocationEmployee", b =>
                {
                    b.HasOne("Database.Tables.TableEmployee", "TblEmployee")
                        .WithMany("TblLocationEmployee")
                        .HasForeignKey("TblEmployeeId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Database.Tables.TableLocation", "TblLocation")
                        .WithMany("TblLocationEmployee")
                        .HasForeignKey("TblLocationId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Database.Tables.TableNotifications", b =>
                {
                    b.HasOne("Database.Tables.TableUser", "TblUser")
                        .WithMany()
                        .HasForeignKey("TblUserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Database.Tables.TableOpeningHours", b =>
                {
                    b.HasOne("Database.Tables.TableStore", "Store")
                        .WithMany("OpeningHours")
                        .HasForeignKey("StoreId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Database.Tables.TableSpecialDay", b =>
                {
                    b.HasOne("Database.Tables.TableStore", "Store")
                        .WithMany("SpecialDays")
                        .HasForeignKey("StoreId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Database.Tables.TableStoreCategory", b =>
                {
                    b.HasOne("Database.Tables.TableProductCategory", "TblProductCategory")
                        .WithMany("TblStoreCategory")
                        .HasForeignKey("TblProductCategoryId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Database.Tables.TableStore", "TblStore")
                        .WithMany("TblStoreCategories")
                        .HasForeignKey("TblStoreId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Database.Tables.TableStoreDelivery", b =>
                {
                    b.HasOne("Database.Tables.TableDeliveryOption", "TblDeliveryOption")
                        .WithMany("TblStoreDelivery")
                        .HasForeignKey("TblDeliveryOptionId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Database.Tables.TableStore", "TblStore")
                        .WithMany("TblStoreDelivery")
                        .HasForeignKey("TblStoreId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Database.Tables.TableStorePayment", b =>
                {
                    b.HasOne("Database.Tables.TablePaymentOption", "TblPaymentOption")
                        .WithMany("TblStorePayments")
                        .HasForeignKey("TblPaymentOptionId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Database.Tables.TableStore", "TblStore")
                        .WithMany("TblStorePayments")
                        .HasForeignKey("TblStoreId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Database.Tables.TableUserDevice", b =>
                {
                    b.HasOne("Database.Tables.TableUser", "TblUser")
                        .WithMany("TblUserDevices")
                        .HasForeignKey("TblUserId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.OwnsOne("Exchange.Model.DbUserDeviceInfo", "Device", b1 =>
                        {
                            b1.Property<int>("TableUserDeviceId")
                                .ValueGeneratedOnAdd()
                                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                            b1.Property<int>("DeviceIdiom");

                            b1.Property<string>("DeviceToken");

                            b1.Property<string>("DeviceType");

                            b1.Property<bool>("IsAppRunning");

                            b1.Property<DateTime>("LastDateTimeUtcOnline");

                            b1.Property<string>("OperatingSystemVersion");

                            b1.Property<int>("Plattform");

                            b1.HasKey("TableUserDeviceId");

                            b1.ToTable("UserDevices");

                            b1.HasOne("Database.Tables.TableUserDevice")
                                .WithOne("Device")
                                .HasForeignKey("Exchange.Model.DbUserDeviceInfo", "TableUserDeviceId")
                                .OnDelete(DeleteBehavior.Cascade);
                        });
                });

            modelBuilder.Entity("Database.Tables.TableVirtualWorkTime", b =>
                {
                    b.HasOne("Database.Tables.TableEmployee", "Employee")
                        .WithMany("TblVirtualWorkTimes")
                        .HasForeignKey("EmployeeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
