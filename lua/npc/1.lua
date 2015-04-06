local npc = require("lua.npc.npc")

function handle(aggregrate, currentDialogNumber)  
	npc.NPC_INITIALIZE(aggregrate)
	
	if(currentDialogNumber == npc.INITIAL_DIALOG) then
		npc.NPC_SAY("Where are you heading for? I can teleport you for a price of 100 silver.")
		npc.NPC_LINK1(1, "Phoenix Castle")
		npc.NPC_LINK1(2, "Desert City")
		npc.NPC_LINK1(3, "Ape Mountain")
		npc.NPC_LINK1(4, "Bird City")
		npc.NPC_LINK1(5, "Mine Cave")
		npc.NPC_LINK1(6, "Market")
		npc.NPC_LINK1(npc.DIALOG_QUIT, "Just passing by.")
		npc.NPC_SETFACE(1)
		npc.NPC_FINISH()
	end
end