SET SCHEMA 'gallery';


CREATE TABLE "File_Content" (
	"File_Id" int4 NOT NULL,
	"Text" varchar COLLATE "default",
	CONSTRAINT "FileContent_pkey" PRIMARY KEY ("File_Id") NOT DEFERRABLE INITIALLY IMMEDIATE
)
WITH (OIDS=FALSE);

CREATE INDEX  "fti_File_Content_Text" ON "File_Content" USING gin(to_tsvector('english', "Text"));


CREATE OR REPLACE FUNCTION "File_Content_Query"(query varchar, fileName varchar = null)
RETURNS TABLE("File_Id" int4)
	AS $BODY$
BEGIN
if query is null then

	return query select f."File_Id" 
	from "File" f
	where 
		(fileName is null or f."FileName" ilike fileName);
else

	return query select fc."File_Id" 
	from "File_Content" fc
		inner join "File" f on fc."File_Id" = f."Id"
	where 
		(to_tsvector('english', fc."Text") @@ plainto_tsquery('english', query))
		and (fileName is null or f."FileName" ilike fileName);
end if;
END
$BODY$
	LANGUAGE plpgsql
	COST 100
	ROWS 1000
	CALLED ON NULL INPUT
	SECURITY INVOKER
	VOLATILE;
