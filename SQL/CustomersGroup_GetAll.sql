SELECT 
	"GroupCode",
	"GroupName",
	"PriceList"
FROM OCRG
WHERE "GroupType" = 'C'
ORDER BY "GroupCode" {0}