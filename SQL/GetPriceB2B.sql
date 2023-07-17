SELECT 
	t1."CardCode",
	t1."CardName",
	t2."Price"
FROM OCRD t1
INNER JOIN OSPP t2 ON t1."CardCode" = t2."CardCode"
WHERE t2."ItemCode" = '{0}'