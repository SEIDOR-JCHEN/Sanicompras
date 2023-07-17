SELECT 
	CAST(CONCAT(CONVERT(date, t1."CreateDate"),' ', FORMAT((t1."CreateTS" / 10000) % 100, '00'),':',FORMAT((t1."CreateTS" / 100) % 100, '00'),':',FORMAT(t1."CreateTS" % 100, '00')) as datetime) as "CreateDateTime",
	CAST(CONCAT(COALESCE(CONVERT(date, t1.UpdateDate), CONVERT(date, t1."CreateDate"), '1900-01-01'),' ', FORMAT((COALESCE(t1."UpdateTS",t1."CreateTS", 0) / 10000) % 100, '00'),':',FORMAT((COALESCE(t1."UpdateTS",t1."CreateTS", 0) / 100) % 100, '00'),':',FORMAT(COALESCE(t1."UpdateTS",t1."CreateTS", 0) % 100, '00')) as datetime) as "UpdateDateTime",
	t1."ItemCode",
	t1."ItemName",
	t3."WhsCode",
	t3."WhsName",
	t2."OnHand" as "Quantity"
FROM OITM t1
INNER JOIN OITW t2 ON t1."ItemCode" = t2."ItemCode"
INNER JOIN OWHS t3 ON t2."WhsCode" = T3."WhsCode"
WHERE t2."OnHand" != 0 
AND t1."ItemCode" = '{0}'
ORDER BY t1."ItemCode";