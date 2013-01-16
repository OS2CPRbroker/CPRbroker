﻿-----------------------------------------------------------------
-----  Allow multiple attributes in a single registration  ------
-----------------------------------------------------------------

sp_rename 'dbo.PersonAttributes.PersonRegistrationId', 'PersonAttributesId', 'COLUMN'
GO
ALTER TABLE dbo.PersonAttributes ADD PersonRegistrationId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID()
GO
UPDATE dbo.PersonAttributes SET PersonRegistrationId = PersonAttributesId
GO
ALTER Table dbo.PersonAttributes DROP CONSTRAINT FK_PersonAttributes_PersonRegistration
GO
ALTER TABLE dbo.PersonAttributes ADD CONSTRAINT FK_PersonAttributes_PersonRegistration FOREIGN KEY(PersonRegistrationId) REFERENCES PersonRegistration(PersonRegistrationId) 
ON UPDATE CASCADE ON DELETE CASCADE
GO

sp_rename 'dbo.CprData.PersonRegistrationId', 'PersonAttributesId', 'COLUMN'
GO
sp_rename 'dbo.ForeignCitizenData.PersonRegistrationId', 'PersonAttributesId', 'COLUMN'
GO
sp_rename 'dbo.ForeignCitizenCountry.PersonRegistrationId', 'PersonAttributesId', 'COLUMN'
GO
sp_rename 'dbo.UnknownCitizenData.PersonRegistrationId', 'PersonAttributesId', 'COLUMN'
GO
sp_rename 'dbo.HealthInformation.PersonRegistrationId', 'PersonAttributesId', 'COLUMN'
GO
sp_rename 'dbo.PersonProperties.PersonRegistrationId', 'PersonAttributesId', 'COLUMN'
GO
sp_rename 'dbo.PersonName.PersonRegistrationId', 'PersonAttributesId', 'COLUMN'
GO



-----------------------------------------------------------------------------------------
-----  Create new records in PersonAttributes to match records in PersonProperties  -----
-----------------------------------------------------------------------------------------

ALTER TABLE PersonProperties ADD PersonAttributesId_Tmp UNIQUEIDENTIFIER NOT NULL 
CONSTRAINT DF_PersonAttributesId_Tmp DEFAULT(NewID())
GO

INSERT INTO PersonAttributes (PersonAttributesId, PersonRegistrationId, EffectId)
SELECT PersonAttributesId_Tmp, PersonAttributesId, EffectId
FROM PersonProperties
GO

UPDATE PersonProperties SET PersonAttributesId = PersonAttributesId_Tmp
GO

ALTER TABLE PersonProperties DROP 
CONSTRAINT DF_PersonAttributesId_Tmp,FK_PersonProperties_Effect,
COLUMN PersonAttributesId_Tmp, EffectId
GO

------------------------------------------------------------------------------------------
-----  Create new records in PersonAttributes to match records in HealthInformation  -----
------------------------------------------------------------------------------------------

ALTER TABLE HealthInformation ADD PersonAttributesId_Tmp UNIQUEIDENTIFIER NOT NULL 
CONSTRAINT DF_PersonAttributesId_Tmp DEFAULT(NewID())
GO

INSERT INTO PersonAttributes (PersonAttributesId, PersonRegistrationId, EffectId)
SELECT PersonAttributesId_Tmp, PersonAttributesId, EffectId
FROM HealthInformation
GO

UPDATE HealthInformation SET PersonAttributesId = PersonAttributesId_Tmp
GO

ALTER TABLE HealthInformation DROP 
CONSTRAINT DF_PersonAttributesId_Tmp,FK_HealthInformation_Effect,
COLUMN PersonAttributesId_Tmp, EffectId
GO