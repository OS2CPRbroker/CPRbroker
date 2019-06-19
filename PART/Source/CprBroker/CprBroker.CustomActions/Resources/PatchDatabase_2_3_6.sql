If NOT EXISTS (SELECT * FROM dbo.[Queue] WHERE TypeName like 'CprBroker.Providers.CPRDirect.EnsureSubscriptionQueue%')
	INSERT INTO dbo.[Queue] (TypeId,TypeName,BatchSize,MaxRetry)
	VALUES(102,'CprBroker.Providers.CPRDirect.EnsureSubscriptionQueue, CprBroker.Providers.CPRDirect', 100, 5)