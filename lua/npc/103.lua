local npc = require("lua.npc.npc")

function handle(aggregrate)  
	npc.NPC_INITIALIZE(aggregrate)
	currentDialogNumber = aggregrate:getCurrentDialogNumber()
	
	if(currentDialogNumber == npc.INITIAL_DIALOG) then
		npc.NPC_SAY("Where are you heading for? I can teleport you for a price of 100 silver.")
		npc.NPC_LINK1(1, "Phoenix Castle")
		npc.NPC_LINK1(2, "Desert City")
		npc.NPC_LINK1(3, "Ape Mountain")
		npc.NPC_LINK1(4, "Bird City")
		npc.NPC_LINK1(5, "Mine Cave")
		npc.NPC_LINK1(6, "Market")
		npc.NPC_LINK1(npc.DIALOG_QUIT, "Just passing by.")
		npc.NPC_SETFACE(currentDialogNumber)
		npc.NPC_FINISH()
	end
	if(currentDialogNumber == 1) then
		npc.NPC_SAY("Sure I bring you to PC")
		npc.NPC_LINK1(npc.DIALOG_QUIT, "kk, ty")
		npc.NPC_SETFACE(currentDialogNumber)
		npc.NPC_FINISH()
	end
	if(currentDialogNumber == 2) then
		npc.NPC_SAY("Sure I bring you to DC")
		npc.NPC_LINK1(npc.DIALOG_QUIT, "kk, ty")
		npc.NPC_SETFACE(currentDialogNumber)
		npc.NPC_FINISH()
	end
	if(currentDialogNumber == 3) then
		npc.NPC_SAY("Sure I bring you to AM")
		npc.NPC_LINK1(npc.DIALOG_QUIT, "kk, ty")
		npc.NPC_SETFACE(currentDialogNumber)
		npc.NPC_FINISH()
	end
	if(currentDialogNumber == 4) then
		npc.NPC_SAY("Sure I bring you to BI")
		npc.NPC_LINK1(npc.DIALOG_QUIT, "kk, ty")
		npc.NPC_SETFACE(currentDialogNumber)
		npc.NPC_FINISH()
	end
	if(currentDialogNumber == 5) then
		npc.NPC_SAY("Sure I bring you to Mine")
		npc.NPC_LINK1(npc.DIALOG_QUIT, "kk, ty")
		npc.NPC_SETFACE(currentDialogNumber)
		npc.NPC_FINISH()
	end
	if(currentDialogNumber == 6) then
		npc.NPC_SAY("Sure I bring you to Market")
		npc.NPC_LINK1(npc.DIALOG_QUIT, "kk, ty")
		npc.NPC_SETFACE(currentDialogNumber)
		npc.NPC_FINISH()
	end
end