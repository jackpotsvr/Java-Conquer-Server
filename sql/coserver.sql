DROP TABLE characters;
DROP TABLE account;


CREATE TABLE account
(
	account_username varchar(16)
		CONSTRAINT pk_account PRIMARY KEY,
	password varchar(16) -- TO BE REPLACED WITH SHA-512 HASHED PASSWORD WITH SALT
		CONSTRAINT nn_password NOT NULL
);

CREATE TABLE characters 
(
	account_username 		varchar(16),
	character_name 			varchar(16)
		CONSTRAINT pk_charr PRIMARY KEY,

	character_level 		INTEGER
		CHECK (character_level >= 1 AND character_level <= 130),

	character_experience 		INTEGER,

	character_strength		INTEGER,
	character_agility		INTEGER,
	character_vitality		INTEGER,
	character_spirit		INTEGER,
	character_attributepoints 	INTEGER, 

	character_profession 		INTEGER,
	character_model			INTEGER, 

	character_gold			INTEGER
		CHECK (character_gold < 1000000000),				
	character_cps			INTEGER
		CHECK (character_cps < 1000000000),
	character_whgold		INTEGER
		CHECK (character_cps < 1000000000),

	
	character_spouse 		varchar(16),
	
	character_map			INTEGER,
	character_x			INTEGER,
	character_y			INTEGER,

	character_hair			INTEGER,

	character_reborn		INTEGER,
	character_avatar		INTEGER,

	CONSTRAINT fk_account FOREIGN KEY (account_username) REFERENCES account
	
);

CREATE TABLE items
(
	item_SID 				INTEGER
		CONSTRAINT pk_item PRIMARY KEY, 
	item_name				varchar(32),
	item_maxDura				INTEGER,
	item_worth				INTEGER,
	item_CPSworth				INTEGER,

	item_classReq				INTEGER,
	item_profReq				INTEGER,
	item_lvlReq				INTEGER,
	item_sexReq				INTEGER,
	item_strReq				INTEGER,
	item_agiReq				INTEGER,
	item_minxAtk				INTEGER,
	item_maxAtk				INTEGER,
	item_defence				INTEGER,
	item_mDef				INTEGER,
	item_mAttack				INTEGER,
	item_dodge				INTEGER,
	item_agility				INTEGER	
);

CREATE TABLE unique_items
(
	item_ID					INTEGER
		CONSTRAINT pk_unique_item PRIMARY KEY,
	item_SID				INTEGER,

	item_dura				INTEGER,
	item_firstSocket			INTEGER,
	item_secondSocket			INTEGER,
	item_plus				INTEGER,
	item_bless				INTEGER,
	item_enchant				INTEGER,

	CONSTRAINT fk_unique_items_sid FOREIGN KEY (item_sid) REFERENCES items
);

CREATE TABLE item_possession
(
	character_name 			varchar(16),
	item_ID					INTEGER,
	item_slot 				INTEGER, -- 0 is invetory	http://conquerwiki.com/wiki/Item_Position_Enum
	
	CONSTRAINT fk_unique_items_id FOREIGN KEY (item_id) REFERENCES items
)


