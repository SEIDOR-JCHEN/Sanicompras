SELECT 
	t2."ItemCode",
	t2."ItemName" as "Title",
	t2."U_SEI_PS_Talla" as "Size",
	t2."U_SEI_PS_Color" as "Color"
FROM OALI t1
INNER JOIN OITM T2 ON t1."AltItem" = t2."ItemCode"
WHERE t1."OrigItem" = '{0}'