SELECT 
	COALESCE(t2.CreateDate, '1900-01-01') as "CreateDateTime" ,
	COALESCE(t2.UpdateDate, '1900-01-01') as "UpdateDateTime",
	t1."CardCode",
	t1."CardName",
	t2."ItemCode",
	t3."ItemName",
	t2."Price",
	t2."Discount",
	t2."ListNum"
FROM 
OCRD t1 INNER JOIN OSPP t2 ON t1."CardCode" = t2."CardCode"
INNER JOIN OITM t3 ON t2."ItemCode" = t3."ItemCode"
WHERE t1."CardCode" IN (
	SELECT DISTINCT t1."CardCode"
	FROM OCRD t1
	INNER JOIN OSPP t2 ON t1."CardCode" = t2."CardCode"
	{2}
) 
AND ( t2.UpdateDate BETWEEN '{0}' AND '{1}' 
OR t2.CreateDate BETWEEN '{0}' AND '{1}' )
ORDER BY t1."CardCode";

