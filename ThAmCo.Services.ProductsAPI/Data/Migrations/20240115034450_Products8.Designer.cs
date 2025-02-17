﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ThAmCo.ProductsAPI.Data;


#nullable disable

namespace ThAmCo.Services.ProductsAPI.Data.Migrations
{
    [DbContext(typeof(ProductsContext))]
    [Migration("20240115034450_Products8")]
    partial class Products8
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.14");

            modelBuilder.Entity("ThAmCo.Services.ProductsAPI.Data.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<float>("Price")
                        .HasColumnType("REAL");

                    b.HasKey("Id");

                    b.ToTable("Products");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Description = " Test",
                            Name = "Test product G",
                            Price = 2f
                        },
                        new
                        {
                            Id = 2,
                            Description = " Test",
                            Name = "Test product H",
                            Price = 2f
                        },
                        new
                        {
                            Id = 3,
                            Description = " Test",
                            Name = "Test product I",
                            Price = 2f
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
