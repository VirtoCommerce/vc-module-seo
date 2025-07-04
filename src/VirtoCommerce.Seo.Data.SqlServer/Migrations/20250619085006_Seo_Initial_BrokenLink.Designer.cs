﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using VirtoCommerce.Seo.Data.Repositories;

#nullable disable

namespace VirtoCommerce.Seo.Data.SqlServer.Migrations
{
    [DbContext(typeof(SeoDbContext))]
    [Migration("20250619085006_Seo_Initial_BrokenLink")]
    partial class Seo_Initial_BrokenLink
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("VirtoCommerce.Seo.Data.Models.BrokenLinkEntity", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("HitCount")
                        .HasColumnType("int");

                    b.Property<string>("LanguageCode")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<DateTime>("LastHitDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<DateTime?>("ModifiedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Permalink")
                        .HasMaxLength(2048)
                        .HasColumnType("nvarchar(2048)");

                    b.Property<string>("RedirectUrl")
                        .HasMaxLength(2048)
                        .HasColumnType("nvarchar(2048)");

                    b.Property<string>("Status")
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<string>("StoreId")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.HasKey("Id");

                    b.HasIndex("Permalink", "StoreId", "LanguageCode")
                        .IsUnique()
                        .HasFilter("[StoreId] IS NOT NULL AND [Permalink] IS NOT NULL AND [LanguageCode] IS NOT NULL");

                    b.ToTable("BrokenLink", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
