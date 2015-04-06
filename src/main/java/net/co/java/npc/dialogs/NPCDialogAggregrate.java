package net.co.java.npc.dialogs;

import java.util.ArrayList;
import java.util.List;

/**
 * Created by Thomas on 06/04/2015.
 */
public class NPCDialogAggregrate {

    private final List<NPCDialog> dialogs;

    public NPCDialogAggregrate() {
        dialogs = new ArrayList<NPCDialog>();
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
        private final NPCDialogType type;

        private NPCDialog(NPCDialogType type) {
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
                return new NPCDialogPacket();
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
                return new NPCDialogPacket();
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
                return new NPCDialogPacket();
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
                return new NPCDialogPacket();
            }
        }

        protected static class NPCFinish extends NPCDialog {
            protected NPCFinish() {
                super(NPCDialogType.NPC_FINISH);
            }

            @Override
            public NPCDialogPacket getPacket() {
                return new NPCDialogPacket();
            }
        }



    }

}
