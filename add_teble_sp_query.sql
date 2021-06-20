USE [EateryDB]
GO

/****** Object:  Table [dbo].[msRecipe]   ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/**
 * Created by: Michael Susilo
 * Date: 15 Jun 2021
 * Purpose: Table Recipe
 */
CREATE TABLE [dbo].[msRecipe](
	[RecipeID] [int] IDENTITY(1,1) NOT NULL,
	[DishID] [int] NOT NULL,
	[RecipeName] [varchar](200) NOT NULL,
	[RecipeDescription] [varchar](MAX) DEFAULT '',
	[AuditedActivity] [char](1) NOT NULL,
	[AuditedTime] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[RecipeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[msRecipe]  WITH CHECK ADD FOREIGN KEY([DishID])
REFERENCES [dbo].[msDish] ([DishID])
GO

/****** Object:  Table [dbo].[msIngridient]   ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/**
 * Created by: Michael Susilo
 * Date: 15 Jun 2021
 * Purpose: Table Ingridient
 */
CREATE TABLE [dbo].[msIngredient](
	[IngredientID] [int] IDENTITY(1,1) NOT NULL,
	[RecipeID] [int] NOT NULL,
	[IngredientName] [varchar](200) NOT NULL,
	[IngredientQty] [int] NOT NULL,
	[IngredientUnit] [varchar](200) NOT NULL,
	[AuditedActivity] [char](1) NOT NULL,
	[AuditedTime] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[IngredientID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[msIngredient]  WITH CHECK ADD FOREIGN KEY([RecipeID])
REFERENCES [dbo].[msRecipe] ([RecipeID])
GO

/****** Object:  StoredProcedure [dbo].[Recipe_Delete] ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/**
 * Created by: Michael Susilo
 * Date: 15 Jun 2021
 * Purpose: Delete Recipe
 */
CREATE PROCEDURE [dbo].[Recipe_Delete]
	@RecipeIDs VARCHAR(MAX)
AS
BEGIN
	UPDATE msRecipe
	SET AuditedActivity = 'D',
		AuditedTime = GETDATE()
	WHERE RecipeID IN (SELECT value FROM fn_General_Split(@RecipeIDs, ','))
END
GO

/****** Object:  StoredProcedure [dbo].[Recipe_InsertUpdate]   ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/**
 * Created by: Michael Susilo
 * Date: 15 Jun 2021
 * Purpose: Insert atau update recipe
 */
CREATE PROCEDURE [dbo].[Recipe_InsertUpdate]
	@RecipeID INT OUTPUT,
	@DishID INT,
	@RecipeName VARCHAR(200)
AS
BEGIN
	DECLARE @RetVal INT
	IF EXISTS (SELECT 1 FROM msRecipe WITH(NOLOCK) WHERE RecipeID = @RecipeID AND AuditedActivity <> 'D')
	BEGIN
		UPDATE msRecipe
		SET RecipeName = @RecipeName,
			DishID = @DishID,
			AuditedActivity = 'U',
			AuditedTime = GETDATE()
		WHERE RecipeID = @RecipeID AND AuditedActivity <> 'D'
		SET @RetVal = @RecipeID
	END
	ELSE
	BEGIN
		INSERT INTO msRecipe
		(RecipeName, DishID, AuditedActivity, AuditedTime)
		VALUES
		(@RecipeName, @DishID, 'I', GETDATE())
		SET @RetVal = SCOPE_IDENTITY()
	END
	SELECT @RecipeId = @RetVal
END
GO

/****** Object:  StoredProcedure [dbo].[Recipe_GetByID]   ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/**
 * Created by: Michael Susilo
 * Date: 15 Jun 2021
 * Purpose: Get 1 recipe by Id
 */
CREATE PROCEDURE [dbo].[Recipe_GetByID]
	@RecipeId INT
AS
BEGIN
	SELECT 
		RecipeID,
		DishID,
		RecipeName,
		RecipeDescription
	FROM msRecipe WITH(NOLOCK)
	WHERE RecipeId = @RecipeId AND AuditedActivity <> 'D'
END
GO

/****** Object:  StoredProcedure [dbo].[Recipe_GetByDish]   ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/**
 * Created by: Michael Susilo
 * Date: 15 Jun 2021
 * Purpose: Get All Recipe by DishID
 */
CREATE PROCEDURE [dbo].[Recipe_GetByDish]
	@DishId INT
AS
BEGIN
	SELECT 
		RecipeID,
		DishID,
		RecipeName,
		RecipeDescription
	FROM msRecipe WITH(NOLOCK)
	WHERE DishId = @DishId AND AuditedActivity <> 'D'
END
GO


/****** Object:  StoredProcedure [dbo].[Ingredient_Delete] ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/**
 * Created by: Michael Susilo
 * Date: 15 Jun 2021
 * Purpose: Delete Ingridient
 */
CREATE PROCEDURE [dbo].[Ingredient_Delete]
	@IngredientIDs VARCHAR(MAX)
AS
BEGIN
	UPDATE msIngredient
	SET AuditedActivity = 'D',
		AuditedTime = GETDATE()
	WHERE IngredientID IN (SELECT value FROM fn_General_Split(@IngredientIDs, ','))
END
GO

/****** Object:  StoredProcedure [dbo].[Ingredient_InsertUpdate]   ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/**
 * Created by: Michael Susilo
 * Date: 15 Jun 2021
 * Purpose: Insert atau update Ingridient
 */
CREATE PROCEDURE [dbo].[Ingredient_InsertUpdate]
	@IngredientID INT OUTPUT,
	@RecipeID INT,
	@IngredientName VARCHAR(200),
	@IngredientQty INT,
	@IngredientUnit VARCHAR(200)
AS
BEGIN
	DECLARE @RetVal INT
	IF EXISTS (SELECT 1 FROM msIngredient WITH(NOLOCK) WHERE IngredientID = @IngredientID AND AuditedActivity <> 'D')
	BEGIN
		UPDATE msIngredient
		SET IngredientName = @IngredientName,
			IngredientQty = @IngredientQty,
			IngredientUnit = @IngredientUnit,
			RecipeID = @RecipeID,
			AuditedActivity = 'U',
			AuditedTime = GETDATE()
		WHERE IngredientID = @IngredientID AND AuditedActivity <> 'D'
		SET @RetVal = @IngredientID
	END
	ELSE
	BEGIN
		INSERT INTO msIngredient
		(IngredientName, IngredientQty, IngredientUnit, RecipeID, AuditedActivity, AuditedTime)
		VALUES
		(@IngredientName, @IngredientQty, @IngredientUnit, @RecipeID, 'I', GETDATE())
		SET @RetVal = SCOPE_IDENTITY()
	END
	SELECT @IngredientID = @RetVal
END
GO

/****** Object:  StoredProcedure [dbo].Ingredient_GetByID]   ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/**
 * Created by: Michael Susilo
 * Date: 15 Jun 2021
 * Purpose: Get 1 ingredient by Id
 */
CREATE PROCEDURE [dbo].[Ingredient_GetByID]
	@IngredientId INT
AS
BEGIN
	SELECT 
		IngredientID,
		RecipeID,
		IngredientName,
		IngredientQty,
		IngredientUnit
	FROM msIngredient WITH(NOLOCK)
	WHERE IngredientId = @IngredientId AND AuditedActivity <> 'D'
END
GO

/****** Object:  StoredProcedure [dbo].[Ingredint_GetByRecipe]   ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/**
 * Created by: Michael Susilo
 * Date: 15 Jun 2021
 * Purpose: Get All Ingredient by RecipeID
 */
CREATE PROCEDURE [dbo].[Ingredient_GetByRecipe]
	@RecipeId INT
AS
BEGIN
	SELECT 
		IngredientID,
		RecipeID,
		IngredientName,
		IngredientQty,
		IngredientUnit
	FROM msIngredient WITH(NOLOCK)
	WHERE RecipeId = @RecipeId AND AuditedActivity <> 'D'
END
GO

/****** Object:  StoredProcedure [dbo].[Recipe_UpdateDescription]   ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/**
 * Created by: Michael Susilo
 * Date: 20 Jun 2021
 * Purpose: update recipe description
 */
CREATE PROCEDURE [dbo].[Recipe_UpdateDescription]
	@RecipeID INT,
	@RecipeDescription VARCHAR(MAX)
AS
BEGIN
	UPDATE msRecipe
	SET RecipeDescription = @RecipeDescription,
		AuditedActivity = 'U',
		AuditedTime = GETDATE()
	WHERE RecipeID = @RecipeID AND AuditedActivity <> 'D'
	
END
GO


