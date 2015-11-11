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


--城市对工作资历的需求数
SELECT COUNT(WorkYear)[JobCount],WorkYear,COUNT(city),City FROM dbo.Job 
 --WHERE City='深圳'
 GROUP BY WorkYear,City
 HAVING COUNT(WorkYear)>3
ORDER BY City ,JobCount

--工作资历与薪资分部

SELECT  COUNT(Salary) [Num] ,
        Salary
FROM    dbo.Job
WHERE   WorkYear = '1-3年'
        AND City = '北京'
GROUP BY Salary
ORDER BY Num DESC 



                  
              


                  
     
                  
                  
                  
                  
                  




--城市具体岗位需求数据 (Java)
SELECT TOP 15 COUNT(City)[JobCount],city  FROM dbo.Job WHERE PositionName='java' GROUP BY City ORDER BY JobCount desc











       