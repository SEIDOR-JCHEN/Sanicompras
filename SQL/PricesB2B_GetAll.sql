SELECT 
	CAST(CONCAT(CONVERT(date, t1."CreateDate"),' ', FORMAT((t1."CreateTS" / 10000) % 100, '00'),':',FORMAT((t1."CreateTS" / 100) % 100, '00'),':',FORMAT(t1."CreateTS" % 100, '00')) as datetime) as "CreateDateTime",
	CAST(CONCAT(COALESCE(CONVERT(date, t1.UpdateDate), CONVERT(date, t1."CreateDate"), '1900-01-01'),' ', FORMAT((COALESCE(t1."UpdateTS",t1."CreateTS", 0) / 10000) % 100, '00'),':',FORMAT((COALESCE(t1."UpdateTS",t1."CreateTS", 0) / 100) % 100, '00'),':',FORMAT(COALESCE(t1."UpdateTS",t1."CreateTS", 0) % 100, '00')) as datetime) as "UpdateDateTime",
	t1."CardCode",
	t1."CardName",
	t2."ItemCode",
	t3."ItemName",
	t2."Price",
	t2."Discount",
	t2."ListNum"
FROM OCRD t1
INNER JOIN OSPP t2 ON t1."CardCode" = t2."CardCode"
INNER JOIN OITM t3 ON t2."ItemCode" = t3."ItemCode"
WHERE t1."CardCode" IN (
	SELECT DISTINCT t1."CardCode"
	FROM OCRD t1
	INNER JOIN OSPP t2 ON t1."CardCode" = t2."CardCode"
	{0}
)
ORDER BY t1."CardCode";