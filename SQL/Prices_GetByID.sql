SELECT 
	CAST(CONCAT(CONVERT(date, t3."CreateDate"),' ', FORMAT((t3."CreateTS" / 10000) % 100, '00'),':',FORMAT((t3."CreateTS" / 100) % 100, '00'),':',FORMAT(t3."CreateTS" % 100, '00')) as datetime) as "CreateDateTime",
	CAST(CONCAT(COALESCE(CONVERT(date, t3."UpdateDate"), CONVERT(date, t3."CreateDate"), '1900-01-01'),' ', FORMAT((COALESCE(t3."UpdateTS",t3."CreateTS", 0) / 10000) % 100, '00'),':',FORMAT((COALESCE(t3."UpdateTS",t3."CreateTS", 0) / 100) % 100, '00'),':',FORMAT(COALESCE(t3."UpdateTS",t3."CreateTS", 0) % 100, '00')) as datetime) as "UpdateDateTime",
	CAST(t2."U_SEI_PS_ListNum" as smallint) as "ListNum",
	t2."ListName",
	t1."ItemCode",
	t3."ItemName",
	ISNULL(t1."Price", 0 ) as "Price",
	t1."Currency",
	t2."BASE_NUM",
	t2."Factor"
FROM ITM1 t1
INNER JOIN OPLN t2 ON t1."PriceList" = t2."ListNum" AND t2."U_SEI_PS_ListNum" > 3
INNER JOIN OITM t3 ON t1."ItemCode" = t3."ItemCode"
WHERE ISNULL(t1."Price", 0 ) > 0
AND t1."ItemCode" = '{0}'
ORDER BY t2."ListNum";