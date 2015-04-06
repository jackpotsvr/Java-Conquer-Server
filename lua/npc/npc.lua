local M = {}

local INITIAL_DIALOG = -1
local DIALOG_QUIT = 255

local aggregrate; 

local function NPC_INITIALIZE(value) 
	aggregrate = value; 
end
	
local function NPC_SAY(message) 
	aggregrate:npcSay(message)
end

local function NPC_LINK1(dialogNumber, value) 
	aggregrate:npcLink1(dialogNumber, value)
end

local function NPC_LINK2(dialogNumber, text) 
	aggregrate:npcLink2(dialogNumber, text)
end

local function NPC_SETFACE(face) 
	aggregrate:npcSetFace(face)
end

local function NPC_FINISH() 
	aggregrate:npcFinish()
end

M.INITIAL_DIALOG = INITIAL_DIALOG
M.DIALOG_QUIT = DIALOG_QUIT
M.NPC_INITIALIZE = NPC_INITIALIZE
M.NPC_SAY = NPC_SAY
M.NPC_LINK1 = NPC_LINK1
M.NPC_LINK2 = NPC_LINK2
M.NPC_SETFACE = NPC_SETFACE
M.NPC_FINISH = NPC_FINISH


return M