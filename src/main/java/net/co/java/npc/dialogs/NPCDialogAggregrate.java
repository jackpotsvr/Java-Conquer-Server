package net.co.java.npc.dialogs;

import net.co.java.entity.NPC;
import net.co.java.packets.NPCDialogPacket;

import java.util.ArrayList;
import java.util.List;

/**
 * Created by Thomas on 06/04/2015.
 */
public class NPCDialogAggregrate {

    private final List<NPCDialog> dialogs;

    private final static int INITIAL_DIALOG = -1;

    private int currentDialogNumber;
    private final NPC npc;


    public NPCDialogAggregrate(NPC npc) {
        dialogs = new ArrayList<>();
        this.currentDialogNumber = INITIAL_DIALOG;
        this.npc = npc;
    }

    protected NPCDialogAggregrate(NPC npc, int currentDialog) {
        this(npc);
        this.currentDialogNumber = currentDialog;
    }

    public void setCurrentDialogNumber(int currentDialogNumber) {
        this.currentDialogNumber = currentDialogNumber;
    }

    public List<NPCDialog> getDialogs() {
        return dialogs;
    }

    /**
     * For the LUA scripts.
     * @return
     */
    public int getNPCFace() {
        return npc.getFace();
    }

    /**
     * For the LUA Scripts.
     * @return
     */
    public int getCurrentDialogNumber() {
        return currentDialogNumber;
    }

    public void npcSay(final String message) {
        dialogs.add(new NPCDialog.NPCSay(message));
    }

    public void npcLink1(final short dialogNumber, final String value) {
        dialogs.add(new NPCDialog.NPCLink1(dialogNumber, value));
    }

    public void npcLink2(final short dialogNumber, final String text) {
        dialogs.add(new NPCDialog.NPCLink2(dialogNumber, text));
    }

    public void npcSetFace(final int face) {
        dialogs.add(new NPCDialog.NPCSetFace(face));
    }

    public void npcFinish() {
        dialogs.add(new NPCDialog.NPCFinish());
    }

    public void spamAllThings() {
        for(NPCDialog dialog : dialogs) {
            System.out.println(dialog.toString());
        }
    }

    public static enum NPCDialogType {
        NPC_SAY,
        NPC_LINK1,
        NPC_LINK2,
        NPC_SETFACE,
        NPC_FINISH
    }

    public static abstract class NPCDialog {
        protected final NPCDialogType type;

        protected NPCDialog(NPCDialogType type) {
            this.type = type;
        }

        public abstract NPCDialogPacket getPacket();

        protected static class NPCSay extends NPCDialog {
            private final String message;

            protected NPCSay(final String message) {
                super(NPCDialogType.NPC_SAY);
                this.message = message;
            }

            @Override
            public NPCDialogPacket getPacket() {
                return NPCDialogPacket.npcSay(message);
            }
        }

        protected static class NPCLink1 extends NPCDialog {
            private final short dialogNumber;
            private final String value;

            protected NPCLink1(final short dialogNumber, final String value) {
                super(NPCDialogType.NPC_LINK1);
                this.dialogNumber = dialogNumber;
                this.value = value;
            }

            @Override
            public NPCDialogPacket getPacket() {
                return NPCDialogPacket.npcLink1(dialogNumber, value);
            }
        }

        protected static class NPCLink2 extends NPCDialog {
            private final short dialogNumber;
            private final String text;

            protected NPCLink2(final short dialogNumber, final String text) {
                super(NPCDialogType.NPC_LINK2);
                this.dialogNumber = dialogNumber;
                this.text = text;
            }

            @Override
            public NPCDialogPacket getPacket() {
                return NPCDialogPacket.npcLink2(dialogNumber, text);
            }
        }

        protected static class NPCSetFace extends NPCDialog {
            private final int face;

            protected NPCSetFace(final int face) {
                super(NPCDialogType.NPC_SETFACE);
                this.face = face;
            }

            @Override
            public NPCDialogPacket getPacket() {
                return NPCDialogPacket.npcSetFace(face);
            }
        }

        protected static class NPCFinish extends NPCDialog {
            protected NPCFinish() {
                super(NPCDialogType.NPC_FINISH);
            }

            @Override
            public NPCDialogPacket getPacket() {
                return NPCDialogPacket.npcFinish();
            }
        }
    }

}
