﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Perritos;

#nullable disable

namespace PERRITOS.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Perritos.Entities.Adoptado", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("FechaAdopcion")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Foto")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("PerritoId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("PerritoId");

                    b.ToTable("Adoptados");
                });

            modelBuilder.Entity("Perritos.Entities.Claim", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("ClaimType")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<bool>("ClaimValue")
                        .HasColumnType("tinyint(1)");

                    b.Property<int>("RolId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("RolId");

                    b.ToTable("Claims");
                });

            modelBuilder.Entity("Perritos.Entities.Discapacidad", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Discapacidades");
                });

            modelBuilder.Entity("Perritos.Entities.Genero", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Generos");
                });

            modelBuilder.Entity("Perritos.Entities.Perrito", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("DiscapacidadId")
                        .HasColumnType("int");

                    b.Property<string>("Edad")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<bool>("Esterilizado")
                        .HasColumnType("tinyint(1)");

                    b.Property<int>("GeneroId")
                        .HasColumnType("int");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("DiscapacidadId");

                    b.HasIndex("GeneroId");

                    b.ToTable("Perritos");
                });

            modelBuilder.Entity("Perritos.Entities.Rol", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("NombreRol")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Rols");
                });

            modelBuilder.Entity("Perritos.Entities.Usuario", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Correo")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<bool>("Estatus")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("NombreCompleto")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("RolId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("RolId");

                    b.ToTable("Usuarios");
                });

            modelBuilder.Entity("Perritos.Entities.Adoptado", b =>
                {
                    b.HasOne("Perritos.Entities.Perrito", "Perrito")
                        .WithMany()
                        .HasForeignKey("PerritoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Perrito");
                });

            modelBuilder.Entity("Perritos.Entities.Claim", b =>
                {
                    b.HasOne("Perritos.Entities.Rol", "Rol")
                        .WithMany("Claims")
                        .HasForeignKey("RolId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Rol");
                });

            modelBuilder.Entity("Perritos.Entities.Perrito", b =>
                {
                    b.HasOne("Perritos.Entities.Discapacidad", "Discapacidad")
                        .WithMany()
                        .HasForeignKey("DiscapacidadId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Perritos.Entities.Genero", "Genero")
                        .WithMany()
                        .HasForeignKey("GeneroId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Discapacidad");

                    b.Navigation("Genero");
                });

            modelBuilder.Entity("Perritos.Entities.Usuario", b =>
                {
                    b.HasOne("Perritos.Entities.Rol", "Rol")
                        .WithMany("Usuarios")
                        .HasForeignKey("RolId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Rol");
                });

            modelBuilder.Entity("Perritos.Entities.Rol", b =>
                {
                    b.Navigation("Claims");

                    b.Navigation("Usuarios");
                });
#pragma warning restore 612, 618
        }
    }
}
