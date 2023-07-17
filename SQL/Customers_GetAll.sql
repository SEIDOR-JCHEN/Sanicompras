
SELECT 
	CAST(CONCAT(CONVERT(date, t1."CreateDate"),' ', FORMAT((t1."CreateTS" / 10000) % 100, '00'),':',FORMAT((t1."CreateTS" / 100) % 100, '00'),':',FORMAT(t1."CreateTS" % 100, '00')) as datetime) as "CreateDateTime",
	CAST(CONCAT(COALESCE(CONVERT(date, t1.UpdateDate), CONVERT(date, t1."CreateDate"), '1900-01-01'),' ', FORMAT((COALESCE(t1."UpdateTS",t1."CreateTS", 0) / 10000) % 100, '00'),':',FORMAT((COALESCE(t1."UpdateTS",t1."CreateTS", 0) / 100) % 100, '00'),':',FORMAT(COALESCE(t1."UpdateTS",t1."CreateTS", 0) % 100, '00')) as datetime) as "UpdateDateTime",
	t1."CardCode",
	t1."CardName", 
	t1."U_SEI_PS_Nombre" as "Nombre",
	t1."U_SEI_PS_Apellido" as "Apellido",
	t1."CardType",
	t1."GroupCode",
	t11."GroupName",
	t1."E_Mail", 
	t1."Phone1",
	t1."Phone2",
	t1."LicTradNum",
	CAST(t12."U_SEI_PS_ListNum" as smallint) as "ListNum",
	t12."ListName",
	CONCAT(t1.CardCode,'_',t2.AdresType,'_',t2.LineNum) as "AddressCode",
	t2."LineNum",
	t2."AdresType",
	t2."Address",
	t2."Street",
	t2."ZipCode",
	t2."City",
	t2."County",
	t2."Country",
	COALESCE(NULLIF(CAST(t2."Building" as varchar),''), t1."Phone1", t1."Phone2") as "Adress_phone" 
FROM OCRD t1
LEFT JOIN OCRG t11 ON t1."GroupCode" = t11."GroupCode"
LEFT JOIN OPLN t12 ON t1."ListNum" = t12."ListNum" AND t12."U_SEI_PS_ListNum" > 3
LEFT JOIN CRD1 t2 ON t1."CardCode" = t2."CardCode" AND DATALENGTH(COALESCE(NULLIF(CAST(t2."Building" as varchar),''), t1."Phone1", t1."Phone2")) > 0
WHERE t1."CardCode" IN (
	SELECT DISTINCT
		t1."CardCode"
	FROM OCRD t1
	LEFT JOIN CRD1 t2 ON t1."CardCode" = t2."CardCode"
	WHERE t1."CardType" = 'C'
	AND ( t1."E_Mail" IS NOT NULL AND t1."E_Mail" != '')
	{0}
)
ORDER BY t1."CardCode";