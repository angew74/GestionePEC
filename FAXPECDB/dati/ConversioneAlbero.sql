select ID_REFERRAL AS VALUE, CASE
 WHEN REFERRAL_TYPE in ('PA','AZ_PRI','AZ_CP','GRP') then RAGIONE_SOCIALE else UFFICIO end as TEXT,
  REFERRAL_TYPE AS SUBTYPE, 'RUBR' AS SOURCE, ID_PADRE AS PADRE from
    [FAXPEC].[FAXPEC].[rubr_entita] where REFERRAL_TYPE in 
	('PA','PA_SUB','PA_UFF','AZ_PRI','AZ_CP','AZ_UFF','GRP') and
	 level <=1 start with ID_REFERRAL = 0 connect by
 NOCYCLE prior ID_REFERRAL = ID_PADRE;



with tree(value, text, subtype, source, padre, level) as (
 select ID_REFERRAL AS VALUE, CASE
 WHEN REFERRAL_TYPE in ('PA','AZ_PRI','AZ_CP','GRP') then RAGIONE_SOCIALE else UFFICIO end as TEXT,
  REFERRAL_TYPE AS SUBTYPE, 'RUBR' AS SOURCE, ID_PADRE AS PADRE, 1 as level from
    [FAXPEC].[FAXPEC].[rubr_entita] as father where REFERRAL_TYPE in 
	('PA','PA_SUB','PA_UFF','AZ_PRI','AZ_CP','AZ_UFF','GRP') 
	union all 
	select 
	 child.ID_REFERRAL AS VALUE, CASE
 WHEN REFERRAL_TYPE in ('PA','AZ_PRI','AZ_CP','GRP') then child.RAGIONE_SOCIALE else child.UFFICIO end as TEXT,
  child.REFERRAL_TYPE AS SUBTYPE, 'RUBR' AS SOURCE, child.ID_PADRE AS PADRE, parent.level +1 from
    [FAXPEC].[FAXPEC].[rubr_entita] as child join tree parent on parent.value=child.ID_PADRE where REFERRAL_TYPE in 
	('PA','PA_SUB','PA_UFF','AZ_PRI','AZ_CP','AZ_UFF','GRP') 
	)
	select *  from tree where level <= 5 and tree.value >= 0 order by tree.level asc option (maxrecursion 20000);


WITH T_TREE_C(ID_REF, REF_ORG, LIV) AS (SELECT ID_REFERRAL, REF_ORG, 
LEVEL 
FROM  [FAXPEC].[FAXPEC].[RUBR_ENTITA]  CONNECT BY NOCYCLE ID_REFERRAL = 
PRIOR ID_PADRE START WITH ID_REFERRAL = 102717),

 T_TREE(ID_REF, LIV)
 AS (SELECT ID_REF, LIV FROM T_TREE_C WHERE REF_ORG IS NOT NULL OR (REF_ORG IS NULL AND 
 LIV = (SELECT MIN(LIV) FROM T_TREE_C WHERE REF_ORG IS NULL))) 
 SELECT DISTINCT ID_REFERRAL as "ID_REF", ID_PADRE AS "ID_PAD", 
 DECODE((SELECT COUNT(*) FROM RUBR_ENTITA WHERE ID_PADRE = RE.ID_REFERRAL), 0, 0, 1) AS "IS_PADRE", 
 CASE WHEN REFERRAL_TYPE IN ('AZ_PF', 'AZ_UFF_PF', 'PA_PF','PA_UFF_PF', 'PF', 'PG') THEN COGNOME||' '||NOME 
 WHEN REFERRAL_TYPE IN ('AZ_UFF', 'PA_UFF') THEN UFFICIO WHEN DISAMB_PRE IS NOT NULL THEN 
 RTRIM(DISAMB_PRE||' '||RAGIONE_SOCIALE||' '||DISAMB_POST) ELSE RTRIM(RAGIONE_SOCIALE||' '||DISAMB_POST) END 
 AS "RAG_SOC", 'RUBR' AS "SRC", REFERRAL_TYPE AS "REF_TYP" FROM  [FAXPEC].[FAXPEC].[RUBR_ENTITA] RE WHERE ID_REFERRAL IN
 (SELECT ID_REF FROM T_TREE) OR ID_PADRE IN (SELECT ID_REF FROM T_TREE WHERE LIV > 1);


 WITH T_TREE_C(ID_REF,REF_ORG,LIV) AS 
 (SELECT ID_REFERRAL, REF_ORG,1 AS LEVEL FROM [FAXPEC].[FAXPEC].[RUBR_ENTITA]
 WHERE ID_REFERRAL=12
 UNION ALL SELECT CHILD.ID_REFERRAL, CHILD.REF_ORG,parent.LIV +1 
 FROM [FAXPEC].[FAXPEC].[RUBR_ENTITA] as CHILD join T_TREE_C parent on parent.ID_REF=child.ID_PADRE),
 T_TREE(ID_REF,LIV)
 AS (SELECT ID_REF,LIV FROM T_TREE_C WHERE REF_ORG IS NOT NULL OR (REF_ORG IS NULL AND 
 LIV = (SELECT MIN(LIV) FROM T_TREE_C WHERE REF_ORG IS NULL)))
 SELECT DISTINCT ID_REFERRAL AS "ID_REF", ID_PADRE AS "ID_PAD",
 IIF(((SELECT COUNT(*) FROM [FAXPEC].[FAXPEC].[RUBR_ENTITA] WHERE ID_PADRE = RE.ID_REFERRAL) = 0), 0, 1) AS "IS_PADRE", 
 CASE WHEN REFERRAL_TYPE IN ('AZ_PF', 'AZ_UFF_PF', 'PA_PF','PA_UFF_PF', 'PF', 'PG') THEN COGNOME+' '+NOME 
 WHEN REFERRAL_TYPE IN ('AZ_UFF', 'PA_UFF') THEN UFFICIO WHEN DISAMB_PRE IS NOT NULL THEN 
 RTRIM(DISAMB_PRE+' '+RAGIONE_SOCIALE+' '+DISAMB_POST) ELSE RTRIM(RAGIONE_SOCIALE +' '+DISAMB_POST) END 
 AS "RAG_SOC", 'RUBR' AS "SRC", REFERRAL_TYPE AS "REF_TYP" FROM  [FAXPEC].[FAXPEC].[RUBR_ENTITA] RE WHERE ID_REFERRAL IN
 (SELECT ID_REF FROM T_TREE) OR ID_PADRE IN (SELECT ID_REF FROM T_TREE WHERE LIV > 1);


 SELECT DISTINCT ID_REFERRAL as "ID_REF", ID_PADRE AS "ID_PAD", 
 IIF((SELECT COUNT(*) FROM  [FAXPEC].[FAXPEC].[RUBR_ENTITA] WHERE ID_PADRE = RE.ID_REFERRAL)= 0, 0, 1) AS "IS_PADRE", 
 CASE WHEN REFERRAL_TYPE IN ('AZ_PF', 'AZ_UFF_PF', 'PA_PF','PA_UFF_PF', 'PF', 'PG') THEN COGNOME +' '+NOME WHEN 
 REFERRAL_TYPE IN ('AZ_UFF', 'PA_UFF') THEN
  UFFICIO WHEN DISAMB_PRE IS NOT NULL THEN RTRIM(DISAMB_PRE+' '+RAGIONE_SOCIALE+' '+DISAMB_POST) ELSE
   RTRIM(RAGIONE_SOCIALE+' '+DISAMB_POST) END AS "RAG_SOC", 'RUBR' AS "SRC", 
   REFERRAL_TYPE AS "REF_TYP" FROM  [FAXPEC].[FAXPEC].[RUBR_ENTITA] RE 
 WHERE ID_PADRE = 99985 ORDER BY RAG_SOC
 