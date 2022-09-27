﻿// <auto-generated />
using System;
using HorrorTacticsApi2.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HorrorTacticsApi2.Migrations
{
    [DbContext(typeof(HorrorDbContext))]
    [Migration("20220927011242_FixIndex")]
    partial class FixIndex
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("AudioEntityStorySceneCommandEntity", b =>
                {
                    b.Property<long>("AudiosId")
                        .HasColumnType("bigint");

                    b.Property<long>("SceneCommandsId")
                        .HasColumnType("bigint");

                    b.HasKey("AudiosId", "SceneCommandsId");

                    b.HasIndex("SceneCommandsId");

                    b.ToTable("AudioEntityStorySceneCommandEntity");
                });

            modelBuilder.Entity("HorrorTacticsApi2.Data.Entities.AudioEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<long>("DurationSeconds")
                        .HasColumnType("bigint");

                    b.Property<long>("FileId")
                        .HasColumnType("bigint");

                    b.Property<bool>("IsBgm")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.HasIndex("FileId");

                    b.ToTable("Audios");
                });

            modelBuilder.Entity("HorrorTacticsApi2.Data.Entities.FileEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("Filename")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<string>("Format")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsDefault")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsVirusScanned")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(300)
                        .HasColumnType("character varying(300)");

                    b.Property<long>("OwnerId")
                        .HasColumnType("bigint");

                    b.Property<long>("SizeInBytes")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("Filename")
                        .IsUnique();

                    b.HasIndex("OwnerId");

                    b.ToTable("Files");
                });

            modelBuilder.Entity("HorrorTacticsApi2.Data.Entities.ImageEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<long>("FileId")
                        .HasColumnType("bigint");

                    b.Property<long>("Height")
                        .HasColumnType("bigint");

                    b.Property<long>("Width")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("FileId");

                    b.ToTable("Images");
                });

            modelBuilder.Entity("HorrorTacticsApi2.Data.Entities.StoryEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(600)
                        .HasColumnType("character varying(600)");

                    b.Property<long>("OwnerId")
                        .HasColumnType("bigint");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(300)
                        .HasColumnType("character varying(300)");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.ToTable("Stories");
                });

            modelBuilder.Entity("HorrorTacticsApi2.Data.Entities.StorySceneCommandEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<long>("Minigames")
                        .HasColumnType("bigint");

                    b.Property<long>("ParentStorySceneId")
                        .HasColumnType("bigint");

                    b.Property<string>("Texts")
                        .HasMaxLength(1000)
                        .HasColumnType("character varying(1000)");

                    b.Property<string>("Timers")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("character varying(1000)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(60)
                        .HasColumnType("character varying(60)");

                    b.HasKey("Id");

                    b.HasIndex("ParentStorySceneId");

                    b.ToTable("StorySceneCommands");
                });

            modelBuilder.Entity("HorrorTacticsApi2.Data.Entities.StorySceneEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<long>("ParentStoryId")
                        .HasColumnType("bigint");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(60)
                        .HasColumnType("character varying(60)");

                    b.HasKey("Id");

                    b.HasIndex("ParentStoryId");

                    b.ToTable("StoryScenes");
                });

            modelBuilder.Entity("HorrorTacticsApi2.Data.Entities.UserEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<DateTime>("LastLogin")
                        .HasColumnType("timestamp with time zone");

                    b.Property<byte[]>("Password")
                        .IsRequired()
                        .HasColumnType("bytea");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<byte[]>("Salt")
                        .IsRequired()
                        .HasColumnType("bytea");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("UserName")
                        .IsUnique();

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            Id = 1L,
                            LastLogin = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Password = new byte[0],
                            Role = "Admin",
                            Salt = new byte[0],
                            UserName = "ht-master"
                        });
                });

            modelBuilder.Entity("ImageEntityStorySceneCommandEntity", b =>
                {
                    b.Property<long>("ImagesId")
                        .HasColumnType("bigint");

                    b.Property<long>("SceneCommandsId")
                        .HasColumnType("bigint");

                    b.HasKey("ImagesId", "SceneCommandsId");

                    b.HasIndex("SceneCommandsId");

                    b.ToTable("ImageEntityStorySceneCommandEntity");
                });

            modelBuilder.Entity("AudioEntityStorySceneCommandEntity", b =>
                {
                    b.HasOne("HorrorTacticsApi2.Data.Entities.AudioEntity", null)
                        .WithMany()
                        .HasForeignKey("AudiosId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HorrorTacticsApi2.Data.Entities.StorySceneCommandEntity", null)
                        .WithMany()
                        .HasForeignKey("SceneCommandsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("HorrorTacticsApi2.Data.Entities.AudioEntity", b =>
                {
                    b.HasOne("HorrorTacticsApi2.Data.Entities.FileEntity", "File")
                        .WithMany()
                        .HasForeignKey("FileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("File");
                });

            modelBuilder.Entity("HorrorTacticsApi2.Data.Entities.FileEntity", b =>
                {
                    b.HasOne("HorrorTacticsApi2.Data.Entities.UserEntity", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("HorrorTacticsApi2.Data.Entities.ImageEntity", b =>
                {
                    b.HasOne("HorrorTacticsApi2.Data.Entities.FileEntity", "File")
                        .WithMany()
                        .HasForeignKey("FileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("File");
                });

            modelBuilder.Entity("HorrorTacticsApi2.Data.Entities.StoryEntity", b =>
                {
                    b.HasOne("HorrorTacticsApi2.Data.Entities.UserEntity", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("HorrorTacticsApi2.Data.Entities.StorySceneCommandEntity", b =>
                {
                    b.HasOne("HorrorTacticsApi2.Data.Entities.StorySceneEntity", "ParentStoryScene")
                        .WithMany("Commands")
                        .HasForeignKey("ParentStorySceneId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ParentStoryScene");
                });

            modelBuilder.Entity("HorrorTacticsApi2.Data.Entities.StorySceneEntity", b =>
                {
                    b.HasOne("HorrorTacticsApi2.Data.Entities.StoryEntity", "ParentStory")
                        .WithMany("Scenes")
                        .HasForeignKey("ParentStoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ParentStory");
                });

            modelBuilder.Entity("ImageEntityStorySceneCommandEntity", b =>
                {
                    b.HasOne("HorrorTacticsApi2.Data.Entities.ImageEntity", null)
                        .WithMany()
                        .HasForeignKey("ImagesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HorrorTacticsApi2.Data.Entities.StorySceneCommandEntity", null)
                        .WithMany()
                        .HasForeignKey("SceneCommandsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("HorrorTacticsApi2.Data.Entities.StoryEntity", b =>
                {
                    b.Navigation("Scenes");
                });

            modelBuilder.Entity("HorrorTacticsApi2.Data.Entities.StorySceneEntity", b =>
                {
                    b.Navigation("Commands");
                });
#pragma warning restore 612, 618
        }
    }
}
