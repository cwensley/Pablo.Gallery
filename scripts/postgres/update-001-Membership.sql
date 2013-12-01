SET SCHEMA 'gallery';

DROP TABLE IF EXISTS "Role";
DROP TABLE IF EXISTS "User_Role";
DROP TABLE IF EXISTS "User_OAuthMembership";
DROP TABLE IF EXISTS "User";

CREATE TABLE "User" (
	"Id" serial NOT NULL,
	"UserName" varchar NOT NULL COLLATE "default",
	"Name" varchar COLLATE "default",
	"LastLoginDate" timestamp(6) NULL,
	"Password" varchar COLLATE "default",
	"CreateDate" timestamp(6) NULL,
	"Email" varchar,
	"Alias" varchar,
	"ConfirmationToken" varchar(128),
	"IsConfirmed" bool NOT NULL,
	"LastPasswordFailureDate" timestamp NULL,
	"PasswordChangedDate" timestamp NULL,
	"PasswordFailuresSinceLastSuccess" int4 NOT NULL,
	"PasswordSalt" varchar,
	"PasswordVerificationToken" varchar(128),
	"PasswordVerificationExpiryDate" timestamp NULL,
	CONSTRAINT "User_pkey" PRIMARY KEY ("Id") NOT DEFERRABLE INITIALLY IMMEDIATE
)
WITH (OIDS=FALSE);

CREATE TABLE "User_OAuthMembership" (
	"User_Id" int4 NOT NULL,
	"Provider" varchar(30) NOT NULL,
	"ProviderUserId" varchar NOT NULL,
	PRIMARY KEY ("Provider", "ProviderUserId"),
	CONSTRAINT "fk_User_OAuthMembership_User" FOREIGN KEY ("User_Id") REFERENCES "User" ("Id") ON DELETE CASCADE
)
WITH (OIDS=FALSE);	


CREATE TABLE "Role" (
	"Id" serial NOT NULL,
	"Name" varchar(30) NOT NULL,
	PRIMARY KEY ("Id")
)
WITH (OIDS=FALSE);


CREATE TABLE "User_Role" (
	"User_Id" int4 NOT NULL,
	"Role_Id" int4 NOT NULL,
	PRIMARY KEY ("User_Id", "Role_Id")
)
WITH (OIDS=FALSE);