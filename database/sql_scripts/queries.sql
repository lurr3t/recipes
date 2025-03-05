-- Remember the recipe_user_id in cookies --

-- Create user --
INSERT INTO [tbl_user]
(user_name, user_password)
VALUES ('xd', 'xd')

-- Create recipe --
INSERT INTO [tbl_recipe]
    ([recipe_user_id], [recipe_title], [recipe_description], [recipe_url], [recipe_created], [recipe_img_url])
VALUES (@userId, @recipeName, @recipeDesc, @recipeUrl, CAST( GETDATE() AS Date ), @imgUrl);
SELECT CAST(scope_identity() AS int);

-- Create ShoppingList --
INSERT INTO [tbl_shopping_list]
([sl_user_id], [sl_created], [sl_title])
VALUES (@userId, GETDATE(), @title)
SELECT CAST(scope_identity() AS int);

-- Create Ingredient --
INSERT INTO [tbl_ingredient]
    ([i_recipe_id], [i_amount], [i_name], [i_unit])
VALUES (@rId, @Amount, @Name, @Unit);

-- Create user --
INSERT INTO [tbl_user]
([user_name])
VALUES (@username)
SELECT CAST(scope_identity() AS int);

-- Update user --
UPDATE [tbl_user] SET
[user_password] = @password
WHERE user_name = @username

-- update --
UPDATE [tbl_recipe] SET
[recipe_title] = @title,
[recipe_description] = @description,
[recipe_img_url] = @imgUrl,
[recipe_portions] = @portions
WHERE recipe_id=@id;

UPDATE [tbl_ingredient] SET
[i_amount] = @amount,
[i_name] = @name,
[i_unit] = @unit
WHERE i_id = @id;

-- Read, search --
Declare @input varchar(1000)
SET @input = ''
Declare @sort varchar(1000)
SET @sort = 'tbl_recipe.recipe_created'
DECLARE @user int
SET @user = 6
SELECT recipe_id FROM tbl_recipe
INNER JOIN tbl_ingredient ON tbl_ingredient.i_recipe_id = tbl_recipe.recipe_id
INNER JOIN tbl_user ON tbl_user.user_id = tbl_recipe.recipe_user_id
WHERE (user_id=@user) AND (
      (recipe_title LIKE '%' + @input + '%') OR
      (recipe_description LIKE '%' + @input + '%') OR
      (recipe_created LIKE '%' + @input + '%') OR
      (i_amount LIKE '%' + @input + '%') OR
      (i_name LIKE '%' + @input + '%') OR
      (i_unit LIKE '%' + @input + '%'))
GROUP BY recipe_id, recipe_title, recipe_description, recipe_created
ORDER BY recipe_created DESC

-- Read, search shoppingList --
Declare @input varchar(1000)
SET @input = 'test'
Declare @sort varchar(1000)
SET @sort = 'tbl_shopping_list.sl_created'
DECLARE @user int
SET @user = 1
SELECT sl_id FROM tbl_shopping_list
FULL JOIN tbl_ingredient ON tbl_ingredient.i_sl_id = tbl_shopping_list.sl_id
INNER JOIN tbl_user ON tbl_user.user_id = tbl_shopping_list.sl_user_id
WHERE (user_id=@user) AND
      (sl_title LIKE '%' + @input + '%') OR
      (sl_created LIKE '%' + @input + '%') OR
      (i_amount LIKE '%' + @input + '%') OR
      (i_name LIKE '%' + @input + '%') OR
      (i_unit LIKE '%' + @input + '%')
GROUP BY sl_id, sl_created, sl_title
ORDER BY sl_created DESC
