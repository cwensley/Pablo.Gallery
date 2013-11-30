/*
 Navicat Premium Data Transfer

 Source Server         : server-postgres
 Source Server Type    : PostgreSQL
 Source Server Version : 90204
 Source Host           : server.local
 Source Database       : pablo_gallery
 Source Schema         : gallery

 Target Server Type    : PostgreSQL
 Target Server Version : 90204
 File Encoding         : utf-8

 Date: 11/28/2013 17:55:17 PM
*/

CREATE SCHEMA IF NOT EXISTS "gallery"
SET SCHEMA 'gallery';

-- ----------------------------
--  Sequence structure for Category_Id_seq
-- ----------------------------
DROP SEQUENCE IF EXISTS "Category_Id_seq";
CREATE SEQUENCE "Category_Id_seq" INCREMENT 1 START 1 MAXVALUE 9223372036854775807 MINVALUE 1 CACHE 1;

-- ----------------------------
--  Sequence structure for File_Id_seq
-- ----------------------------
DROP SEQUENCE IF EXISTS "File_Id_seq";
CREATE SEQUENCE "File_Id_seq" INCREMENT 1 START 160913 MAXVALUE 9223372036854775807 MINVALUE 1 CACHE 1;

-- ----------------------------
--  Sequence structure for Pack_Id_seq
-- ----------------------------
DROP SEQUENCE IF EXISTS "Pack_Id_seq";
CREATE SEQUENCE "Pack_Id_seq" INCREMENT 1 START 4264 MAXVALUE 9223372036854775807 MINVALUE 1 CACHE 1;

-- ----------------------------
--  Table structure for Pack
-- ----------------------------
DROP TABLE IF EXISTS "Pack";
CREATE TABLE "Pack" (
	"Id" int4 NOT NULL DEFAULT nextval('"Pack_Id_seq"'::regclass),
	"Name" varchar NOT NULL COLLATE "default",
	"FileName" varchar COLLATE "default",
	"Date" date,
	"Thumbnail_Id" int4
)
WITH (OIDS=FALSE);

-- ----------------------------
--  Table structure for Pack_Category
-- ----------------------------
DROP TABLE IF EXISTS "Pack_Category";
CREATE TABLE "Pack_Category" (
	"Pack_Id" int4 NOT NULL,
	"Category_Id" int4 NOT NULL
)
WITH (OIDS=FALSE);

-- ----------------------------
--  Table structure for File
-- ----------------------------
DROP TABLE IF EXISTS "File";
CREATE TABLE "File" (
	"Id" int4 NOT NULL DEFAULT nextval('"File_Id_seq"'::regclass),
	"FileName" varchar NOT NULL COLLATE "default",
	"Pack_Id" int4 NOT NULL,
	"Format" varchar(20) COLLATE "default",
	"Order" int4,
	"Width" int4,
	"Height" int4,
	"Type" varchar(20) COLLATE "default"
)
WITH (OIDS=FALSE);

-- ----------------------------
--  Table structure for User
-- ----------------------------
DROP TABLE IF EXISTS "User";
CREATE TABLE "User" (
	"Id" int4 NOT NULL,
	"UserName" varchar NOT NULL COLLATE "default",
	"Name" varchar COLLATE "default",
	"LastLogin" timestamp(6) NULL,
	"Password" varchar COLLATE "default",
	"CreatedDate" timestamp(6) NULL
)
WITH (OIDS=FALSE);

-- ----------------------------
--  Table structure for Artist
-- ----------------------------
DROP TABLE IF EXISTS "Artist";
CREATE TABLE "Artist" (
	"Id" int4 NOT NULL,
	"Name" varchar NOT NULL COLLATE "default",
	"DateAdded" date,
	"DateModified" date
)
WITH (OIDS=FALSE);

-- ----------------------------
--  Table structure for Category
-- ----------------------------
DROP TABLE IF EXISTS "Category";
CREATE TABLE "Category" (
	"Id" int4 NOT NULL DEFAULT nextval('"Category_Id_seq"'::regclass),
	"Name" varchar NOT NULL COLLATE "default",
	"Year" int4 NOT NULL
)
WITH (OIDS=FALSE);


-- ----------------------------
--  Alter sequences owned by
-- ----------------------------
ALTER SEQUENCE "Category_Id_seq" OWNED BY "Category"."Id";
ALTER SEQUENCE "File_Id_seq" OWNED BY "File"."Id";
ALTER SEQUENCE "Pack_Id_seq" OWNED BY "Pack"."Id";
-- ----------------------------
--  Primary key structure for table Pack
-- ----------------------------
ALTER TABLE "Pack" ADD CONSTRAINT "Pack_pkey" PRIMARY KEY ("Id") NOT DEFERRABLE INITIALLY IMMEDIATE;

-- ----------------------------
--  Indexes structure for table Pack
-- ----------------------------
CREATE INDEX  "Pack_date" ON "Pack" USING btree("Date" ASC NULLS LAST);
CREATE UNIQUE INDEX  "ux_Pack_name" ON "Pack" USING btree("Name" COLLATE "default" ASC NULLS LAST);

-- ----------------------------
--  Primary key structure for table Pack_Category
-- ----------------------------
ALTER TABLE "Pack_Category" ADD CONSTRAINT "Pack_Category_pkey" PRIMARY KEY ("Pack_Id", "Category_Id") NOT DEFERRABLE INITIALLY IMMEDIATE;

-- ----------------------------
--  Primary key structure for table File
-- ----------------------------
ALTER TABLE "File" ADD CONSTRAINT "File_pkey" PRIMARY KEY ("Id") NOT DEFERRABLE INITIALLY IMMEDIATE;

-- ----------------------------
--  Indexes structure for table File
-- ----------------------------
CREATE INDEX  "ix_File_FileName" ON "File" USING btree("FileName" COLLATE "default" ASC NULLS LAST);
CREATE INDEX  "ix_File_Format" ON "File" USING btree("Format" COLLATE "default" ASC NULLS LAST);
CREATE INDEX  "ix_File_Pack" ON "File" USING btree("Pack_Id" ASC NULLS LAST);

-- ----------------------------
--  Primary key structure for table User
-- ----------------------------
ALTER TABLE "User" ADD CONSTRAINT "User_pkey" PRIMARY KEY ("Id") NOT DEFERRABLE INITIALLY IMMEDIATE;

-- ----------------------------
--  Primary key structure for table Artist
-- ----------------------------
ALTER TABLE "Artist" ADD CONSTRAINT "Artist_pkey" PRIMARY KEY ("Id") NOT DEFERRABLE INITIALLY IMMEDIATE;

-- ----------------------------
--  Primary key structure for table Category
-- ----------------------------
ALTER TABLE "Category" ADD CONSTRAINT "Year_pkey" PRIMARY KEY ("Id") NOT DEFERRABLE INITIALLY IMMEDIATE;

