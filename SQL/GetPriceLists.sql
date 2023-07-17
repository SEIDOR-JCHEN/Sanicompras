SELECT 
	CAST(t2."U_SEI_PS_ListNum" as smallint) as "ListNum",
	t2."ListName",
	ISNULL(t1."Price", 0 ) as "Price",
	t1."Currency"
FROM ITM1 t1
INNER JOIN OPLN t2 ON t1."PriceList" = t2."ListNum" AND t2."U_SEI_PS_ListNum" > 3
WHERE t1."ItemCode" = '{0}' AND ISNULL(t1."Price", 0 ) > 0