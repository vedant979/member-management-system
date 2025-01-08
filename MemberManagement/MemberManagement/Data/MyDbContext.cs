using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;
using Project5.Models;

namespace Project5.Data;

public partial class MyDbContext : DbContext
{
    public MyDbContext()
    {
    }

    public MyDbContext(DbContextOptions<MyDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Address> Addresses { get; set; }

    public virtual DbSet<Contact> Contacts { get; set; }

    public virtual DbSet<Member> Members { get; set; }

    public virtual DbSet<Memberaddress> Memberaddresses { get; set; }

    public virtual DbSet<Sessionlog> Sessionlogs { get; set; }

   
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb3_general_ci")
            .HasCharSet("utf8mb3");

        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasKey(e => e.AddressId).HasName("PRIMARY");

            entity.ToTable("address");

            entity.Property(e => e.AddressId).HasColumnName("address_id");
            entity.Property(e => e.City)
                .HasMaxLength(45)
                .HasColumnName("city");
            entity.Property(e => e.Country)
                .HasMaxLength(45)
                .HasColumnName("country");
            entity.Property(e => e.HouseNo)
                .HasMaxLength(45)
                .HasColumnName("house_no");
            entity.Property(e => e.PostalCode)
                .HasMaxLength(10)
                .HasColumnName("postal_code");
            entity.Property(e => e.State)
                .HasMaxLength(45)
                .HasColumnName("state");
            entity.Property(e => e.Street)
                .HasMaxLength(300)
                .HasColumnName("street");
        });

        modelBuilder.Entity<Contact>(entity =>
        {
            entity.HasKey(e => new { e.ContactId, e.MemberId })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("contact");

            entity.HasIndex(e => e.ContactId, "contact_id_UNIQUE").IsUnique();

            entity.HasIndex(e => e.MemberId, "fk_Contact_Member1_idx");

            entity.Property(e => e.ContactId).HasColumnName("contact_id");
            entity.Property(e => e.MemberId).HasColumnName("member_id");
            entity.Property(e => e.ContactType)
                .HasColumnType("enum('Personal','home','work')")
                .HasColumnName("contact_type");
            entity.Property(e => e.PhoneNumber).HasColumnName("phone_number");

            entity.HasOne(d => d.Member).WithMany(p => p.Contacts)
                .HasForeignKey(d => d.MemberId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_Contact_Member1");
        });

        modelBuilder.Entity<Member>(entity =>
        {
            entity.HasKey(e => e.MemberId).HasName("PRIMARY");

            entity.ToTable("member");

            entity.HasIndex(e => e.MemberId, "Id_UNIQUE").IsUnique();

            entity.HasIndex(e => e.Email, "email_UNIQUE").IsUnique();

            entity.Property(e => e.MemberId).HasColumnName("member_id");
            entity.Property(e => e.Dob).HasColumnName("dob");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(45)
                .HasColumnName("first_name");
            entity.Property(e => e.Gender)
                .HasColumnType("enum('male','female','other')")
                .HasColumnName("gender");
            entity.Property(e => e.HashPassword)
                .HasMaxLength(400)
                .HasColumnName("hash_password");
            entity.Property(e => e.LastName)
                .HasMaxLength(45)
                .HasColumnName("last_name");
            entity.Property(e => e.MiddleName)
                .HasMaxLength(45)
                .HasColumnName("middle_name");
        });

        modelBuilder.Entity<Memberaddress>(entity =>
        {
            entity.HasKey(e => new { e.MemberAddressId, e.MemberId, e.AddressId })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0, 0 });

            entity.ToTable("memberaddress");

            entity.HasIndex(e => e.AddressId, "fk_MemberAddress_Address1_idx");

            entity.HasIndex(e => e.MemberId, "fk_MemberAddress_Member1_idx");

            entity.Property(e => e.MemberAddressId).HasColumnName("MemberAddress_id");
            entity.Property(e => e.MemberId).HasColumnName("member_id");
            entity.Property(e => e.AddressId).HasColumnName("address_id");
            entity.Property(e => e.AddressType)
                .HasColumnType("enum('current','permanent','work')")
                .HasColumnName("address_type");

            entity.HasOne(d => d.Address).WithMany(p => p.Memberaddresses)
                .HasForeignKey(d => d.AddressId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_MemberAddress_Address1");

            entity.HasOne(d => d.Member).WithMany(p => p.Memberaddresses)
                .HasForeignKey(d => d.MemberId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_MemberAddress_Member1");
        });

        modelBuilder.Entity<Sessionlog>(entity =>
        {
            entity.HasKey(e => new { e.SessionlogId, e.MemberId })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("sessionlog");

            entity.HasIndex(e => e.MemberId, "fk_SessionLog_Member1_idx");

            entity.Property(e => e.SessionlogId).HasColumnName("sessionlog_id");
            entity.Property(e => e.MemberId).HasColumnName("member_id");
            entity.Property(e => e.IsValid).HasMaxLength(5);
            entity.Property(e => e.SessionDuration).HasColumnName("session_duration");
            entity.Property(e => e.Token)
                .HasMaxLength(300)
                .HasColumnName("token");

            entity.HasOne(d => d.Member).WithMany(p => p.Sessionlogs)
                .HasForeignKey(d => d.MemberId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_SessionLog_Member1");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
