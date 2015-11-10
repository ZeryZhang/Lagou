SELECT * FROM dbo.Job 
--城市工作需求数据
SELECT TOP 15
        COUNT(City) [JobCount] ,
        City 
FROM    dbo.Job
GROUP BY City 
ORDER BY JobCount DESC

--城市公司数据
SELECT City,COUNT(DISTINCT CompanyName) AS CompanyNum
FROM Job
GROUP BY City  ORDER BY CompanyNum DESC 

--工作资历与薪资分部

SELECT COUNT(Salary)[Num],Salary FROM dbo.Job WHERE WorkYear='1-3年'
GROUP BY Salary ORDER BY  Num DESC 


       