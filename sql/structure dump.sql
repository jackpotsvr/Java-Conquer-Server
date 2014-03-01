/*
Navicat PGSQL Data Transfer

Source Server         : post
Source Server Version : 90302
Source Host           : localhost:5432
Source Database       : coserver
Source Schema         : public

Target Server Type    : PGSQL
Target Server Version : 90302
File Encoding         : 65001

Date: 2014-03-01 17:00:32
*/


-- ----------------------------
-- Table structure for account
-- ----------------------------
DROP TABLE IF EXISTS "public"."account";
CREATE TABLE "public"."account" (
"account_username" varchar(16) COLLATE "default" NOT NULL,
"password" varchar(16) COLLATE "default" NOT NULL
)
WITH (OIDS=FALSE)

;

-- ----------------------------
-- Table structure for characters
-- ----------------------------
DROP TABLE IF EXISTS "public"."characters";
CREATE TABLE "public"."characters" (
"account_username" varchar(16) COLLATE "default",
"character_name" varchar(16) COLLATE "default" NOT NULL,
"character_level" int4,
"character_experience" int4,
"character_strength" int4,
"character_agility" int4,
"character_vitality" int4,
"character_spirit" int4,
"character_attributepoints" int4,
"character_profession" int4,
"character_mesh" int4,
"character_gold" int4,
"character_cps" int4,
"character_whgold" int4,
"character_spouse" varchar(16) COLLATE "default",
"character_map" int4,
"character_x" int4,
"character_y" int4,
"character_hair" int4,
"character_reborn" int4,
"character_curhp" int4,
"character_curmp" int4
)
WITH (OIDS=FALSE)

;

-- ----------------------------
-- Table structure for item_possession
-- ----------------------------
DROP TABLE IF EXISTS "public"."item_possession";
CREATE TABLE "public"."item_possession" (
"character_name" varchar(16) COLLATE "default",
"item_id" int8,
"item_slot" int4
)
WITH (OIDS=FALSE)

;

-- ----------------------------
-- Table structure for items
-- ----------------------------
DROP TABLE IF EXISTS "public"."items";
CREATE TABLE "public"."items" (
"item_sid" int8 NOT NULL,
"item_name" varchar(32) COLLATE "default",
"item_maxdura" int4,
"item_worth" int4,
"item_cpsworth" int4,
"item_classreq" int4,
"item_profreq" int4,
"item_lvlreq" int4,
"item_sexreq" int4,
"item_strreq" int4,
"item_agireq" int4,
"item_minatk" int4,
"item_maxatk" int4,
"item_defence" int4,
"item_mdef" int4,
"item_mattack" int4,
"item_dodge" int4,
"item_agility" int4
)
WITH (OIDS=FALSE)

;

-- ----------------------------
-- Table structure for unique_items
-- ----------------------------
DROP TABLE IF EXISTS "public"."unique_items";
CREATE TABLE "public"."unique_items" (
"item_id" int8 NOT NULL,
"item_sid" int8,
"item_dura" int4,
"item_firstsocket" int4,
"item_secondsocket" int4,
"item_plus" int4,
"item_bless" int4,
"item_enchant" int4
)
WITH (OIDS=FALSE)

;

-- ----------------------------
-- Alter Sequences Owned By 
-- ----------------------------

-- ----------------------------
-- Primary Key structure for table account
-- ----------------------------
ALTER TABLE "public"."account" ADD PRIMARY KEY ("account_username");

-- ----------------------------
-- Checks structure for table characters
-- ----------------------------
ALTER TABLE "public"."characters" ADD CHECK (character_cps < 1000000000);
ALTER TABLE "public"."characters" ADD CHECK ((character_level >= 1) AND (character_level <= 130));
ALTER TABLE "public"."characters" ADD CHECK (character_gold < 1000000000);
ALTER TABLE "public"."characters" ADD CHECK (character_cps < 1000000000);

-- ----------------------------
-- Primary Key structure for table characters
-- ----------------------------
ALTER TABLE "public"."characters" ADD PRIMARY KEY ("character_name");

-- ----------------------------
-- Primary Key structure for table items
-- ----------------------------
ALTER TABLE "public"."items" ADD PRIMARY KEY ("item_sid");

-- ----------------------------
-- Primary Key structure for table unique_items
-- ----------------------------
ALTER TABLE "public"."unique_items" ADD PRIMARY KEY ("item_id");

-- ----------------------------
-- Foreign Key structure for table "public"."characters"
-- ----------------------------
ALTER TABLE "public"."characters" ADD FOREIGN KEY ("account_username") REFERENCES "public"."account" ("account_username") ON DELETE NO ACTION ON UPDATE NO ACTION;

-- ----------------------------
-- Foreign Key structure for table "public"."item_possession"
-- ----------------------------
ALTER TABLE "public"."item_possession" ADD FOREIGN KEY ("item_id") REFERENCES "public"."unique_items" ("item_id") ON DELETE RESTRICT ON UPDATE RESTRICT;

-- ----------------------------
-- Foreign Key structure for table "public"."unique_items"
-- ----------------------------
ALTER TABLE "public"."unique_items" ADD FOREIGN KEY ("item_sid") REFERENCES "public"."items" ("item_sid") ON DELETE NO ACTION ON UPDATE NO ACTION;
