alter table n2Item 
    add column State int(11);

alter table n2Item 
    add column AncestralTrail varchar(50);

alter table n2Item 
    add column VersionIndex int(11);

alter table n2Item 
    add column AlteredPermissions int(11);
	
alter table n2Item 
    add column TemplateKey varchar(50);
	
alter table n2Item 
    add column TranslationKey int(11);
	
	