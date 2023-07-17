SELECT 
	CAST(CONCAT(CONVERT(date, t1."CreateDate"),' ', FORMAT((t1."CreateTS" / 10000) % 100, '00'),':',FORMAT((t1."CreateTS" / 100) % 100, '00'),':',FORMAT(t1."CreateTS" % 100, '00')) as datetime) as "CreateDateTime",
	CAST(CONCAT(COALESCE(CONVERT(date, t1.UpdateDate), CONVERT(date, t1."CreateDate"), '1900-01-01'),' ', FORMAT((COALESCE(t1."UpdateTS",t1."CreateTS", 0) / 10000) % 100, '00'),':',FORMAT((COALESCE(t1."UpdateTS",t1."CreateTS", 0) / 100) % 100, '00'),':',FORMAT(COALESCE(t1."UpdateTS",t1."CreateTS", 0) % 100, '00')) as datetime) as "UpdateDateTime",
	t1."DocEntry",
	t1."DocNum",
	t1."DocDate", 
	t1."DocDueDate", 
	t1."DocTotal",
	t1."DocStatus",
	t1."CANCELED",
	t1."CardCode",
	t2."ItemCode",
	t2."Dscription", 
	t2."Price", 
	t2."DiscPrcnt",
	t2."Quantity",
	t2."VatGroup", 
	t2."LineTotal"
FROM ORDR t1
INNER JOIN RDR1 t2 ON t1."DocEntry" = t2."DocEntry"
WHERE t1."DocEntry" = {0}
ORDER BY t1."DocEntry";