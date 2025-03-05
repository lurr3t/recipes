


CREATE TABLE [tbl_user] (
    [user_id] INT IDENTITY (1,1) NOT NULL,
    [user_name] NVARCHAR (20) NOT NULL,
    [user_password] NVARCHAR (256),
    CONSTRAINT [PK_u_id] PRIMARY KEY ([user_id]),
);



ALTER TABLE tbl_user ALTER COLUMN user_password nvarchar(256);


CREATE TABLE [tbl_shopping_list] (
    [sl_id] INT IDENTITY (1,1) NOT NULL,
    [sl_user_id] INT NOT NULL,
    [sl_title] NVARCHAR (256),
    [sl_created] DATE,
    CONSTRAINT [PK_sl_id] PRIMARY KEY ([sl_id]),
    CONSTRAINT [FK_sl_user_id] FOREIGN KEY ([sl_user_id]) REFERENCES [tbl_user] ([user_id]) ON DELETE CASCADE
);

/*Must be removed manually if user is deleted*/
CREATE TABLE [tbl_recipe] (
    [recipe_id] INT IDENTITY (1,1) NOT NULL,
    [recipe_user_id] INT NOT NULL,
    [recipe_sl_id] INT,
    [recipe_portions] INT,
    [recipe_title] NVARCHAR (256),
    [recipe_description] NVARCHAR (4000),
    [recipe_url] NVARCHAR (1000),
    [recipe_created] DATE,
    [recipe_img_url] NVARCHAR (1000),
    CONSTRAINT [PK_recipe_id] PRIMARY KEY ([recipe_id]),
    CONSTRAINT [FK_recipe_user_id] FOREIGN KEY ([recipe_user_id]) REFERENCES [tbl_user] ([user_id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_recipe_sl_id] FOREIGN KEY ([recipe_sl_id]) REFERENCES [tbl_shopping_list] ([sl_id]) ON DELETE NO ACTION
);

CREATE TABLE [tbl_ingredient] (
  [i_id] INT IDENTITY (1,1) NOT NULL,
  [i_recipe_id] INT,
  [i_sl_id] INT,
  [i_amount] FLOAT,
  [i_checked] INT,
  [i_name] NVARCHAR(256),
  [i_unit] NVARCHAR(20),
  CONSTRAINT [PK_i_id] PRIMARY KEY ([i_id]),
  CONSTRAINT [FK_i_recipe_id] FOREIGN KEY ([i_recipe_id]) REFERENCES tbl_recipe ([recipe_id]) ON DELETE CASCADE,
  CONSTRAINT [FK_i_sl_id] FOREIGN KEY ([i_sl_id]) REFERENCES tbl_shopping_list ([sl_id]) ON DELETE CASCADE
);

ALTER TABLE tbl_ingredient
ADD i_checked INT;

/*
DROP TABLE tbl_ingredient
DROP TABLE tbl_recipe
DROP TABLE tbl_shopping_list
DROP TABLE tbl_user
*/
