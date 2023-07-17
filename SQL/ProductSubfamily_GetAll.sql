SELECT 
	"U_stec_codFamOITB" as "FamilyCode",
	"U_stec_CodSubFamilia" as "SubFamilyCode",
	"U_stec_nombre" as "SubFamilyName",
	"U_stec_ens" as "Image"
FROM "@STEC_SUBFAMILIA" 
ORDER BY "U_stec_codFamOITB" {0}