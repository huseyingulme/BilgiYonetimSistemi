﻿// <auto-generated />
using System;
using BilgiYonetimSistemi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BilgiYonetimSistemi.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20241224133047_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("BilgiYonetimSistemi.Models.Advisors", b =>
                {
                    b.Property<int>("AdvisorID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AdvisorID"));

                    b.Property<string>("Department")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("AdvisorID");

                    b.ToTable("Advisors");
                });

            modelBuilder.Entity("BilgiYonetimSistemi.Models.Course", b =>
                {
                    b.Property<int>("CourseID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CourseID"));

                    b.Property<int>("AdvisorID")
                        .HasColumnType("int");

                    b.Property<string>("CourseCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CourseName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Credit")
                        .HasColumnType("int");

                    b.Property<string>("Department")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsMandatory")
                        .HasColumnType("bit");

                    b.HasKey("CourseID");

                    b.HasIndex("AdvisorID");

                    b.ToTable("Courses");
                });

            modelBuilder.Entity("BilgiYonetimSistemi.Models.CourseQuotas", b =>
                {
                    b.Property<int>("CourseId")
                        .HasColumnType("int");

                    b.Property<int>("Quota")
                        .HasColumnType("int");

                    b.Property<int>("RemainingQuota")
                        .HasColumnType("int");

                    b.HasIndex("CourseId")
                        .IsUnique();

                    b.ToTable("CourseQuotas");
                });

            modelBuilder.Entity("BilgiYonetimSistemi.Models.CourseSelectionHistory", b =>
                {
                    b.Property<int>("StudentID")
                        .HasColumnType("int");

                    b.Property<DateTime>("SelectionDate")
                        .HasColumnType("datetime2");

                    b.HasKey("StudentID");

                    b.ToTable("CourseSelectionHistory");
                });

            modelBuilder.Entity("BilgiYonetimSistemi.Models.NonConfirmedSelections", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CourseId")
                        .HasColumnType("int");

                    b.Property<DateTime>("SelectedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("StudentId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CourseId");

                    b.HasIndex("StudentId");

                    b.ToTable("NonConfirmedSelections");
                });

            modelBuilder.Entity("BilgiYonetimSistemi.Models.Student", b =>
                {
                    b.Property<int>("StudentID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("StudentID"));

                    b.Property<int?>("AdvisorID")
                        .HasColumnType("int");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("EnrollmentDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("StudentID");

                    b.HasIndex("AdvisorID");

                    b.ToTable("Students");
                });

            modelBuilder.Entity("BilgiYonetimSistemi.Models.StudentCourseSelections", b =>
                {
                    b.Property<int>("SelectionID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("SelectionID"));

                    b.Property<int>("CourseID")
                        .HasColumnType("int");

                    b.Property<bool>("IsApproved")
                        .HasColumnType("bit");

                    b.Property<DateTime>("SelectionDate")
                        .HasColumnType("date");

                    b.Property<int>("StudentID")
                        .HasColumnType("int");

                    b.HasKey("SelectionID");

                    b.HasIndex("CourseID");

                    b.HasIndex("StudentID");

                    b.ToTable("StudentCourseSelections");
                });

            modelBuilder.Entity("BilgiYonetimSistemi.Models.Users", b =>
                {
                    b.Property<int>("UserID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserID"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("RelatedID")
                        .HasColumnType("int");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserID");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("BilgiYonetimSistemi.Models.Course", b =>
                {
                    b.HasOne("BilgiYonetimSistemi.Models.Advisors", "Advisor")
                        .WithMany("Courses")
                        .HasForeignKey("AdvisorID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Advisor");
                });

            modelBuilder.Entity("BilgiYonetimSistemi.Models.CourseQuotas", b =>
                {
                    b.HasOne("BilgiYonetimSistemi.Models.Course", "Course")
                        .WithOne()
                        .HasForeignKey("BilgiYonetimSistemi.Models.CourseQuotas", "CourseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Course");
                });

            modelBuilder.Entity("BilgiYonetimSistemi.Models.CourseSelectionHistory", b =>
                {
                    b.HasOne("BilgiYonetimSistemi.Models.Student", "Student")
                        .WithMany()
                        .HasForeignKey("StudentID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Student");
                });

            modelBuilder.Entity("BilgiYonetimSistemi.Models.NonConfirmedSelections", b =>
                {
                    b.HasOne("BilgiYonetimSistemi.Models.Course", "Course")
                        .WithMany("NonConfirmedSelections")
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("BilgiYonetimSistemi.Models.Student", "Student")
                        .WithMany("NonConfirmedSelections")
                        .HasForeignKey("StudentId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Course");

                    b.Navigation("Student");
                });

            modelBuilder.Entity("BilgiYonetimSistemi.Models.Student", b =>
                {
                    b.HasOne("BilgiYonetimSistemi.Models.Advisors", "Advisors")
                        .WithMany("Students")
                        .HasForeignKey("AdvisorID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Advisors");
                });

            modelBuilder.Entity("BilgiYonetimSistemi.Models.StudentCourseSelections", b =>
                {
                    b.HasOne("BilgiYonetimSistemi.Models.Course", "Course")
                        .WithMany("StudentCourseSelections")
                        .HasForeignKey("CourseID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BilgiYonetimSistemi.Models.Student", "Student")
                        .WithMany("StudentCourseSelections")
                        .HasForeignKey("StudentID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Course");

                    b.Navigation("Student");
                });

            modelBuilder.Entity("BilgiYonetimSistemi.Models.Advisors", b =>
                {
                    b.Navigation("Courses");

                    b.Navigation("Students");
                });

            modelBuilder.Entity("BilgiYonetimSistemi.Models.Course", b =>
                {
                    b.Navigation("NonConfirmedSelections");

                    b.Navigation("StudentCourseSelections");
                });

            modelBuilder.Entity("BilgiYonetimSistemi.Models.Student", b =>
                {
                    b.Navigation("NonConfirmedSelections");

                    b.Navigation("StudentCourseSelections");
                });
#pragma warning restore 612, 618
        }
    }
}
