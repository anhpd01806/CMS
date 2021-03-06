USE [CMS]
GO
/****** Object:  StoredProcedure [dbo].[PROC_GetListNewsInHome]    Script Date: 19/06/2017 7:11:43 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROC [dbo].[PROC_GetListNewsInHome]
(
	@UserId int, 
	@CateId int = 0, 
	@DistricId int = 0, 
	@StatusId int = 0, 
	@SiteId int = 0,
	@BackDate int = NULL, 
	@From datetime = NULL, 
	@To datetime = NULL, 
	@MinPrice decimal = NULL, 
	@MaxPrice decimal = NULL, 
	@pageIndex int = 1, 
	@pageSize int = 20, 
	@IsRepeat bit = 0, 
	@key nvarchar(255) = NULL, 
	@NameOrder nvarchar(200) = NULL, 
	@descending bit = 0, 
	@total int output
)
AS
BEGIN
	DECLARE @Query nvarchar(MAX)
	DECLARE @TotalQR nvarchar(MAX)
	DECLARE @TotalTB table (
		Total int
	)
	DECLARE @temp table (
		Id int,
		Title nvarchar(1000),
		CategoryId int,
		SiteId int,
		Link nvarchar(500),
		Phone nvarchar(500),
		Contents nvarchar(max),
		Price decimal,
		PriceText nvarchar(100),
		DistrictId int,
		DistictName nvarchar(200),
		StatusId int,
		StatusName nvarchar(200),
		CreatedOn datetime,
		CusIsReaded bit,
		IsRepeat bit,
		RepeatTotal int, 
		Iscc bit,
		SiteName nvarchar(100)
	)
	IF OBJECT_ID('TempDB.dbo.#temp') IS NOT NULL
		DROP TABLE #temp

	IF OBJECT_ID('TempDB.dbo.#tempIsread') IS NOT NULL
		DROP TABLE #tempIsread

	--SELECT c.NewsId INTO #tempIsread FROM News_Customer_Mapping c WHERE c.CustomerId = @UserId AND c.IsReaded = 1
	--(CASE 
	--	WHEN c.Id  IN (SELECT NewsId FROM #tempIsread) THEN 1 
	--	WHEN c.Id NOT IN (SELECT NewsId FROM #tempIsread) THEN 0 
	-- END) AS CusIsReaded,

	SET @Query = N';WITH tblTmp AS (
	SELECT Id,
	Title,
	CategoryId,
	SiteId,
	Link,
	Phone,
	Contents,
	Price,
	PriceText,
	DistrictId,
	DistictName,
	StatusId,
	StatusName,
	CreatedOn,
	CusIsReaded,
	IsRepeat,
	RepeatTotal, 
	Iscc,
	SiteName, ROW_NUMBER()
    OVER(' 
	IF(@NameOrder IS NULL OR @NameOrder = '')
	BEGIN 
		IF (@IsRepeat = 0)
		BEGIN	
			SET @Query = @Query + N' ORDER BY CAST(CreatedOn AS DATE)  DESC, Id DESC '
		END
		ELSE
		BEGIN 
			SET @Query = @Query + N' ORDER BY Phone DESC'
		END
	END
	ELSE
	BEGIN
		IF (@descending = 1)
		BEGIN	
			SET @Query = @Query + N' ORDER BY ' + @NameOrder + N' DESC'
		END
		ELSE
		BEGIN 
			SET @Query = @Query + N' ORDER BY ' + @NameOrder + N' ASC'
		END
	END
	
	SET @Query = @Query + N') AS Row
	FROM ( SELECT DISTINCT
	c.Id,
	c.Title,
	c.CategoryId,
	c.SiteId,
	c.Link,
	c.Phone,
	c.Contents,
	c.Price,
	c.PriceText,
	d.Id as DistrictId,
	d.Name as DistictName,
	t.Id as StatusId,
	t.Name as StatusName,
	c.CreatedOn,
	(CASE 
		WHEN 0 = 0 THEN 0
		WHEN 0 = 0 THEN 0 
	 END) AS CusIsReaded,
	c.IsRepeat,
	(CASE 
		WHEN c.TotalRepeat IS NOT NULL THEN c.TotalRepeat
		ELSE 0
    END) AS RepeatTotal,
	(CASE 
		WHEN (nac.Iscc IS NOT NULL) AND ((nac.Iscc) = 1) THEN 1
		WHEN NOT ((nac.Iscc IS NOT NULL) AND ((nac.Iscc) = 1)) THEN 0
		ELSE NULL
    END) AS [Iscc],
	s.Name as SiteName
	FROM News c WITH (NOLOCK, INDEX(IX_KeyUni,IX_search_date,IX_Search_Price, IX_search_Key))
	JOIN District d WITH(INDEX(PK_District)) ON c.DistrictId = d.Id
	JOIN NewsStatus t WITH(INDEX(PK_NewsStatus_1)) ON c.StatusId = t.Id
	JOIN [Site] s WITH(INDEX(PK_Site)) ON c.SiteId = s.ID
	JOIN Category ct WITH(INDEX(PK__Category__3214EC076FE651AC)) ON ct.Id = c.CategoryId
	LEFT JOIN News_Customer_Mapping ncm WITH (NOLOCK, INDEX(PK_News_Customer_Mapping_1)) ON c.Id = ncm.NewsId
	LEFT JOIN News_customer_action nac WITH (NOLOCK, INDEX(PK_News_customer_action)) ON c.Id = nac.NewsId
	WHERE c.CreatedOn IS NOT NULL AND c.IsDeleted = 0 AND c.IsSpam = 0 
	AND s.Deleted = 0 AND s.Published = 1 AND d.IsDeleted = 0 AND d.Published = 1
	AND c.Id NOT IN (SELECT DISTINCT c.NewsId FROM News_Trash c WHERE (c.IsDeleted = 1 OR c.Isdelete = 1))
	AND c.Id NOT IN (SELECT c.NewsId FROM News_Customer_Mapping c WHERE c.CustomerId = ' + CAST(@UserId as nvarchar(30)) + N' AND (c.IsDeleted = 1 OR c.IsSaved = 1 OR c.IsAgency = 1))'
	
	--where o tren (check tin da xoa)
	-- AND c.Id NOT IN (SELECT DISTINCT c.NewsId FROM News_Trash c WHERE (c.IsDeleted = 1 OR c.Isdelete = 1))

	--AND c.Id NOT IN (SELECT c.NewsId FROM News_Trash c WHERE (c.IsDeleted = 1 OR c.Isdelete = 1) AND c.CustomerID = ' +  CAST(@UserId as nvarchar(30)) + N')
	--BEGIN WHERE
	IF (@CateId <> 0)
	BEGIN
		SET @Query = @Query + N' 
		AND (ct.Id = ' + CAST(@CateId as nvarchar(30)) + ' OR ct.ParentCategoryId = ' + CAST(@CateId as nvarchar(30)) + ')'
	END
	IF (@DistricId <> 0)
	BEGIN
		SET @Query = @Query + N' 
		AND c.DistrictId = ' +CAST(@DistricId as nvarchar(30))
	END
	IF (@SiteId <> 0)
	BEGIN
		SET @Query = @Query + N' AND c.SiteId = ' +CAST(@SiteId as nvarchar(30))
	END
	IF (@StatusId <> 0)
	BEGIN
		SET @Query = @Query + N' AND c.StatusId = ' +CAST(@StatusId as nvarchar(30))
	END
	IF (@BackDate = 0)
		BEGIN
			SET @Query = @Query + N' AND c.CreatedOn >= CONVERT(DateTime, DATEDIFF(DAY, 0, GETDATE()))'
		END
	IF (@BackDate > 0)
		BEGIN
			SET @Query = @Query + N' AND DATEDIFF(DAY, c.CreatedOn, GETDATE()) = ' + CAST(@BackDate as nvarchar(30))
		END

	IF (@From IS NOT NULL)
	BEGIN	
		SET @Query = @Query + N' AND DATEDIFF(DAY, c.CreatedOn, ''' + CONVERT(VARCHAR(24),@From,120) + N''') <= 0'
	END
	IF (@To IS NOT NULL)
	BEGIN	
		SET @Query = @Query + N' AND DATEDIFF(DAY, c.CreatedOn, ''' + CONVERT(VARCHAR(24),@To,120) + N''') >= 0'
	END
	IF (@MinPrice IS NOT NULL)
	BEGIN	
		SET @Query = @Query + N' AND c.Price >= ' +CAST(@MinPrice as nvarchar(30))
	END
	IF (@MaxPrice IS NOT NULL)
	BEGIN	
		SET @Query = @Query + N' AND c.Price <= ' +CAST(@MaxPrice as nvarchar(30))
	END
	IF (@key IS NOT NULL OR @NameOrder = '')
	BEGIN	
		SET @Query = @Query + N' AND (c.Title like N''%' + @key +'%'' OR c.Contents like N''%' + @key +'%'' OR c.Phone like N''%' + @key +'%'' OR d.Name like N''%' + @key +'%'')'
	END
	IF (@IsRepeat = 0)
	BEGIN	
		SET @Query = @Query + N' AND c.IsRepeat = 0'
	END

	--END 

	SET @TotalQR = @Query + N') as tbl) SELECT COUNT(Id) AS Total FROM tblTmp'

	SET @Query = @Query + N')as tbl) 
	SELECT DISTINCT
	Id,
	Title,
	CategoryId,
	SiteId,
	Link,
	Phone,
	Contents,
	Price,
	PriceText,
	DistrictId,
	DistictName,
	StatusId,
	StatusName,
	CreatedOn,
	CusIsReaded,
	IsRepeat,
	RepeatTotal, 
	Iscc,
	SiteName
	FROM tblTmp
    WHERE Row > ' + CAST(((@pageIndex - 1) * @pageSize) as nvarchar(100))+' AND Row <= ' + CAST((@pageIndex * @pageSize) as nvarchar(100))
	
	INSERT INTO @TotalTB  EXEC(@TotalQR)
	INSERT INTO @temp EXEC(@Query)

	print(@Query)

	SET @total = (SELECT Total FROM @TotalTB)
	SELECT * FROM @temp
END

	