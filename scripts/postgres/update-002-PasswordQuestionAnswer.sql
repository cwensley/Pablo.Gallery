SET SCHEMA 'gallery';

ALTER TABLE "User" ADD COLUMN "PasswordQuestion" varchar,
	ADD COLUMN "PasswordAnswer" varchar;