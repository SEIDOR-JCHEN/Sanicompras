SELECT 
	t3."CreateDateTime",
	t3."UpdateDateTime",
	t2."ListNum",
	t2."ListName",
	t1."ItemCode",
	t3."ItemName",
	ISNULL(t1."Price", 0 ) as "Price",
	t1."Currency",
	t2."BASE_NUM",
	t2."Factor"
FROM ITM1 t1
INNER JOIN OPLN t2 ON t1."PriceList" = t2."ListNum"
INNER JOIN (
	SELECT * FROM (
		SELECT DISTINCT
			CAST(CONCAT(CONVERT(date, t1."CreateDate"),' ', FORMAT((t1."CreateTS" / 10000) % 100, '00'),':',FORMAT((t1."CreateTS" / 100) % 100, '00'),':',FORMAT(t1."CreateTS" % 100, '00')) as datetime) as "CreateDateTime",
			CAST(CONCAT(COALESCE(CONVERT(date, t1.UpdateDate), CONVERT(date, t1."CreateDate"), '1900-01-01'),' ', FORMAT((COALESCE(t1."UpdateTS",t1."CreateTS", 0) / 10000) % 100, '00'),':',FORMAT((COALESCE(t1."UpdateTS",t1."CreateTS", 0) / 100) % 100, '00'),':',FORMAT(COALESCE(t1."UpdateTS",t1."CreateTS", 0) % 100, '00')) as datetime) as "UpdateDateTime",
			t1."ItemCode",
			t1."ItemName"
		FROM OITM t1
	) as t
	WHERE ( t."UpdateDateTime" BETWEEN '{0}' AND '{1}' 
	OR t."CreateDateTime" BETWEEN '{0}' AND '{1}' )
) t3 ON t1."ItemCode" = t3."ItemCode"
WHERE ISNULL(t1."Price", 0 ) > 0
AND t2."ListNum" IN (
	SELECT DISTINCT
		t."ListNum"
	FROM OPLN t
	{2}
)
--ORDER BY t3."ItemCode"
ORDER BY t2."ListNum";