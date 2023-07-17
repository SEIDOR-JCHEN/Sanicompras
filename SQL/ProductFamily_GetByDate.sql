SELECT 
	T1."ItmsGrpCod" as "FamilyCode",
	T1."ItmsGrpNam" as "FamilyName",
	T1."U_stec_enf" as "Image",
	T1."U_tbi_traspatienda" as "ToShop",
	T1."U_tbi_grupoclientes" as "GrupoClientes",
	T3."U_SEI_PS_ListNum" as "PriceList",
	T1."createDate" as "CreateDateTime",
	T1."updateDate" as "UpdateDateTime"
FROM OITB T1 LEFT JOIN OCRG T2 ON T2."GroupName" = T1."U_tbi_grupoclientes"
INNER JOIN OPLN T3 ON T2."PriceList" = T3."ListNum"
WHERE ( T1."createDate" BETWEEN '{0}' AND '{1}' 
OR T1."updateDate" BETWEEN '{0}' AND '{1}' ) 
ORDER BY T1."ItmsGrpCod" {2};
